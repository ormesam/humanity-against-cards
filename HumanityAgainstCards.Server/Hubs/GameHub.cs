using HumanityAgainstCards.Server.Utility;
using HumanityAgainstCards.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Server.Hubs
{
    public class GameHub : Hub<IClient>, IGameHub
    {
        public GameController gameController;

        public GameHub()
        {
            gameController = new GameController(this);
        }

        public async Task<string> Create(string name)
        {
            string roomCode = gameController.CreateGame();

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            await Join(roomCode, name);

            return roomCode;
        }

        public async Task<bool> Join(string roomCode, string name)
        {
            bool joined = await gameController.JoinGame(Context.ConnectionId, roomCode, name);

            if (joined)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            }

            return joined;
        }
    }
}
