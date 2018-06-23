using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Entities
{
    public sealed class Controller
    {
        private static Controller instance;
        private static readonly object _lock = new object();
        private Random random;

        public IDictionary<string, Game> Games { get; private set; }

        private Controller()
        {
            Games = new Dictionary<string, Game>();
            random = new Random();
        }

        public static Controller Instance {
            get {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new Controller();
                    }

                    return instance;
                }
            }
        }

        #region Join / Create

        public string CreateGroup(string connectionId, string hostName)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string roomCode = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)])
              .ToArray());

            // Don't want to create a group with a duplicate room code
            if (Games.ContainsKey(roomCode))
            {
                return CreateGroup(connectionId, hostName);
            }

            Game game = new Game(roomCode);
            Games.Add(roomCode, game);

            return roomCode;
        }

        public void JoinGroup(string connectionId, string roomCode, string name)
        {
            if (Games.ContainsKey(roomCode))
            {
                Games[roomCode].AddPlayer(connectionId, name);
            }
            else
            {
                throw new Exception("Unable to find group...");
            }
        }

        public void StartGame(string roomCode)
        {
            Task.Run(() => Games[roomCode].Start());
        }

        public void SubmitCard(string roomCode, string connectionId, string card)
        {
            Games[roomCode].SubmitCard(connectionId, card);
        }

        #endregion
    }
}