namespace Client.Events {
    public delegate void PlayerJoinedEventHandler(PlayerJoinedEventArgs e);

    public class PlayerJoinedEventArgs {
        public string Name { get; set; }

        public PlayerJoinedEventArgs(string name) {
            Name = name;
        }
    }
}
