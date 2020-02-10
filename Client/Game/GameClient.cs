using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Interfaces;

namespace Client.Game {
    public class GameClient : HubClientBase {
        private string code;
        public bool IsConnected { get; private set; }

        public event EventHandler MessageReceived;
        public event EventHandler ConnectionChanged;

        public GameClient(NavigationManager navigationManager) : base(navigationManager) {
        }

        protected override void LinkHubConnections() {
            HubConnection.On<string>(nameof(IGameClient.Test), (message) => MessageReceived?.Invoke(message, null));
        }

        public async Task CreateGame(string name) {
            await StartAsync();

            code = await this.Call(i => i.CreateGame(name));

            IsConnected = true;
            ConnectionChanged.Invoke(null, null);
        }

        public async Task JoinGame(string name, string code) {
            await StartAsync();

            await this.Call(i => i.JoinGame(name, code));

            this.code = code;

            IsConnected = true;
            ConnectionChanged.Invoke(null, null);
        }

        public async Task LeaveGame() {
            await this.Call(i => i.LeaveGame(code));

            await StopAsync();

            IsConnected = false;
            ConnectionChanged.Invoke(null, null);
        }
    }
}
