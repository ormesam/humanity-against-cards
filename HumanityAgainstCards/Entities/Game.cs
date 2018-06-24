using HumanityAgainstCards.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Entities
{
    public class Game
    {
        private const int numberOfCardsInHand = 10;

        private string roomCode;
        private IDictionary<string, Player> players;
        private IList<QuestionCard> questionCards;
        private IList<Card> answerCards;
        private IClient groupHub => GetGroupHub();
        private GameStatus status;

        private QuestionCard currentQuestion;
        private IDictionary<string, string> currentAnswers;
        private IDictionary<string, int> currentVotes;

        public Game(string roomCode)
        {
            players = new Dictionary<string, Player>();
            questionCards = new List<QuestionCard>();
            answerCards = new List<Card>();

            this.roomCode = roomCode;
            status = GameStatus.Created;
        }

        private void PopulateCards()
        {
            CardGenerator generator = new CardGenerator();

            questionCards = generator.GenerateQuestions();
            answerCards = generator.GenerateAnswers();
        }

        public void AddPlayer(string connectionId, string name)
        {
            Player player = new Player(connectionId, name);

            players.Add(connectionId, player);

            groupHub.PlayerJoined(name);
            player.GetPlayerHub().RoomCodeChanged(roomCode);
        }

        public async Task Start()
        {
            if (status != GameStatus.Created)
            {
                return;
            }

            status = GameStatus.Running;

            PopulateCards();

            while (questionCards.Any())
            {
                currentQuestion = null;
                currentAnswers = new Dictionary<string, string>();
                currentVotes = new Dictionary<string, int>();

                PopulateHands();
                ShowNext();
                ShowHands();

                // give users time to pick cards, show timer maybe?
                await Task.Delay(1000 * 20); // just leave at 20 seconds for now

                ShowSelectedCards();

                // give users time to select the winning card
                await Task.Delay(1000 * 20); // just leave at 20 seconds for now

                CalculateAndShowWinningCards();

                await Task.Delay(1000 * 10); // just leave at 10 seconds for now
            }
        }

        private void PopulateHands()
        {
            foreach (var player in players)
            {
                while (player.Value.Hand.Count < numberOfCardsInHand && answerCards.Any())
                {
                    var card = answerCards.FirstOrDefault();
                    player.Value.AddToHand(card);
                    answerCards.Remove(card);
                }
            }
        }

        private void ShowHands()
        {
            foreach (var player in players)
            {
                player.Value.ShowHand();
            }
        }

        private void ShowNext()
        {
            currentQuestion = questionCards.First();

            groupHub.NewQuestion(currentQuestion);

            questionCards.Remove(currentQuestion);
        }

        private void ShowSelectedCards()
        {
            IList<string> flatAnswers = currentAnswers.Values
                .Select(row => string.Join(" | ", row))
                .ToList();

            groupHub.ShowSelectedCards(flatAnswers);
        }

        public void SubmitCard(string connectionId, string card)
        {
            if (!currentAnswers.ContainsKey(connectionId))
            {
                currentAnswers.Add(connectionId, card);
            }
            else
            {
                currentAnswers[connectionId] += " | " + card;
            }

            players[connectionId].RemoveCardFromHand(card);
        }

        public void SubmitVote(string card)
        {
            if (!currentVotes.ContainsKey(card))
            {
                currentVotes.Add(card, 0);
            }

            currentVotes[card]++;
        }

        private void CalculateAndShowWinningCards()
        {
            if (!currentVotes.Any())
            {
                // no votes cast, skip
                return;
            }

            // will need to account for multiple cards with the same votes at somepoint
            var winningCard = currentVotes
                .OrderByDescending(row => row.Value)
                .FirstOrDefault();

            string playerConnection = currentAnswers
                .Where(row => row.Value == winningCard.Key)
                .Select(row => row.Key)
                .SingleOrDefault();

            Player winner = players[playerConnection];
            winner.Points++;

            groupHub.ShowWinningCard(winner.Name, winningCard.Key, winningCard.Value);
        }

        private IClient GetGroupHub()
        {
            return GlobalHost.ConnectionManager.GetHubContext<GameHub, IClient>().Clients.Group(roomCode);
        }
    }
}