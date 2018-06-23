using HumanityAgainstCards.Entities;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;

namespace HumanityAgainstCards.Hubs
{
    public interface IClient
    {
        void PlayerJoined(string name);
        void RoomCodeChanged(string roomCode);
        void NewQuestion(Card question);
        void ShowHand(IList<Card> hand);
        void ShowSelectedCards(IList<string> answers);
    }
}