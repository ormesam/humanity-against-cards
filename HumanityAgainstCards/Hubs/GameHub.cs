using HumanityAgainstCards.Entities;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Hubs
{
    public class GameHub : Hub<IClient>
    {
        public async Task JoinGame(string roomCode, string name)
        {
            if (Controller.Instance.Games.ContainsKey(roomCode))
            {
                await Groups.Add(Context.ConnectionId, roomCode);

                Controller.Instance.JoinGroup(Context.ConnectionId, roomCode, name);
            }
        }

        public async Task CreateGame(string hostName)
        {
            string roomCode = Controller.Instance.CreateGroup(Context.ConnectionId, hostName);

            await Groups.Add(Context.ConnectionId, roomCode);

            await JoinGame(roomCode, hostName);

            Clients.Group(roomCode).RoomCodeChanged(roomCode);
        }
    }
}