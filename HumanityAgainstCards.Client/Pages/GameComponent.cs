using Blazor.Extensions;
using HumanityAgainstCards.Shared;
using Microsoft.AspNetCore.Blazor.Components;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Client.Pages
{
    public class GameComponent : BlazorComponent, IClient
    {
        private HubConnection connection;

        [Parameter]
        internal string RoomCode { get; set; }
        public bool IsNewGame => string.IsNullOrWhiteSpace(RoomCode);
        public string Name { get; set; }

        protected override async Task OnInitAsync()
        {
            connection = new HubConnectionBuilder().WithUrl("/gamehub").Build();
            // connection.On<string>(nameof(IClient.PlayerJoined), this.PlayerJoined);
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
            }
            else
            {
                await JoinGame();
            }
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
    }
}
