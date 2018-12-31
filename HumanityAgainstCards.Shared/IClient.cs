using HumanityAgainstCards.Shared.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Shared
{
    public interface IClient
    {
        Task ShowHand(IList<AnswerCard> hand);
        Task SetTimer(int seconds);
        Task ShowQuestion(QuestionCard question);
        Task ShowAnswers(IList<AnswerCardGroup> submittedAnswers);
        Task ShowWinningCard(AnswerCardGroup answerCardGroup);
        Task UpdateScoreboard(IList<Player> players);
        Task WaitForNextRound();
    }
}
