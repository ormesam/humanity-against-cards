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
        private IList<Card> questionCards;
        private IList<Card> answerCards;
        private IClient groupHub => GetGroupHub();
        private GameStatus status;

        private Card currentQuestion;
        private IDictionary<string, IList<string>> currentAnswers;

        public Game(string roomCode)
        {
            players = new Dictionary<string, Player>();
            questionCards = new List<Card>();
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
        }

        public async Task Start()
        {
            if (status != GameStatus.Created)
            {
                return;
            }

            status = GameStatus.Running;
            currentQuestion = null;
            currentAnswers = new Dictionary<string, IList<string>>();

            PopulateCards();

            while (questionCards.Any())
            {
                PopulateHands();
                ShowNext();
                ShowHands();

                // give users time to pick cards, show timer maybe?
                await Task.Delay(1000 * 10); // just leave at 10 seconds for now

                ShowSelectedCards();

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
                currentAnswers.Add(connectionId, new List<string>());
            }

            currentAnswers[connectionId].Add(card);
            players[connectionId].RemoveCardFromHand(card);
        }

        private IClient GetGroupHub()
        {
            return GlobalHost.ConnectionManager.GetHubContext<GameHub, IClient>().Clients.Group(roomCode);
        }
    }
}