using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanityAgainstCards.Entities
{
    public class VotingCard : Card
    {
        public IList<string> Values { get; set; }
        public int Votes { get; set; }
        public override string Value => string.Join(Environment.NewLine, Values);
        public string PlayerId { get; set; }
    }
}