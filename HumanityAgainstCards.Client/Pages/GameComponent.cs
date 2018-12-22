using Blazor.Extensions;
using HumanityAgainstCards.Shared;
using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Client.Pages
{
    public class GameComponent : BlazorComponent, IClient
    {
        private HubConnection connection;
        public string Message = "";
        public IList<string> messages = new List<string>();

        protected override async Task OnInitAsync()
        {
            connection = new HubConnectionBuilder().WithUrl("/gamehub").Build();
            connection.On<string, string>(nameof(IClient.BroadcastMessage), this.BroadcastMessage);
            await connection.StartAsync();
        }

        public Task BroadcastMessage(string name, string message)
        {
            messages.Add(name + " : " + message);
            StateHasChanged();
            return Task.CompletedTask;
        }

        public async Task SendMessage()
        {
            await connection.InvokeAsync(nameof(IGameHub.Send), Message);
            Message = "";
        }
    }
}
