using System;

namespace Common.Dtos {
    public class Card {
        public Guid Id { get; }

        public Card() {
            Id = Guid.NewGuid();
        }
    }
}