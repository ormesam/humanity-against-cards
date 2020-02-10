using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Shared.Interfaces;

namespace Server.Game {
    public class Controller {
        private readonly IHubContext<GameHub, IGameClient> gameHub;
        private readonly IDictionary<string, Session> sessions;
        private readonly Random random;

        public Controller(IHubContext<GameHub, IGameClient> gameHub) {
            this.gameHub = gameHub;
            sessions = new Dictionary<string, Session>();
            random = new Random();
        }

        public string CreateSession() {
            Session session = new Session(GenerateCode());

            sessions.Add(session.Code, session);

            return session.Code;
        }

        private string GenerateCode() {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string roomCode = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)])
              .ToArray());

            // Don't want to create a group with a duplicate room code
            if (sessions.ContainsKey(roomCode)) {
                return GenerateCode();
            }

            return roomCode;
        }

        public async Task<bool> JoinSession(string connectionId, string name, string code) {
            if (!sessions.ContainsKey(code)) {
                return false;
            }

            await sessions[code].Join(gameHub, connectionId, name);

            return true;
        }
    }
}
