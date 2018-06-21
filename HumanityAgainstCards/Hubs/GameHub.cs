using HumanityAgainstCards.Entities;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Hubs
{
    public class GameHub : Hub<IGameHub>
    {
        public async Task JoinGroup(string roomCode)
        {
            Controller.Instance.JoinGroup(Context.ConnectionId, roomCode);

            await Groups.Add(Context.ConnectionId, roomCode);
        }

        public async Task CreateGroup()
        {
            string roomCode = Controller.Instance.CreateGroup(Context.ConnectionId);

            await Groups.Add(Context.ConnectionId, roomCode);
        }
    }
}