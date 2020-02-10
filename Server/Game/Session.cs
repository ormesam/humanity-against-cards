using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Shared.Interfaces;

namespace Server.Game {
    public class Session {
        public string Code { get; }

        public Session(string code) {
            Code = code;
        }

        public async Task Join(IHubContext<GameHub, IGameClient> gameHub, string connectionId, string name) {
            await gameHub.Clients.Group(Code).PlayerJoined(name);
        }
    }
}
