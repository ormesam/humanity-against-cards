using System.Threading.Tasks;

namespace HumanityAgainstCards.Hubs
{
    public interface IGameHub
    {
        Task JoinGame(string roomCode);
        Task<string> CreateGame();
    }
}