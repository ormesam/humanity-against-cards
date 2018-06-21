using System.Threading.Tasks;

namespace HumanityAgainstCards.Hubs
{
    public interface IGameHub
    {
        Task JoinGroup(string roomCode);

        Task LeaveGroup(string roomCode);
    }
}