using System.Collections.Generic;
using System.Linq;
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
        public Queue<AnswerCard> AnswerPile { get; set; }

        public Session(string code) {
            Code = code;
            GameState = GameState.NotStarted;
            Players = new List<Player>();
            AnswerPile = new Queue<AnswerCard>();
        }

        public async Task Start() {
            while (Players.Any()) {
                // Deal Cards
                // Show Question
                // Wait 30 Seconds with check
                // Show Answers
                // Wait for users to pick answers (30 seconds with check)
                // Display results
            }
        }

        public async Task<GameState> Join(IHubContext<GameHub, IGameClient> gameHub, string connectionId, string name) {
            Players.Add(new Player(connectionId, name));

            await gameHub.Clients.Group(Code).PlayerJoined(name);

            return GameState;
        }
    }
}
