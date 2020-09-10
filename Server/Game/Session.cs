using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Dtos;
using Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;

namespace Server.Game {
    public class Session {
        private readonly int maxCardsInHand = 10;
        private readonly IHubContext<GameHub, IGameClient> gameHub;

        public string Code { get; }
        public GameState GameState { get; private set; }
        public IList<Player> Players { get; set; }
        public Queue<AnswerCard> AnswerPile { get; set; }
        public Queue<QuestionCard> QuestionPile { get; set; }
        public QuestionCard CurrentQuestion { get; set; }
        public IList<SubmittedCard> SubmittedAnswers { get; set; }

        public Session(IHubContext<GameHub, IGameClient> gameHub, string code) {
            this.gameHub = gameHub;
            Code = code;
            GameState = GameState.NotStarted;
            Players = new List<Player>();
            AnswerPile = new Queue<AnswerCard>();
            QuestionPile = new Queue<QuestionCard>();
            SubmittedAnswers = new List<SubmittedCard>();
        }

        public async Task Start() {
            await ChangeGameState(GameState.Running);
        }

        public async Task Run() {
            while (GameState != GameState.Running) {
                await Task.Delay(1000);
            }

            SetUpGame();

            while (Players.Any() && QuestionPile.Any() && AnswerPile.Any()) {
                SetUpRound();
                await DealCards();
                await ShowQuestion();
                // Wait for users to pick their answers
                await Sleep(60, CheckIfMaxAnswersHaveBeenSubmitted);
                await ShowAnswers();
                // Wait for users to cast their votes
                await Sleep(60, CheckIfMaxVotesHaveBeenCast);
                await CalculateAndDisplayWinningCard();
                await Sleep(8, () => false);
                await UpdateLeaderboard();
            }

            await ChangeGameState(GameState.Ended);
        }

        private async Task ChangeGameState(GameState state) {
            GameState = state;
            await gameHub.Clients.Group(Code).GameStateChanged(state);
        }

        private void SetUpGame() {
            CardGenerator generator = new CardGenerator();

            foreach (var card in generator.GenerateQuestions()) {
                QuestionPile.Enqueue(card);
            }

            foreach (var card in generator.GenerateAnswers()) {
                AnswerPile.Enqueue(card);
            }
        }

        private bool CheckIfMaxVotesHaveBeenCast() {
            return SubmittedAnswers.Sum(i => i.Votes) == Players.Count;
        }

        private bool CheckIfMaxAnswersHaveBeenSubmitted() {
            return SubmittedAnswers.Count == Players.Count;
        }

        private async Task CalculateAndDisplayWinningCard() {
            if (!SubmittedAnswers.Any()) {
                return;
            }

            var topCard = SubmittedAnswers
                .OrderByDescending(i => i.Votes)
                .First();

            var winningPlayer = Players.Single(i => i.ConnectionId == topCard.PlayerId);
            winningPlayer.CardsWon.Add(CurrentQuestion);

            await gameHub.Clients.Group(Code).ShowWinningCard(topCard);
        }

        // Must be a more sophisticated way of doing this
        private async Task Sleep(int seconds, Func<bool> cancellationCheck) {
            for (int i = 0; i < seconds; i++) {
                await gameHub.Clients.Group(Code).UpdateTimer(seconds - i);

                if (cancellationCheck()) {
                    return;
                }

                await Task.Delay(1000);
            }
        }

        private async Task ShowAnswers() {
            await gameHub.Clients.Group(Code).ShowAnswers(SubmittedAnswers);
        }

        private async Task ShowQuestion() {
            await gameHub.Clients.Group(Code).ShowQuestion(CurrentQuestion);
        }

        private async Task DealCards() {
            foreach (var player in Players) {
                while (player.Hand.Count < maxCardsInHand && AnswerPile.Any()) {
                    var card = AnswerPile.Dequeue();

                    player.Hand.Add(card);
                }

                await gameHub.Clients.Client(player.ConnectionId).ShowHand(player.Hand);
            }
        }

        private void SetUpRound() {
            SubmittedAnswers.Clear();
            CurrentQuestion = QuestionPile.Dequeue();

            foreach (var player in Players) {
                player.Voted = false;
            }
        }

        public async Task<bool> Join(string connectionId, string name) {
            if (GameState != GameState.NotStarted) {
                return false;
            }

            Players.Add(new Player(connectionId, name));

            await UpdateLeaderboard();

            await gameHub.Clients.Client(connectionId).UpdateLeaderboard(GetLeaderboard());

            return true;
        }

        public async Task Leave(string connectionId) {
            var player = Players.SingleOrDefault(i => i.ConnectionId == connectionId);

            if (player == null) {
                return;
            }

            Players.Remove(player);

            await UpdateLeaderboard();
        }

        public void SubmitCards(string connectionId, IList<Guid> answerCardIds) {
            if (SubmittedAnswers.Any(i => i.PlayerId == connectionId)) {
                return;
            }

            var player = Players.Single(i => i.ConnectionId == connectionId);
            IList<AnswerCard> answerCards = new List<AnswerCard>();

            foreach (var answerCardId in answerCardIds) {
                var answerCard = player.Hand.SingleOrDefault(i => i.Id == answerCardId);

                answerCards.Add(answerCard);
                player.Hand.Remove(answerCard);
            }

            SubmittedAnswers.Add(new SubmittedCard {
                PlayerId = connectionId,
                PlayerName = player.Name,
                AnswerCards = answerCards,
            });
        }

        public void Vote(string connectionId, Guid submittedCardId) {
            var player = Players.Single(i => i.ConnectionId == connectionId);

            if (player.Voted) {
                return;
            }

            player.Voted = true;

            var card = SubmittedAnswers.Single(i => i.Id == submittedCardId);
            card.Votes++;
        }

        private async Task UpdateLeaderboard() {
            await gameHub.Clients.Group(Code).UpdateLeaderboard(GetLeaderboard());
        }

        private IList<LeaderboardItem> GetLeaderboard() {
            return Players
                .Select(i => new LeaderboardItem(i.Name, i.CardsWon.Count))
                .ToList();
        }
    }
}
