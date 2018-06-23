using HumanityAgainstCards.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HumanityAgainstCards.Entities
{
    public class Game
    {
        private string roomCode;
        private IDictionary<string, Player> players;
        private IList<Card> questionCards;
        private IList<Card> answerCards;
        private IClient groupHub => GetGroupHub();

        public Game(string roomCode)
        {
            players = new Dictionary<string, Player>();
            questionCards = new List<Card>();
            answerCards = new List<Card>();

            this.roomCode = roomCode;

            PopulateCards();
        }

        private void PopulateCards()
        {
            CardGenerator generator = new CardGenerator();

            questionCards = generator.GenerateQuestions();
            answerCards = generator.GenerateAnswers();
        }

        public void AddPlayer(string connectionId, string name)
        {
            Player player = new Player(connectionId, name);

            var hand = answerCards.Take(10).ToList();
            player.PopulateHand(hand);

            foreach (var card in hand)
            {
                answerCards.Remove(card);
            }

            players.Add(connectionId, player);

            groupHub.PlayerJoined(name);
        }

        private IClient GetGroupHub()
        {
            return GlobalHost.ConnectionManager.GetHubContext<GameHub, IClient>().Clients.Group(roomCode);
        }
    }
}