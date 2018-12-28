using System;
using System.Collections.Generic;

namespace HumanityAgainstCards.Shared.Entities
{
    public class AnswerCardGroup
    {
        public Guid Id { get; set; }
        public Player Player { get; set; }
        public IList<AnswerCard> AnswerCards { get; set; }
        public int Votes { get; set; }

        public AnswerCardGroup()
        {
        }

        public AnswerCardGroup(Player player)
        {
            Id = Guid.NewGuid();
            Player = player;
            Votes = 0;
            AnswerCards = new List<AnswerCard>();
        }
    }
}
