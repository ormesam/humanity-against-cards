using System;

namespace Common.Dtos {
    public class Card {
        public Guid Id { get; set; }

        public Card() {
            Id = Guid.NewGuid();
        }
    }
}