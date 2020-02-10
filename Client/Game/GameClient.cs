using System;
using System.Threading.Tasks;
using Client.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Interfaces;

namespace Client.Game {
    public class GameClient : HubClientBase {
        public string Code { get; private set; }
        public bool IsConnected { get; private set; }

        public event PlayerJoinedEventHandler PlayerJoined;
        public event EventHandler ConnectionChanged;

        public GameClient(NavigationManager navigationManager) : base(navigationManager) {
        }

        protected override void LinkHubConnections() {
            HubConnection.On<string>(nameof(IGameClient.PlayerJoined), (name) => PlayerJoined?.Invoke(new PlayerJoinedEventArgs(name)));
        }

        public async Task CreateGame(string name) {
            await StartAsync();

            Code = await this.Call(i => i.CreateGame(name));

            IsConnected = true;
            ConnectionChanged.Invoke(null, null);
        }

        public async Task JoinGame(string name, string code) {
            await StartAsync();

            bool connected = await this.Call(i => i.JoinGame(name, code));

            if (!connected) {
                await StopAsync();

                return;
            }

            Code = code;

            IsConnected = true;
            ConnectionChanged.Invoke(null, null);
        }

        public async Task LeaveGame() {
            await this.Call(i => i.LeaveGame(Code));

            await StopAsync();

            IsConnected = false;
            ConnectionChanged.Invoke(null, null);
        }
    }
}
