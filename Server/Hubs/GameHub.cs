using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Game;
using Shared.Interfaces;

namespace Server.Hubs {
    public class GameHub : Hub<IGameClient>, IGameHub {
        private readonly Controller controller;

        public GameHub(Controller controller) {
            this.controller = controller;
        }

        public override Task OnConnectedAsync() {
            controller.Connected();

            return base.OnConnectedAsync();
        }

        public string CreateGame(string name) {
            return "1234";
        }

        public void JoinGame(string name, string code) {
        }

        public void LeaveGame(string code) {
        }
    }
}
