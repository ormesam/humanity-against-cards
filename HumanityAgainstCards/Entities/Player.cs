using HumanityAgainstCards.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanityAgainstCards.Entities
{
    public class Player
    {
        private IClient hub => GetPlayerHub();

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

        public void AddToHand(Card card)
        {
            Hand.Add(card);
        }

        public void ShowHand()
        {
            hub.ShowHand(Hand);
        }

        private IClient GetPlayerHub()
        {
            return GlobalHost.ConnectionManager.GetHubContext<GameHub, IClient>().Clients.Client(ConnectionId);
        }
    }
}