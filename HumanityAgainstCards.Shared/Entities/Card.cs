using System;

namespace HumanityAgainstCards.Shared.Entities
{
    public abstract class Card
    {
        public Guid Id { get; set; }
        public string Text { get; set; }

        public Card(string text)
        {
            Text = text;
        }
    }
}
