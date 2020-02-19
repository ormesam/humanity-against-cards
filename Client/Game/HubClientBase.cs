﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace Client.Game {
    public abstract class HubClientBase : IAsyncDisposable {
        public HubConnection HubConnection;
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

            RegisterHubConnections();

            await HubConnection.StartAsync();
        }

        protected abstract void RegisterHubConnections();

        public async Task StopAsync() {
            if (!started) {
                return;
            }

            await HubConnection.StopAsync();
            await HubConnection.DisposeAsync();

            HubConnection = null;
            started = false;
        }

        public void Register<T>(string methodName, Action<T> handler) {
            HubConnection.On<object>(methodName, (obj) => {
                var parsed = JsonConvert.DeserializeObject<T>(obj.ToString());

                handler(parsed);
            });
        }

        public async ValueTask DisposeAsync() {
            await StopAsync();
        }
    }
}