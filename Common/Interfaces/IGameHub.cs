using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Dtos;

namespace Common.Interfaces {
    public interface IGameHub {
        Task<string> CreateGame(string name);
        Task<GameState> JoinGame(string name, string code);
        Task Vote(string code, Guid submittedCardId);
        Task SubmitCards(string code, IList<Guid> answerCardIds);
        void StartGame(string code);
    }
}
