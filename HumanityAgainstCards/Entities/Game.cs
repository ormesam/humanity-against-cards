using System;
using System.Collections.Generic;

namespace HumanityAgainstCards.Entities
{
    public class Game
    {
        private string roomCode;
        private IList<string> players;

        public Game(string roomCode)
        {
            players = new List<string>();

            this.roomCode = roomCode;
        }

        public void AddPlayer(string connectionId)
        {
            players.Add(connectionId);
        }
    }
}