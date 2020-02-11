using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Shared.Dtos;
using Shared.Interfaces;

namespace Server.Game {
    public class Session {
        public string Code { get; }
        public GameState GameState { get; private set; }
        public IList<Player> Players { get; set; }

        public Session(string code) {
            Code = code;
            GameState = GameState.NotStarted;
            Players = new List<Player>();
        }

        public async Task<GameState> Join(IHubContext<GameHub, IGameClient> gameHub, string connectionId, string name) {
            Players.Add(new Player(connectionId, name));

            await gameHub.Clients.Group(Code).PlayerJoined(name);

            return GameState;
        }
    }
}
