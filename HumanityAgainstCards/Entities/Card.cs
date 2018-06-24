using System;

namespace HumanityAgainstCards.Entities
{
    public abstract class Card
    {
        public Guid Id { get; set; }
        public virtual string Value { get; set; }
    }
}