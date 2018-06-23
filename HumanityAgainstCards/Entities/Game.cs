using HumanityAgainstCards.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Entities
{
    public class Game
    {
        private string roomCode;
        private IDictionary<string, Player> players;
        private IList<Card> questionCards;
        private IList<Card> answerCards;
        private IClient groupHub => GetGroupHub();
        private GameStatus status;

        private Card currentQuestion;

        public Game(string roomCode)
        {
            players = new Dictionary<string, Player>();
            questionCards = new List<Card>();
            answerCards = new List<Card>();

            this.roomCode = roomCode;
            status = GameStatus.Created;
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

            players.Add(connectionId, player);

            groupHub.PlayerJoined(name);
        }

        public async Task Start()
        {
            if (status != GameStatus.Created)
            {
                return;
            }

            status = GameStatus.Running;

            PopulateCards();
            PopulateHands();

            while (questionCards.Any())
            {
                ShowNext();

                ShowHands();

                await Task.Delay(1000 * 10); // just leave at 10 seconds for now
            }
        }

        private void PopulateHands()
        {
            foreach (var player in players)
            {
                var hand = answerCards.Take(10).ToList();
                player.Value.PopulateHand(hand);

                foreach (var card in hand)
                {
                    answerCards.Remove(card);
                }
            }
        }

        private void ShowHands()
        {
            foreach (var player in players)
            {
                player.Value.ShowHand();
            }
        }

        private void ShowNext()
        {
            currentQuestion = questionCards.First();

            groupHub.NewQuestion(currentQuestion);

            questionCards.Remove(currentQuestion);
        }

        private IClient GetGroupHub()
        {
            return GlobalHost.ConnectionManager.GetHubContext<GameHub, IClient>().Clients.Group(roomCode);
        }
    }
}