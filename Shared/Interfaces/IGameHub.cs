using System.Threading.Tasks;

namespace Shared.Interfaces {
    public interface IGameHub {
        Task<string> CreateGame(string name);
        Task<bool> JoinGame(string name, string code);
        Task LeaveGame(string code);
    }
}
