using System;

namespace Shared.Dtos {
    public class Card {
        public Guid Id { get; }

        public Card() {
            Id = Guid.NewGuid();
        }
    }
}