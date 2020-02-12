using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Dtos;

namespace Shared.Interfaces {
    public interface IGameClient {
        Task PlayerJoined(string name);
        Task DealCard(AnswerCard card);
        Task ShowQuestion(QuestionCard question);
        Task ShowAnswers(IList<AnswerCard> submittedAnswers);
        Task UpdateTimer(int seconds);
        Task ShowWinningCard(AnswerCard topCard);
    }
}
