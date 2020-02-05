using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Game;
using Shared.Interfaces;

namespace Server.Hubs {
    public class GameHub : Hub<IGameHubClient> {
        private readonly Controller controller;

        public GameHub(Controller controller) {
            this.controller = controller;
        }


        public override Task OnConnectedAsync() {
            controller.Connected();

            return base.OnConnectedAsync();
        }
    }
}
