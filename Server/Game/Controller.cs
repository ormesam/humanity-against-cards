using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Dtos;
using Common.Exceptions;
using Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;

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
            Session session = new Session(gameHub, GenerateCode());

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

        public async Task<GameState> JoinSession(string connectionId, string name, string code) {
            if (!sessions.ContainsKey(code)) {
                throw new GameNotFoundException();
            }

            return await sessions[code].Join(connectionId, name);
        }

        public Task SubmitCard(string code, string connectionId, Guid answerCardId) {
            if (!sessions.ContainsKey(code)) {
                throw new GameNotFoundException();
            }

            sessions[code].SubmitCard(connectionId, answerCardId);

            return Task.CompletedTask;
        }

        public Task Vote(string code, string connectionId, Guid submittedCardId) {
            if (!sessions.ContainsKey(code)) {
                throw new GameNotFoundException();
            }

            sessions[code].Vote(connectionId, submittedCardId);

            return Task.CompletedTask;
        }
    }
}
