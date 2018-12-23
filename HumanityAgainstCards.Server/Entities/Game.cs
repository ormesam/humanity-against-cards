using System;
using System.Threading.Tasks;
using HumanityAgainstCards.Client;
using HumanityAgainstCards.Server.Hubs;
using HumanityAgainstCards.Shared;
using Microsoft.AspNetCore.SignalR;

namespace HumanityAgainstCards.Server.Entities
{
    public class Game
    {
        public readonly string roomCode;
        private IClient hubContext;

        public Game(GameHub hub, string roomCode)
        {
            this.roomCode = roomCode;
            hubContext = hub.Clients.Group(roomCode);
        }

        public async Task AddPlayer(string connectionId, string name)
        {
            await hubContext.PlayerJoined(name);
        }
    }
}
