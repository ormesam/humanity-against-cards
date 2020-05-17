using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Server.Game;

namespace Server.Hubs {
    public class GameHub : Hub<IGameClient>, IGameHub {
        private readonly Controller controller;
        private readonly Random random;

        public GameHub(Controller controller) {
            this.controller = controller;
            random = new Random();
        }

        public async Task<string> CreateGame(string name) {
            string code = GenerateCode();
            string jobId = BackgroundJob.Enqueue(() => controller.CreateSession(Context.ConnectionId, name, code));

            await Groups.AddToGroupAsync(Context.ConnectionId, code);

            return code;
        }

        public async Task<bool> JoinGame(string name, string code) {
            code = code.ToUpperInvariant().Trim();

            bool joined = await controller.JoinSession(Context.ConnectionId, name, code);

            if (joined) {
                await Groups.AddToGroupAsync(Context.ConnectionId, code);
            }

            return joined;
        }

        public async Task StartGame(string code) {
            await controller.StartGame(code);
        }

        public async Task SubmitCards(string code, IList<Guid> answerCardIds) {
            await controller.SubmitCards(code, Context.ConnectionId, answerCardIds);
        }

        public async Task Vote(string code, Guid submittedCardId) {
            await controller.Vote(code, Context.ConnectionId, submittedCardId);
        }

        private string GenerateCode() {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string roomCode = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)])
              .ToArray());

            // Don't want to create a group with a duplicate room code
            if (controller.Sessions.ContainsKey(roomCode)) {
                return GenerateCode();
            }

            return roomCode;
        }
    }
}
