using Blazor.Extensions;
using HumanityAgainstCards.Shared;
using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Client.Pages
{
    public class GameComponent : BlazorComponent, IClient
    {
        private HubConnection connection;
        public string RoomCode;
        public string Name;
        public IList<string> messages = new List<string>();

        protected override async Task OnInitAsync()
        {
            connection = new HubConnectionBuilder().WithUrl("/gamehub").Build();
            connection.On<string>(nameof(IClient.PlayerJoined), this.PlayerJoined);
            await connection.StartAsync();
        }

        public Task PlayerJoined(string name)
        {
            messages.Add(name);
            StateHasChanged();
            return Task.CompletedTask;
        }

        public async Task CreateGame()
        {
            RoomCode = await connection.InvokeAsync<string>(nameof(IGameHub.Create), Name);
        }

        public async Task JoinGame()
        {
            await connection.InvokeAsync(nameof(IGameHub.Join), RoomCode, Name);
        }
    }
}
