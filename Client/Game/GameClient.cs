using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Interfaces;

namespace Client.Game {
    public class GameClient : HubClientBase {
        public event EventHandler MessageReceived;

        public GameClient(NavigationManager navigationManager) : base(navigationManager) {
        }

        public void Test(string message) {
            MessageReceived?.Invoke(message, null);
        }

        protected override void LinkHubConnections() {
            HubConnection.On<string>(nameof(IGameHubClient.Test), (message) => Test(message));
        }
    }
}
