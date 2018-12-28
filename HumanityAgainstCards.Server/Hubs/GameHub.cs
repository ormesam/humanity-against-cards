using HumanityAgainstCards.Server.Utility;
using HumanityAgainstCards.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Server.Hubs
{
    public class GameHub : Hub<IClient>, IGameHub
    {
        public GameHub() {
            // not ideal
            GameController.Instance.SetHub(this);
        }

        public async Task<string> Create(string name)
        {
            string roomCode = GameController.Instance.CreateGame();

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            await Join(roomCode, name);

            return roomCode;
        }

        public async Task<bool> Join(string roomCode, string name)
        {
            bool joined = await GameController.Instance.JoinGame(Context.ConnectionId, roomCode, name);

            if (joined)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            }

            return joined;
        }

        public Task Start(string roomCode)
        {
            GameController.Instance.Start(roomCode);

            return Task.CompletedTask;
        }

        public Task Submit(string roomCode, Guid cardId)
        {
            GameController.Instance.Submit(roomCode, Context.ConnectionId, cardId);

            return Task.CompletedTask;
        }

        public Task Vote(string roomCode, Guid cardId)
        {
            GameController.Instance.Vote(roomCode, Context.ConnectionId, cardId);

            return Task.CompletedTask;
        }
    }
}
