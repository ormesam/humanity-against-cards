using System.Threading.Tasks;

namespace Shared.Interfaces {
    public interface IGameHubClient {
        Task Test(string message);
    }
}
