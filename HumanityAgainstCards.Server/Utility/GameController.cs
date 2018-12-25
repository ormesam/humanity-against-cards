using HumanityAgainstCards.Server.Entities;
using HumanityAgainstCards.Server.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Server.Utility
{
    public sealed class GameController
    {
        private readonly Random random;
        private readonly GameHub hub;
        public readonly IDictionary<string, Game> Games;

        public GameController(GameHub hub)
        {
            this.hub = hub;
            Games = new Dictionary<string, Game>();
            random = new Random();
        }

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

            Game game = new Game(hub, roomCode);
            Games.Add(roomCode, game);

            return roomCode;
        }

        public bool DoesGameExist(string roomCode)
        {
            return Games.ContainsKey(roomCode);
        }

        public async Task<bool> JoinGame(string connectionId, string roomCode, string name)
        {
            if (DoesGameExist(roomCode))
            {
                await Games[roomCode].AddPlayer(connectionId, name);

                return true;
            }

            return false;
        }
    }
}
