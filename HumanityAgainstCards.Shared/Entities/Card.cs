using System;

namespace HumanityAgainstCards.Shared.Entities
{
    public abstract class Card
    {
        public Guid Id { get; set; }
        public string Text { get; set; }

        public Card()
        {
        }

        public Card(string text)
        {
            Id = Guid.NewGuid();
            Text = text;
        }
    }
}
