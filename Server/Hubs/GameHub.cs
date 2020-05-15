using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Server.Game;

namespace Server.Hubs {
    public class GameHub : Hub<IGameClient>, IGameHub {
        private readonly Controller controller;

        public GameHub(Controller controller) {
            this.controller = controller;
        }

        public async Task<string> CreateGame(string name) {
            string code = controller.CreateSession();
            await controller.JoinSession(Context.ConnectionId, name, code);
            await Groups.AddToGroupAsync(Context.ConnectionId, code);

            return code;
        }

        public async Task<bool> JoinGame(string name, string code) {
            code = code.ToUpperInvariant().Trim();

            await controller.JoinSession(Context.ConnectionId, name, code);

            await Groups.AddToGroupAsync(Context.ConnectionId, code);

            return true;
        }

        public void StartGame(string code) {
            controller.StartGame(code);
        }

        public async Task SubmitCards(string code, IList<Guid> answerCardIds) {
            await controller.SubmitCards(code, Context.ConnectionId, answerCardIds);
        }

        public async Task Vote(string code, Guid submittedCardId) {
            await controller.Vote(code, Context.ConnectionId, submittedCardId);
        }
    }
}
