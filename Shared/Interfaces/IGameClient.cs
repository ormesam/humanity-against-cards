using System.Threading.Tasks;

namespace Shared.Interfaces {
    public interface IGameClient {
        Task PlayerJoined(string name);
    }
}
