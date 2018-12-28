using System.Collections.Generic;

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
    }
}
