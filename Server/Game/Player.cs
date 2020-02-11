using System.Collections.Generic;

namespace Server.Game {
    public class Player {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public int Points { get; set; }
        public IList<AnswerCard> Hand { get; set; }

        public Player(string connectionId, string name) {
            ConnectionId = connectionId;
            Name = name;
            Points = 0;
            Hand = new List<AnswerCard>();
        }
    }
}
