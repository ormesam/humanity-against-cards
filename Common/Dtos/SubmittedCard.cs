using System.Collections.Generic;

namespace Common.Dtos {
    public class SubmittedCard : Card {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public IList<AnswerCard> AnswerCards { get; set; }
        public int Votes { get; set; }
        public bool IsWinningCard { get; set; }
        public bool IsSelected { get; set; }

        public SubmittedCard() : base() { }
    }
}
