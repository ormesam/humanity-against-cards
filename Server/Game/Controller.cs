using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;

namespace Server.Game {
    public class Controller {
        private readonly IHubContext<GameHub, IGameClient> gameHub;

        public IDictionary<string, Session> Sessions { get; }

        public Controller(IHubContext<GameHub, IGameClient> gameHub) {
            this.gameHub = gameHub;
            Sessions = new Dictionary<string, Session>();
        }

        public async Task CreateSession(string connectionId, string name, string code) {
            Session session = new Session(gameHub, code);

            Sessions.Add(session.Code, session);

            await JoinSession(connectionId, name, code);

            await session.Run();
        }

        public async Task<bool> JoinSession(string connectionId, string name, string code) {
            if (!Sessions.ContainsKey(code)) {
                return false;
            }

            return await Sessions[code].Join(connectionId, name);
        }

        public Task StartGame(string code) {
            var session = Sessions[code];

            return session.Start();
        }

        public Task SubmitCards(string code, string connectionId, IList<Guid> answerCardIds) {
            if (!Sessions.ContainsKey(code)) {
                throw new GameNotFoundException();
            }

            Sessions[code].SubmitCards(connectionId, answerCardIds);

            return Task.CompletedTask;
        }

        public Task Vote(string code, string connectionId, Guid submittedCardId) {
            if (!Sessions.ContainsKey(code)) {
                throw new GameNotFoundException();
            }

            Sessions[code].Vote(connectionId, submittedCardId);

            return Task.CompletedTask;
        }
    }
}
