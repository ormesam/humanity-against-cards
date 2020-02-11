using System.Threading.Tasks;
using Shared.Dtos;

namespace Shared.Interfaces {
    public interface IGameHub {
        Task<string> CreateGame(string name);
        Task<GameState> JoinGame(string name, string code);
        Task LeaveGame(string code);
    }
}
