using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanityAgainstCards.Entities
{
    public class AnswerCard : Card
    {
        public string PlayerId { get; set; }
        public bool IsAvailable => string.IsNullOrWhiteSpace(PlayerId);
    }
}