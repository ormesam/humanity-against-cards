using System.Threading.Tasks;

namespace HumanityAgainstCards.Shared
{
    public interface IGameHub
    {
        Task<string> Create(string name);
        Task<bool> Join(string roomCode, string name);
        Task Start(string roomCode);
    }
}
