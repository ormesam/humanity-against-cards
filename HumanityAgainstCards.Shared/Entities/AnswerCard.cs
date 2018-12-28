namespace HumanityAgainstCards.Shared.Entities
{
    public class AnswerCard : Card
    {
        public bool Submitted { get; set; }

        public AnswerCard() : base()
        {
        }

        public AnswerCard(string text) : base(text)
        {
        }
    }
}
