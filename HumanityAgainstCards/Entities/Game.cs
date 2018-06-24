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
        private IList<QuestionCard> allQuestionCards;
        private IList<AnswerCard> allAnswerCards;
        private GameStatus status;

        private QuestionCard currentQuestion;
        private IList<VotingCard> votes;
        private IClient groupHub => GetGroupHub();

        public Game(string roomCode)
        {
            players = new Dictionary<string, Player>();
            allQuestionCards = new List<QuestionCard>();
            allAnswerCards = new List<AnswerCard>();

            this.roomCode = roomCode;
            status = GameStatus.Created;
        }

        private void PopulateCards()
        {
            CardGenerator generator = new CardGenerator();

            allQuestionCards = generator.GenerateQuestions();
            allAnswerCards = generator.GenerateAnswers();
        }

        public void AddPlayer(string connectionId, string name)
        {
            Player player = new Player(connectionId, name);

            players.Add(connectionId, player);

            groupHub.PlayerJoined(name);

            UpdatePlayerLeaderboard();

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

            while (allQuestionCards.Any())
            {
                currentQuestion = null;
                votes = new List<VotingCard>();

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

            status = GameStatus.Stopped;
        }

        private void PopulateHands()
        {
            foreach (var player in players)
            {
                while (player.Value.Hand.Count < numberOfCardsInHand && allAnswerCards.Any())
                {
                    var card = allAnswerCards
                        .Where(row => row.IsAvailable)
                        .FirstOrDefault();

                    player.Value.AddToHand(card);
                    card.PlayerId = player.Key;
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
            currentQuestion = allQuestionCards.First();

            groupHub.NewQuestion(currentQuestion);

            allQuestionCards.Remove(currentQuestion);
        }

        private void ShowSelectedCards()
        {
            groupHub.ShowVotingCards(votes);
        }

        public void SubmitCard(string connectionId, Guid cardId)
        {
            // as some questions can have multiple answers we need to handle users submitting multiple cards
            VotingCard votingCard = votes
                .Where(row => row.PlayerId == connectionId)
                .SingleOrDefault();

            if (votingCard == null)
            {
                votingCard = new VotingCard
                {
                    Id = Guid.NewGuid(),
                    PlayerId = connectionId,
                    Values = new List<string>(),
                    Votes = 0,
                };

                votes.Add(votingCard);
            }

            var card = allAnswerCards
                .Where(row => row.Id == cardId)
                .SingleOrDefault();

            votingCard.Values.Add(card.Value);

            players[connectionId].RemoveCardFromHand(card);
        }

        public void SubmitVote(Guid cardId)
        {
            VotingCard card = votes
                .Where(row => row.Id == cardId)
                .SingleOrDefault();

            card.Votes++;
        }

        private void CalculateAndShowWinningCards()
        {
            if (votes.All(v => v.Votes == 0))
            {
                // no votes cast, skip
                return;
            }

            // will need to account for multiple cards with the same votes at somepoint
            var winningCard = votes
                .OrderByDescending(row => row.Votes)
                .FirstOrDefault();

            Player winner = players[winningCard.PlayerId];
            winner.Points++;

            groupHub.ShowWinningCard(winner.Name, winningCard.Value, winningCard.Votes);

            UpdatePlayerLeaderboard();
        }

        private void UpdatePlayerLeaderboard()
        {
            IList<Player> leaderboard = players.Values
                .OrderByDescending(row => row.Points)
                .ToList();

            groupHub.UpdateLeaderboard(leaderboard);
        }

        private IClient GetGroupHub()
        {
            return GlobalHost.ConnectionManager.GetHubContext<GameHub, IClient>().Clients.Group(roomCode);
        }
    }
}