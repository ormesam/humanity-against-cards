﻿using System.Collections.Generic;
using Shared.Dtos;

namespace Server.Game {
    public class Player {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public IList<AnswerCard> Hand { get; set; }
        public IList<QuestionCard> CardsWon { get; set; }

        public Player(string connectionId, string name) {
            ConnectionId = connectionId;
            Name = name;
            CardsWon = new List<QuestionCard>();
            Hand = new List<AnswerCard>();
        }
    }
}