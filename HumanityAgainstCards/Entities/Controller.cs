using System;
using System.Collections.Generic;
using System.Linq;

namespace HumanityAgainstCards.Entities
{
    public sealed class Controller
    {
        private static Controller instance;
        private static readonly object _lock;

        private IDictionary<string, Game> games;
        private Random random;

        private Controller()
        {
            games = new Dictionary<string, Game>();
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

        public string CreateGroup(string host)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string roomCode = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)])
              .ToArray());

            // Don't want to create a group with a duplicate room code
            if (games.ContainsKey(roomCode))
            {
                return CreateGroup(host);
            }

            games.Add(roomCode, new Game(host));

            return roomCode;
        }

        public void JoinGroup(string connectionId, string roomCode)
        {
            if (games.ContainsKey(roomCode))
            {
                // add connectionId to the game's players
            }
            else
            {
                throw new Exception("Unable to find group...");
            }
        }

        #endregion
    }
}