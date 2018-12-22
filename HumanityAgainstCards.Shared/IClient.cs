using System.Threading.Tasks;

namespace HumanityAgainstCards.Shared
{
    public interface IClient
    {
        Task BroadcastMessage(string name, string message);
    }
}
