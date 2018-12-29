using Blazor.Extensions;
using HumanityAgainstCards.Shared;
using HumanityAgainstCards.Shared.Entities;
using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace HumanityAgainstCards.Client.Pages
{
    public class GameComponent : BlazorComponent, IClient
    {
        private HubConnection connection;

        [Parameter]
        internal string RoomCode { get; set; }
        public bool IsNewGame => string.IsNullOrWhiteSpace(RoomCode);
        public bool CanSubmit => SubmitCount < SelectedQuestion.NumberOfAnswers;
        public bool CanVote { get; set; }
        public bool ShowNameEntry { get; set; }
        public bool ShowStart { get; set; }
        public string Name { get; set; }
        public int Timer = 0;
        public int SubmitCount = 0;
        public QuestionCard SelectedQuestion { get; set; }
        public IList<AnswerCard> Hand { get; set; }
        public IList<AnswerCardGroup> Answers { get; set; }

        protected override async Task OnInitAsync()
        {
            ShowNameEntry = true;
            ShowStart = false;

            connection = new HubConnectionBuilder().WithUrl("/gamehub").Build();
            connection.On<QuestionCard>(nameof(IClient.ShowQuestion), this.ShowQuestion);
            connection.On<IList<AnswerCard>>(nameof(IClient.ShowHand), this.ShowHand);
            connection.On<int>(nameof(IClient.SetTimer), this.SetTimer);
            connection.On<IList<AnswerCardGroup>>(nameof(IClient.ShowAnswers), this.ShowAnswers);
            connection.On<AnswerCardGroup>(nameof(IClient.ShowWinningCard), this.ShowWinningCard);
            connection.On<IList<Player>>(nameof(IClient.UpdateScoreboard), this.UpdateScoreboard);
            // connection.On(nameof(IClient.WaitForNextRound), this.WaitForNextRound);
            await connection.StartAsync();
        }

        public Task PlayerJoined(string name)
        {
            StateHasChanged();
            return Task.CompletedTask;
        }

        public async Task CreateOrJoinGame()
        {
            if (IsNewGame)
            {
                await CreateGame();

                ShowStart = true;
            }
            else
            {
                await JoinGame();
            }

            ShowNameEntry = false;
        }

        public async Task Start()
        {
            await connection.InvokeAsync(nameof(IGameHub.Start), RoomCode);
            ShowStart = false;
        }

        public async Task CreateGame()
        {
            RoomCode = await connection.InvokeAsync<string>(nameof(IGameHub.Create), Name);
        }

        public async Task JoinGame()
        {
            bool joined = await connection.InvokeAsync<bool>(nameof(IGameHub.Join), RoomCode, Name);

            if (!joined)
            {
                // go to home and show error?
            }
        }

        public async Task Submit(AnswerCard card)
        {
            if (!CanSubmit || card.IsSubmitted)
            {
                return;
            }

            SubmitCount++;

            card.IsSubmitted = true;

            await connection.InvokeAsync(nameof(IGameHub.Submit), RoomCode, card.Id);
        }

        public async Task Vote(AnswerCardGroup card)
        {
            if (!CanVote)
            {
                return;
            }

            CanVote = false;

            card.IsVoted = true;

            await connection.InvokeAsync(nameof(IGameHub.Vote), RoomCode, card.Id);
        }

        public Task ShowQuestion(QuestionCard question)
        {
            Answers = null;
            CanVote = true;
            SubmitCount = 0;
            SelectedQuestion = question;
            StateHasChanged();
            return Task.CompletedTask;
        }

        public Task ShowHand(IList<AnswerCard> hand)
        {
            Hand = hand;
            StateHasChanged();
            return Task.CompletedTask;
        }

        public Task SetTimer(int seconds)
        {
            Timer = seconds;

            StateHasChanged();

            return Task.CompletedTask;
        }

        public Task ShowAnswers(IList<AnswerCardGroup> submittedAnswers)
        {
            Hand = null;
            Answers = submittedAnswers;
            StateHasChanged();
            return Task.CompletedTask;
        }

        public Task ShowWinningCard(AnswerCardGroup winningCard)
        {
            var winner = Answers.Single(i => i.Id == winningCard.Id);
            winner.IsWinner = true;
            StateHasChanged();
            return Task.CompletedTask;
        }

        public async Task UpdateScoreboard(IList<Player> players)
        {
        }

        public Task WaitForNextRound()
        {
            return Task.CompletedTask;
        }
    }
}
