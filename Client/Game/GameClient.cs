using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Client.Events;
using Common.Dtos;
using Common.Exceptions;
using Common.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Game {
    public class GameClient : HubClientBase {
        private GameState state;
        private string code;
        private bool isConnected;
        private QuestionCard currentQuestion;
        private IList<AnswerCard> hand;
        private IList<Guid> cardsToSubmit;
        private IList<SubmittedCard> submittedAnswers;

        public event UIUpdatedEventHandler UIUpdated;

        public GameState State {
            get => state;
            set {
                if (state != value) {
                    state = value;
                    UpdateUI();
                }
            }
        }

        public string Code {
            get => code;
            private set {
                if (code != value) {
                    code = value;
                    UpdateUI();
                }
            }
        }

        public bool IsConnected {
            get => isConnected;
            private set {
                if (isConnected != value) {
                    isConnected = value;
                    UpdateUI();
                }
            }
        }

        public QuestionCard CurrentQuestion {
            get => currentQuestion;
            set {
                currentQuestion = value;
                UpdateUI();
            }
        }

        public IList<AnswerCard> Hand {
            get => hand;
            private set {
                if (hand != value) {
                    hand = value;
                    UpdateUI();
                }
            }
        }

        public IList<SubmittedCard> SubmittedAnswers {
            get => submittedAnswers;
            private set {
                if (submittedAnswers != value) {
                    submittedAnswers = value;
                    UpdateUI();
                }
            }
        }

        public GameClient(NavigationManager navigationManager) : base(navigationManager) {
            cardsToSubmit = new List<Guid>();
            hand = new List<AnswerCard>();
            submittedAnswers = new List<SubmittedCard>();
        }

        protected override void RegisterHubConnections() {
            Register<string>(nameof(IGameClient.PlayerJoined), (name) => UpdateUI());
            Register<IList<AnswerCard>>(nameof(IGameClient.ShowHand), (cards) => {
                SubmittedAnswers.Clear();
                Hand = cards;
            });
            Register<GameState>(nameof(IGameClient.GameStateChanged), (state) => { State = state; });
            Register<QuestionCard>(nameof(IGameClient.ShowQuestion), (question) => { CurrentQuestion = question; });
            Register<IList<SubmittedCard>>(nameof(IGameClient.ShowAnswers), (submittedCards) => {
                cardsToSubmit.Clear();
                Hand.Clear();
                SubmittedAnswers = submittedCards;
            });
            Register<SubmittedCard>(nameof(IGameClient.ShowWinningCard), (winningCard) => {
                submittedAnswers.Single(i => i.Id == winningCard.Id).IsWinningCard = true;
                UpdateUI();
            });
        }

        public async Task CreateGame(string name) {
            await StartAsync();

            Code = await this.Call(i => i.CreateGame(name));

            IsConnected = true;
        }

        public async Task JoinGame(string name, string code) {
            await StartAsync();

            try {

            } catch (GameNotFoundException) {
                await StopAsync();

                return;
            }

            GameState state = await this.Call(i => i.JoinGame(name, code));

            State = state;
            Code = code;

            IsConnected = true;
        }

        public async Task LeaveGame() {
            await StopAsync();

            IsConnected = false;
        }

        public async Task StartGame() {
            await this.Call(i => i.StartGame(Code));
        }

        public async Task SubmitCard(Guid cardId) {
            if (cardsToSubmit.Count == CurrentQuestion.NoOfAnswers) {
                return;
            }

            cardsToSubmit.Add(cardId);

            if (cardsToSubmit.Count == CurrentQuestion.NoOfAnswers) {
                await this.Call(i => i.SubmitCards(Code, cardsToSubmit));
            }
        }

        public async Task Vote(Guid cardId) {
            await this.Call(i => i.Vote(Code, cardId));
        }

        private void UpdateUI() {
            Debug.WriteLine("UI Updated");
            UIUpdated?.Invoke();
        }
    }
}
