using Blazor.Extensions;
using HumanityAgainstCards.Shared;
using HumanityAgainstCards.Shared.Entities;
using Microsoft.AspNetCore.Blazor.Components;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

namespace HumanityAgainstCards.Client.Pages
{
    public class GameComponent : BlazorComponent, IClient
    {
        private HubConnection connection;
        private Timer timer;

        [Parameter]
        internal string RoomCode { get; set; }
        public bool IsNewGame => string.IsNullOrWhiteSpace(RoomCode);
        public bool ShowNameEntry { get; set; }
        public bool ShowStart { get; set; }
        public string Name { get; set; }
        public int Timer = 0;
        public QuestionCard SelectedQuestion { get; set; }
        public IList<AnswerCard> Hand { get; set; }
        public IList<AnswerCardGroup> Answers { get; set; }
        public AnswerCardGroup WinningCard { get; set; }

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

            timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Timer <= 0)
            {
                timer.Stop();
                return;
            }

            Timer--;

            StateHasChanged();
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

        public async Task Start() {
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

        public Task ShowQuestion(QuestionCard question)
        {
            WinningCard = null;
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

            timer.Stop();
            timer.Start();

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
            Answers = null;
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
