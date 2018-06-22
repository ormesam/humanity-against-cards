using HumanityAgainstCards.Entities;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Hubs
{
    public class GameHub : Hub<IClient>
    {
        public async Task JoinGame(string roomCode, string name)
        {
            Controller.Instance.JoinGroup(Context.ConnectionId, roomCode, name);

            await Groups.Add(Context.ConnectionId, roomCode);

            Clients.User(Context.ConnectionId).RoomCodeChanged(roomCode);

            Clients.Group(roomCode).PlayerJoined(name);
        }

        public async Task CreateGame(string hostName)
        {
            string roomCode = Controller.Instance.CreateGroup(Context.ConnectionId, hostName);

            await Groups.Add(Context.ConnectionId, roomCode);

            Clients.Group(roomCode).RoomCodeChanged(roomCode);
        }
    }
}