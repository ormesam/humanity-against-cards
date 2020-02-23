namespace Common.Dtos {
    public class AnswerCard : Card {
        public string Text { get; set; }
        public bool IsSelected { get; set; }
        public int Votes { get; set; }

        public AnswerCard(string text) : base() {
            Text = text;
            Votes = 0;
        }
    }
}