using HumanityAgainstCards.Server.Entities;
using HumanityAgainstCards.Server.Hubs;
using HumanityAgainstCards.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Server.Utility
{
    public sealed class GameController
    {
        private static GameController instance = null;
        private static readonly object padlock = new object();

        public static GameController Instance {
            get {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameController();
                    }

                    return instance;
                }
            }
        }

        private readonly Random random;
        private GameHub hub;
        public readonly IDictionary<string, Game> Games;

        public GameController()
        {
            Games = new Dictionary<string, Game>();
            random = new Random();
        }

        public void SetHub(GameHub hub) {
            this.hub = hub;
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
                await Games[roomCode].AddPlayer(new Player(hub.Clients.Client(connectionId), connectionId, name));

                return true;
            }

            return false;
        }

        public void Start(string roomCode)
        {
            Task.Run(() => Games[roomCode].Start());
        }

        public void Submit(string roomCode, string connectionId, Guid cardId)
        {
            Games[roomCode].Submit(connectionId, cardId);
        }

        public void Vote(string roomCode, string connectionId, Guid cardId)
        {
            Games[roomCode].Vote(connectionId, cardId);
        }
    }
}
