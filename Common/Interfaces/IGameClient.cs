using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Dtos;

namespace Common.Interfaces {
    public interface IGameClient {
        Task PlayerJoined(string name);
        Task DealCard(AnswerCard card);
        Task ShowQuestion(QuestionCard question);
        Task ShowAnswers(IList<AnswerCard> submittedAnswers);
        Task UpdateTimer(int seconds);
        Task ShowWinningCard(AnswerCard topCard);
    }
}
