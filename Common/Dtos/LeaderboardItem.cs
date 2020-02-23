namespace Common.Dtos {
    public class LeaderboardItem {
        public string PlayerName { get; set; }
        public int Score { get; set; }

        public LeaderboardItem(string playerName, int score) {
            PlayerName = playerName;
            Score = score;
        }

    }
}
