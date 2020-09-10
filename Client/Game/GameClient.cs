using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Events;
using Common.Dtos;
using Common.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Game {
    public class GameClient : HubClientBase {
        private GameState state;
        private string code;
        private bool hasVoted;
        private bool isSettingUp;
        private int timer;
        private QuestionCard currentQuestion;
        private IList<AnswerCard> hand;
        private IList<Guid> cardsToSubmit;
        private IList<SubmittedCard> submittedAnswers;
        private IList<LeaderboardItem> leaderboard;

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

        public int Timer {
            get => timer;
            set {
                if (timer != value) {
                    timer = value;
                    UpdateUI();
                }
            }
        }

        public IList<LeaderboardItem> Leaderboard {
            get => leaderboard;
            set {
                if (leaderboard != value) {
                    leaderboard = value;
                    UpdateUI();
                }
            }
        }

        public GameClient(NavigationManager navigationManager) : base(navigationManager) {
            cardsToSubmit = new List<Guid>();
            hand = new List<AnswerCard>();
            submittedAnswers = new List<SubmittedCard>();
            leaderboard = new List<LeaderboardItem>();
        }

        protected override void RegisterHubConnections() {
            Register<string>(nameof(IGameClient.PlayerJoined), (name) => UpdateUI());
            Register<IList<AnswerCard>>(nameof(IGameClient.ShowHand), (cards) => {
                hasVoted = false;
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
            Register<int>(nameof(IGameClient.UpdateTimer), (seconds) => {
                Timer = seconds;
            });
            Register<IList<LeaderboardItem>>(nameof(IGameClient.UpdateLeaderboard), (leaderboard) => { Leaderboard = leaderboard; });
        }

        public async Task SetupGame(string name, string code) {
            if (isSettingUp) {
                return;
            }

            isSettingUp = true;

            if (string.IsNullOrWhiteSpace(name)) {
                isSettingUp = false;
                return;
            }

            switch (State) {
                case GameState.Creating:
                    await CreateGame(name);
                    break;
                case GameState.Joining:
                    if (string.IsNullOrWhiteSpace(code)) {
                        return;
                    }

                    await JoinGame(name, code);
                    break;
                default:
                    break;
            }

            isSettingUp = false;
        }

        private async Task CreateGame(string name) {
            await StartAsync();

            Code = await this.Call(i => i.CreateGame(name));

            State = GameState.NotStarted;
        }

        private async Task JoinGame(string name, string code) {
            await StartAsync();

            bool joined = await this.Call(i => i.JoinGame(name, code));

            if (!joined) {
                await StopAsync();

                // show error message

                return;
            }

            Code = code;

            State = GameState.NotStarted;
        }

        public async Task LeaveGame() {
            await StopAsync();

            State = GameState.None;
        }

        public async Task StartGame() {
            await this.Call(i => i.StartGame(Code));
        }

        public async Task SubmitCard(AnswerCard card) {
            if (card.IsSelected || cardsToSubmit.Count == CurrentQuestion.NoOfAnswers) {
                return;
            }

            card.IsSelected = true;

            cardsToSubmit.Add(card.Id);

            if (cardsToSubmit.Count == CurrentQuestion.NoOfAnswers) {
                await this.Call(i => i.SubmitCards(Code, cardsToSubmit));
            }
        }

        public async Task Vote(SubmittedCard card) {
            if (hasVoted || card.PlayerId == HubConnection.ConnectionId) {
                return;
            }

            hasVoted = true;

            card.IsSelected = true;

            await this.Call(i => i.Vote(Code, card.Id));
        }

        private void UpdateUI() {
            UIUpdated?.Invoke();
        }
    }
}
