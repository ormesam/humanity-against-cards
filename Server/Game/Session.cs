using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Shared.Dtos;
using Shared.Interfaces;

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
        public IList<AnswerCard> SubmittedAnswers { get; set; }

        public Session(IHubContext<GameHub, IGameClient> gameHub, string code) {
            this.gameHub = gameHub;
            Code = code;
            GameState = GameState.NotStarted;
            Players = new List<Player>();
            AnswerPile = new Queue<AnswerCard>();
            QuestionPile = new Queue<QuestionCard>();
            SubmittedAnswers = new List<AnswerCard>();
        }

        public async Task Start() {
            while (Players.Any() && QuestionPile.Any()) {
                SetUpRound();
                await DealCards();
                await ShowQuestion();
                // Wait for users to pick their answers
                await Sleep(30, CheckIfMaxAnswersHaveBeenSubmitted);
                await ShowAnswers();
                // Wait for users to cast their votes
                await Sleep(30, CheckIfMaxVotesHaveBeenCast);
                await CalculateAndDisplayWinningCard();
            }
        }

        private bool CheckIfMaxVotesHaveBeenCast() {
            return SubmittedAnswers.Sum(i => i.Votes) == Players.Count;
        }

        private bool CheckIfMaxAnswersHaveBeenSubmitted() {
            return SubmittedAnswers.Count == CurrentQuestion.NoOfAnswers * Players.Count;
        }

        private async Task CalculateAndDisplayWinningCard() {
            if (!SubmittedAnswers.Any()) {
                return;
            }

            var topCard = SubmittedAnswers
                .OrderByDescending(i => i.Votes)
                .First();

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
                if (player.Hand.Count < maxCardsInHand) {
                    var card = AnswerPile.Dequeue();

                    player.Hand.Add(card);
                    await gameHub.Clients.Client(player.ConnectionId).DealCard(card);
                }
            }
        }

        private void SetUpRound() {
            SubmittedAnswers.Clear();
            CurrentQuestion = QuestionPile.Dequeue();
        }

        public async Task<GameState> Join(IHubContext<GameHub, IGameClient> gameHub, string connectionId, string name) {
            Players.Add(new Player(connectionId, name));

            await gameHub.Clients.Group(Code).PlayerJoined(name);

            return GameState;
        }
    }
}
