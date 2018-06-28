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
            roomCode = roomCode.ToUpper().Trim();

            if (Controller.Instance.Games.ContainsKey(roomCode))
            {
                // add to hub groups before joining the game
                await Groups.Add(Context.ConnectionId, roomCode);

                GameStatus status = Controller.Instance.JoinGame(Context.ConnectionId, roomCode, name);

                // return bool indicating if the game is running or not
                return status == GameStatus.Running;
            }

            return false;
        }

        public async Task CreateGame(string hostName)
        {
            // create game, add the player to the group, then the game
            string roomCode = Controller.Instance.CreateGame();

            await Groups.Add(Context.ConnectionId, roomCode);

            await JoinGame(roomCode, hostName);
        }

        public void Start(string roomCode)
        {
            roomCode = roomCode.ToUpper().Trim();

            Controller.Instance.StartGame(roomCode);
        }

        public void SubmitCard(string roomCode, Guid cardId)
        {
            roomCode = roomCode.ToUpper().Trim();

            Controller.Instance.SubmitCard(roomCode, Context.ConnectionId, cardId);
        }

        public void SubmitVote(string roomCode, Guid cardId)
        {
            roomCode = roomCode.ToUpper().Trim();

            Controller.Instance.SubmitVote(roomCode, cardId);
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            Controller.Instance.LeaveGame(Context.ConnectionId);

            await base.OnDisconnected(stopCalled);
        }
    }
}