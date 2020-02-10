namespace Shared.Interfaces {
    public interface IGameHub {
        string CreateGame(string name);
        void JoinGame(string name, string code);
        void LeaveGame(string code);
    }
}
