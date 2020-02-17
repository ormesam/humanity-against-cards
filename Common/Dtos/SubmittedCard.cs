using System.Collections.Generic;

namespace Common.Dtos {
    public class SubmittedCard : Card {
        public string PlayerId { get; set; }
        public IList<AnswerCard> AnswerCards { get; set; }
        public int Votes { get; set; }
    }
}
