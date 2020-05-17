using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Interfaces {
    public interface IGameHub {
        Task<string> CreateGame(string name);
        Task<bool> JoinGame(string name, string code);
        Task Vote(string code, Guid submittedCardId);
        Task SubmitCards(string code, IList<Guid> answerCardIds);
        Task StartGame(string code);
    }
}
