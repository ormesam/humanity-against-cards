using System;
using System.Collections.Concurrent;
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
        public ConcurrentDictionary<string, SubmittedCard> SubmittedAnswers { get; set; }

        public Session(IHubContext<GameHub, IGameClient> gameHub, string code) {
            this.gameHub = gameHub;
            Code = code;
            GameState = GameState.NotStarted;
            Players = new List<Player>();
            AnswerPile = new Queue<AnswerCard>();
            QuestionPile = new Queue<QuestionCard>();
            SubmittedAnswers = new ConcurrentDictionary<string, SubmittedCard>();
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
            return SubmittedAnswers.Count == Players.Count;
        }

        private bool CheckIfMaxAnswersHaveBeenSubmitted() {
            return SubmittedAnswers.SelectMany(i => i.Value.AnswerCards).Count() == CurrentQuestion.NoOfAnswers * Players.Count;
        }

        private async Task CalculateAndDisplayWinningCard() {
            if (!SubmittedAnswers.Any()) {
                return;
            }

            var topCard = SubmittedAnswers
                .OrderByDescending(i => i.Value.Votes)
                .First();

            await gameHub.Clients.Group(Code).ShowWinningCard(topCard.Value);
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
            await gameHub.Clients.Group(Code).ShowAnswers(SubmittedAnswers.Select(i => i.Value).ToList());
        }

        private async Task ShowQuestion() {
            await gameHub.Clients.Group(Code).ShowQuestion(CurrentQuestion);
        }

        private async Task DealCards() {
            foreach (var player in Players) {
                if (player.Hand.Count < maxCardsInHand) {
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

        public async Task<GameState> Join(string connectionId, string name) {
            Players.Add(new Player(connectionId, name));

            await gameHub.Clients.Group(Code).PlayerJoined(name);

            return GameState;
        }

        public void SubmitCard(string connectionId, Guid answerCardId) {
            var player = Players.Single(i => i.ConnectionId == connectionId);

            int numberOfCardsSubmitted = SubmittedAnswers[connectionId].AnswerCards.Count;

            if (numberOfCardsSubmitted < CurrentQuestion.NoOfAnswers) {
                var answerCard = player.Hand.Single(i => i.Id == answerCardId);

                if (!SubmittedAnswers.ContainsKey(connectionId)) {
                    SubmittedAnswers.TryAdd(connectionId, new SubmittedCard {
                        PlayerId = connectionId,
                    });
                }

                SubmittedAnswers[connectionId].AnswerCards.Add(answerCard);
            }
        }

        public void Vote(string connectionId, Guid submittedCardId) {
            var player = Players.Single(i => i.ConnectionId == connectionId);

            if (player.Voted) {
                return;
            }

            player.Voted = true;

            var card = SubmittedAnswers.Single(i => i.Value.Id == submittedCardId).Value;
            card.Votes++;
        }
    }
}
