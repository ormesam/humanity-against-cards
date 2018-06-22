using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanityAgainstCards.Entities
{
    public class Player
    {
        public readonly string ConnectionId;
        public readonly string Name;
        public int Points { get; private set; }
        public IList<Card> Hand { get; private set; }

        public Player(string connectionId, string name)
        {
            ConnectionId = connectionId;
            Name = name;
            Hand = new List<Card>();
            Points = 0;
        }

        public void PopulateHand(IList<Card> hand)
        {
            Hand = hand;
        }

        public void AddToHand(Card card)
        {
            Hand.Add(card);
        }
    }
}