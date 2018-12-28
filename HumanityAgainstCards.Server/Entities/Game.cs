using HumanityAgainstCards.Server.Hubs;
using HumanityAgainstCards.Server.Utility;
using HumanityAgainstCards.Shared;
using HumanityAgainstCards.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Server.Entities
{
    public class Game
    {
        private readonly int maxCardsInHand = 5; // change to 10 later
        private readonly string roomCode;
        private GameStatus status;
        private IClient hubContext;

        private readonly IList<Player> Players;
        private QuestionCard selectedQuestion;
        private IList<QuestionCard> questionDeck;
        private IList<AnswerCard> answerDeck;
        private CardGenerator cardGenerator;

        public Game(GameHub hub, string roomCode)
        {
            this.roomCode = roomCode;
            status = GameStatus.Pending;
            hubContext = hub.Clients.Group(roomCode);

            Players = new List<Player>();
            cardGenerator = new CardGenerator();
            questionDeck = cardGenerator.GenerateQuestions();
            answerDeck = cardGenerator.GenerateAnswers();
        }

        public async Task Start()
        {
            if (status != GameStatus.Pending)
            {
                return;
            }

            status = GameStatus.Running;

            while (questionDeck.Count > 0 && answerDeck.Count > 0)
            {
                DealCards();
                await PickAndShowSelectedQuestion();
                await ShowHands();
                await SetTimer(30); // set timer and wait for cards to be picked

                bool hasSubmittedAnswers = selectedQuestion.SubmittedAnswers.Any();

                if (!hasSubmittedAnswers)
                {
                    continue;
                }

                await ShowAnswers();
                await SetTimer(30); // set timer and wait for votes to be cast
                await PickAndShowWinner();
                await SetTimer(8);
            }

            status = GameStatus.Complete;
        }

        private async Task PickAndShowWinner()
        {
            int maxVotes = selectedQuestion.SubmittedAnswers
                .Max(i => i.Votes);

            var winningCards = selectedQuestion.SubmittedAnswers
                .Where(i => i.Votes == maxVotes)
                .ToList();

            winningCards.Shuffle();

            var winningCard = winningCards.First();
            winningCard.Player.Points++;

            await hubContext.ShowWinningCard(winningCard);
            await UpdateScoreboard();
        }

        private async Task UpdateScoreboard()
        {
            await hubContext.UpdateScoreboard(Players);
        }

        private async Task ShowAnswers()
        {
            await hubContext.ShowAnswers(selectedQuestion.SubmittedAnswers);
        }

        private async Task PickAndShowSelectedQuestion()
        {
            selectedQuestion = questionDeck[0];
            questionDeck.Remove(selectedQuestion);

            await hubContext.ShowQuestion(selectedQuestion);
        }

        private async Task SetTimer(int seconds)
        {
            int milliseconds = seconds * 1000;

            await hubContext.SetTimer(seconds);

            await Task.Delay(milliseconds);
        }

        private async Task ShowHands()
        {
            foreach (var player in Players)
            {
                await player.ShowHand();
            }
        }

        private void DealCards()
        {
            foreach (var player in Players)
            {
                DealCards(player);
            }
        }

        private void DealCards(Player player)
        {
            var hand = answerDeck.Take(maxCardsInHand - player.Hand.Count);

            if (hand.Any())
            {
                player.Hand.AddRange(hand);
                answerDeck.RemoveRange(hand);
            }
        }

        public void Submit(string connectionId, Guid cardId)
        {
            Player player = Players
                .Single(i => i.ConnectionId == connectionId);

            AnswerCard card = player.Hand
                .Single(i => i.Id == cardId);

            selectedQuestion.SubmitAnswer(player, card);
        }

        public void Vote(string connectionId, Guid cardGroupId)
        {
            var cardGroup = selectedQuestion.SubmittedAnswers
                .Single(i => i.Id == cardGroupId);

            cardGroup.Votes++;
        }

        public async Task AddPlayer(Player player)
        {
            Players.Add(player);

            if (status == GameStatus.Running)
            {
                await player.WaitForNextRound();
            }

            await UpdateScoreboard();
        }

        public async Task RemovePlayer(string connectionId)
        {
            var playerToRemove = Players
                .Where(row => row.ConnectionId == connectionId)
                .SingleOrDefault();

            Players.Remove(playerToRemove);

            await UpdateScoreboard();
        }
    }
}
