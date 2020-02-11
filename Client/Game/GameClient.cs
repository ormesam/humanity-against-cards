using System.Threading.Tasks;
using Client.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Dtos;
using Shared.Exceptions;
using Shared.Interfaces;

namespace Client.Game {
    public class GameClient : HubClientBase {
        private GameState state;
        private string code;
        private bool isConnected;

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

        public GameClient(NavigationManager navigationManager) : base(navigationManager) {
        }

        protected override void LinkHubConnections() {
            HubConnection.On<string>(nameof(IGameClient.PlayerJoined), (name) => UpdateUI());
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
            await this.Call(i => i.LeaveGame(Code));

            await StopAsync();

            IsConnected = false;
        }

        private void UpdateUI() {
            UIUpdated?.Invoke();
        }
    }
}
