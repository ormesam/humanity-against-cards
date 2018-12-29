namespace HumanityAgainstCards.Shared.Entities
{
    public class AnswerCard : Card
    {
        public bool IsSubmitted { get; set; }

        public AnswerCard() : base()
        {
        }

        public AnswerCard(string text) : base(text)
        {
        }
    }
}
