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

        private IDictionary<string, Player> players;
        private IList<QuestionCard> allQuestionCards;
        private IList<AnswerCard> allAnswerCards;

        private QuestionCard currentQuestion;
        private IList<VotingCard> votes;
        private bool skipTimer;
        private int maxSubmitCount;
        private int currentSubmitCount;
        private int maxVoteCount;
        private int currentVoteCount;

        private IClient groupHub => GetGroupHub();

        public GameStatus Status { get; private set; }
        public string RoomCode { get; private set; }

        public Game(string roomCode)
        {
            players = new Dictionary<string, Player>();
            allQuestionCards = new List<QuestionCard>();
            allAnswerCards = new List<AnswerCard>();

            this.RoomCode = roomCode;
            Status = GameStatus.Created;
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

            UpdateLeaderboard();

            player.GetPlayerHub().RoomCodeChanged(RoomCode);
        }

        public async Task Start()
        {
            if (Status != GameStatus.Created)
            {
                return;
            }

            Status = GameStatus.Running;

            PopulateCards();

            while (allQuestionCards.Any() && Status == GameStatus.Running)
            {
                currentQuestion = null;
                maxSubmitCount = 0;
                maxVoteCount = players.Count;

                currentSubmitCount = 0;
                currentVoteCount = 0;

                votes = new List<VotingCard>();

                PopulateHands();
                ShowNext();
                ShowHands();

                await StartTimer(60);

                ShowSelectedCards();

                await StartTimer(60);

                CalculateAndShowWinningCards();

                await StartTimer(10);
            }

            Status = GameStatus.Stopped;
        }

        private async Task StartTimer(int seconds)
        {
            skipTimer = false;

            while (seconds > 0)
            {
                if (skipTimer)
                {
                    break;
                }

                groupHub.SetTimer(seconds);

                await Task.Delay(1000);
                seconds--;
            }
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
            maxSubmitCount = currentQuestion.BlankCount * players.Count;

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

            currentSubmitCount++;

            // skip the timer if the max number of submitted cards has been reached
            if (currentSubmitCount == maxSubmitCount)
            {
                skipTimer = true;
            }
        }

        public void SubmitVote(Guid cardId)
        {
            VotingCard card = votes
                .Where(row => row.Id == cardId)
                .SingleOrDefault();

            card.Votes++;

            currentVoteCount++;

            // skip the timer if the max number of votes has been reached
            if (currentVoteCount == maxVoteCount)
            {
                skipTimer = true;
            }
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

            groupHub.ShowWinningCard(winner.Name, winningCard.Id, winningCard.Votes);

            UpdateLeaderboard();
        }

        public bool ContainsPlayer(string connectionId)
        {
            return players.ContainsKey(connectionId);
        }

        public void RemovePlayer(string connectionId)
        {
            if (ContainsPlayer(connectionId))
            {
                string playerName = players[connectionId].Name;

                players.Remove(connectionId);

                groupHub.PlayerLeft(playerName);

                UpdateLeaderboard();
            }

            if (!players.Any())
            {
                Status = GameStatus.Stopped;
            }
        }

        private void UpdateLeaderboard()
        {
            IList<Player> leaderboard = players.Values
                .OrderByDescending(row => row.Points)
                .ToList();

            groupHub.UpdateLeaderboard(leaderboard);
        }

        private IClient GetGroupHub()
        {
            return GlobalHost.ConnectionManager.GetHubContext<GameHub, IClient>().Clients.Group(RoomCode);
        }
    }
}