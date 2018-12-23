using System.Threading.Tasks;

namespace HumanityAgainstCards.Shared
{
    public interface IClient
    {
        Task PlayerJoined(string name);
    }
}
