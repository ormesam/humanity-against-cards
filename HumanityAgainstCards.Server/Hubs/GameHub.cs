using HumanityAgainstCards.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Server.Hubs
{
    public class GameHub : Hub<IClient>, IGameHub
    {
        public void Send(string message)
        {
            Clients.All.BroadcastMessage(Context.ConnectionId, message);
        }
    }
}
