using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client.Game {
    public abstract class HubClientBase : IAsyncDisposable {
        protected HubConnection HubConnection;
        private NavigationManager navigationManager;
        private bool started;

        public HubClientBase(NavigationManager navigationManager) {
            this.navigationManager = navigationManager;
        }

        public async Task StartAsync() {
            if (started) {
                return;
            }

            HubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("http://localhost:64075/gameHub"))
                .Build();

            started = true;

            LinkHubConnections();

            await HubConnection.StartAsync();
        }

        protected abstract void LinkHubConnections();

        public async Task StopAsync() {
            if (!started) {
                return;
            }

            await HubConnection.StopAsync();
            await HubConnection.DisposeAsync();

            HubConnection = null;
            started = false;
        }

        public async ValueTask DisposeAsync() {
            await StopAsync();
        }
    }
}