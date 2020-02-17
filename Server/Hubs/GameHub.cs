using System;
using System.Threading.Tasks;
using Common.Dtos;
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

        public async Task<GameState> JoinGame(string name, string code) {
            GameState gameState = await controller.JoinSession(Context.ConnectionId, name, code);

            await Groups.AddToGroupAsync(Context.ConnectionId, code);

            return gameState;
        }

        public async Task StartGame(string code) {
            // Start game using something like hangfire?
        }

        public async Task SubmitCard(string code, Guid answerCardId) {
            await controller.SubmitCard(code, Context.ConnectionId, answerCardId);
        }

        public async Task Vote(string code, Guid submittedCardId) {
            await controller.Vote(code, Context.ConnectionId, submittedCardId);
        }
    }
}
