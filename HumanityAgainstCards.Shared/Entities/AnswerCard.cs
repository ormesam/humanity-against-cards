using System.Diagnostics;

namespace HumanityAgainstCards.Shared.Entities
{
    [DebuggerDisplay("Text = {Text}")]
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
