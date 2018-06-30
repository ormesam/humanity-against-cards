using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Entities
{
    public sealed class GameController
    {
        private static GameController instance;
        private static readonly object _lock = new object();
        private Random random;

        public IDictionary<string, Game> Games { get; private set; }

        private GameController()
        {
            Games = new Dictionary<string, Game>();
            random = new Random();
        }

        public static GameController Instance {
            get {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new GameController();
                    }

                    return instance;
                }
            }
        }

        #region Join / Create

        public string CreateGame()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string roomCode = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)])
              .ToArray());

            // Don't want to create a group with a duplicate room code
            if (Games.ContainsKey(roomCode))
            {
                return CreateGame();
            }

            Game game = new Game(roomCode);
            Games.Add(roomCode, game);

            return roomCode;
        }

        public GameStatus JoinGame(string connectionId, string roomCode, string name)
        {
            if (Games.ContainsKey(roomCode))
            {
                return Games[roomCode].AddPlayer(connectionId, name);
            }
            else
            {
                throw new Exception("Unable to find group...");
            }
        }

        public bool DoesGameExist(string roomCode)
        {
            return Games.ContainsKey(roomCode);
        }

        #endregion

        #region Leave

        public void LeaveGame(string connectionId)
        {
            Game game = Games
                .Where(row => row.Value.ContainsPlayer(connectionId))
                .Select(row => row.Value)
                .SingleOrDefault();

            if (game == null)
            {
                return;
            }

            game.RemovePlayer(connectionId);

            if (game.Status == GameStatus.Stopped)
            {
                Games.Remove(game.RoomCode);
            }
        }

        #endregion

        public void StartGame(string roomCode)
        {
            // waiting for this would take a loooooong time
            Task.Run(() => Games[roomCode].Start());
        }

        public void SubmitCard(string roomCode, string connectionId, Guid card)
        {
            Games[roomCode].SubmitCard(connectionId, card);
        }

        public void SubmitVote(string roomCode, Guid cardId)
        {
            Games[roomCode].SubmitVote(cardId);
        }
    }
}