using HumanityAgainstCards.Entities;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Hubs
{
    public class GameHub : Hub<IClient>
    {
        public async Task<bool> JoinGame(string roomCode, string name)
        {
            // add to hub groups before joining the game
            await Groups.Add(Context.ConnectionId, roomCode);

            GameStatus status = GameController.Instance.JoinGame(Context.ConnectionId, roomCode, name);

            // return bool indicating if the game is running or not
            return status == GameStatus.Running;
        }

        public void Start(string roomCode)
        {
            GameController.Instance.StartGame(roomCode);
        }

        public void SubmitCard(string roomCode, Guid cardId)
        {
            GameController.Instance.SubmitCard(roomCode, Context.ConnectionId, cardId);
        }

        public void SubmitVote(string roomCode, Guid cardId)
        {
            GameController.Instance.SubmitVote(roomCode, cardId);
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            GameController.Instance.LeaveGame(Context.ConnectionId);

            await base.OnDisconnected(stopCalled);
        }
    }
}