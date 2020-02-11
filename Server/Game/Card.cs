using System;

namespace Server.Game {
    public class Card {
        public Guid Id { get; }

        public Card() {
            Id = Guid.NewGuid();
        }
    }
}