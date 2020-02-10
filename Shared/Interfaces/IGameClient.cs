using System.Threading.Tasks;

namespace Shared.Interfaces {
    public interface IGameClient {
        Task Test(string message);
    }
}
