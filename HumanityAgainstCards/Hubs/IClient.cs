using Microsoft.AspNet.SignalR;

namespace HumanityAgainstCards.Hubs
{
    public interface IClient
    {
        void PlayerJoined(string name);
        void RoomCodeChanged(string roomCode);
    }
}