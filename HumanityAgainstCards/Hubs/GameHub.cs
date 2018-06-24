using HumanityAgainstCards.Entities;
using Microsoft.AspNet.SignalR;
using System;
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
        }

        public void Start(string roomCode)
        {
            Controller.Instance.StartGame(roomCode);
        }

        public void SubmitCard(string roomCode, Guid cardId)
        {
            Controller.Instance.SubmitCard(roomCode, Context.ConnectionId, cardId);
        }

        public void SubmitVote(string roomCode, Guid cardId)
        {
            Controller.Instance.SubmitVote(roomCode, cardId);
        }
    }
}