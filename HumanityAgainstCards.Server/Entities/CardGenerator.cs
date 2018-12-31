using HumanityAgainstCards.Server.Utility;
using HumanityAgainstCards.Shared.Entities;
using System.Collections.Generic;

namespace HumanityAgainstCards.Server.Entities
{
    public class CardGenerator
    {
        private readonly IList<QuestionCard> questionCards = new List<QuestionCard>()
        {
            new QuestionCard("One answer _", 1),
            new QuestionCard("Two answers _ _", 2),
            new QuestionCard("Three answers _ _ _", 3),
    };

        private readonly IList<AnswerCard> answerCards = new List<AnswerCard>()
        {
            new AnswerCard("1"),
            new AnswerCard("2"),
            new AnswerCard("3"),
            new AnswerCard("4"),
            new AnswerCard("5"),
            new AnswerCard("6"),
            new AnswerCard("7"),
            new AnswerCard("8"),
            new AnswerCard("9"),
            new AnswerCard("10"),
            new AnswerCard("11"),
            new AnswerCard("12"),
            new AnswerCard("13"),
            new AnswerCard("14"),
            new AnswerCard("15"),
            new AnswerCard("16"),
            new AnswerCard("17"),
            new AnswerCard("18"),
            new AnswerCard("19"),
            new AnswerCard("20"),
            new AnswerCard("21"),
            new AnswerCard("22"),
            new AnswerCard("23"),
            new AnswerCard("24"),
            new AnswerCard("25"),
            new AnswerCard("26"),
            new AnswerCard("27"),
            new AnswerCard("28"),
            new AnswerCard("29"),
            new AnswerCard("30"),
            new AnswerCard("31"),
            new AnswerCard("32"),
            new AnswerCard("33"),
            new AnswerCard("34"),
            new AnswerCard("35"),
            new AnswerCard("36"),
            new AnswerCard("37"),
            new AnswerCard("38"),
            new AnswerCard("39"),
            new AnswerCard("40"),
            new AnswerCard("41"),
            new AnswerCard("42"),
            new AnswerCard("43"),
            new AnswerCard("44"),
            new AnswerCard("45"),
            new AnswerCard("46"),
            new AnswerCard("47"),
            new AnswerCard("48"),
            new AnswerCard("49"),
            new AnswerCard("50"),
        };

        public IList<QuestionCard> GenerateQuestions()
        {
            questionCards.Shuffle();
            return questionCards;
        }

        public IList<AnswerCard> GenerateAnswers()
        {
            answerCards.Shuffle();
            return answerCards;
        }
    }
}
