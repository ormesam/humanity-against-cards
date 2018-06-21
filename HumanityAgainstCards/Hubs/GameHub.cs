using HumanityAgainstCards.Entities;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Hubs
{
    public class GameHub : Hub<IGameHub>, IGameHub
    {
        public async Task JoinGame(string roomCode)
        {
            Controller.Instance.JoinGroup(Context.ConnectionId, roomCode);

            await Groups.Add(Context.ConnectionId, roomCode);
        }

        public async Task<string> CreateGame()
        {
            string roomCode = Controller.Instance.CreateGroup(Context.ConnectionId);

            await Groups.Add(Context.ConnectionId, roomCode);

            return roomCode;
        }
    }
}