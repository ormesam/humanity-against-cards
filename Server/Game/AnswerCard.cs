namespace Server.Game {
    public class AnswerCard : Card {
        public string Text { get; set; }

        public AnswerCard(string text) {
            Text = text;
        }
    }
}