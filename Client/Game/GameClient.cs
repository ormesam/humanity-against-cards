using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Events;
using Common.Dtos;
using Common.Exceptions;
using Common.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client.Game {
    public class GameClient : HubClientBase {
        private GameState state;
        private string code;
        private bool isConnected;
        private IList<AnswerCard> hand;

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

        public IList<AnswerCard> Hand {
            get => hand;
            private set {
                if (hand != value) {
                    hand = value;
                    UpdateUI();
                }
            }
        }

        public GameClient(NavigationManager navigationManager) : base(navigationManager) {
            hand = new List<AnswerCard>();
        }

        protected override void LinkHubConnections() {
            HubConnection.On<string>(nameof(IGameClient.PlayerJoined), (name) => UpdateUI());
            HubConnection.On<IList<AnswerCard>>(nameof(IGameClient.ShowHand), (hand) => { Hand = hand; });
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

        private void UpdateUI() {
            UIUpdated?.Invoke();
        }
    }
}
