namespace Common.Dtos {
    public class QuestionCard : Card {
        public string Text { get; set; }
        public int NoOfAnswers { get; set; }

        public QuestionCard(string text, int noOfAnswers) : base() {
            Text = text;
            NoOfAnswers = noOfAnswers;
        }
    }
}
