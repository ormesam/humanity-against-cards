using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Shared.Interfaces;

namespace Server.Game {
    public class Controller {
        private readonly IHubContext<GameHub, IGameClient> gameHub;

        public Controller(IHubContext<GameHub, IGameClient> gameHub) {
            this.gameHub = gameHub;
        }

        public void Connected() {
            gameHub.Clients.All.Test("New connection...");
        }
    }
}
