using System.Collections.Generic;

namespace HumanityAgainstCards.Shared.Entities
{
    public class AnswerCardGroup
    {
        public Player Player { get; set; }
        public IList<AnswerCard> AnswerCards { get; set; }
        public int Votes { get; set; }

        public AnswerCardGroup()
        {
        }

        public AnswerCardGroup(Player player)
        {
            Player = player;
            Votes = 0;
        }
    }
}
