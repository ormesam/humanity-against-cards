using System.Collections.Generic;
using System.Linq;

namespace HumanityAgainstCards.Shared.Entities
{
    public class QuestionCard : Card
    {
        public int NumberOfAnswers { get; set; }
        public IList<AnswerCardGroup> SubmittedAnswers { get; set; }

        public QuestionCard() : base()
        {
        }

        public QuestionCard(string text, int numberOfAnswers) : base(text)
        {
            NumberOfAnswers = numberOfAnswers;
            SubmittedAnswers = new List<AnswerCardGroup>();
        }

        public void SubmitAnswer(Player player, AnswerCard card)
        {
            AnswerCardGroup answerGroup = SubmittedAnswers
                .SingleOrDefault(i => i.Player.ConnectionId == player.ConnectionId);

            if (answerGroup == null)
            {
                answerGroup = new AnswerCardGroup(player);
                SubmittedAnswers.Add(answerGroup);
            }

            answerGroup.AnswerCards.Add(card);
        }
    }
}
