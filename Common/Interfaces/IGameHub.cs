using System;
using System.Threading.Tasks;
using Common.Dtos;

namespace Common.Interfaces {
    public interface IGameHub {
        Task<string> CreateGame(string name);
        Task<GameState> JoinGame(string name, string code);
        Task Vote(string code, Guid submittedCardId);
        Task SubmitCard(string code, Guid answerCardId);
        void StartGame(string code);
    }
}
