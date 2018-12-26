using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HumanityAgainstCards.Shared.Entities
{
    public class Player
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public IList<AnswerCard> Hand { get; set; }

        private IClient hubContext;

        public Player(IClient playerHubContext, string connectionId, string name)
        {
            ConnectionId = connectionId;
            Name = name;
            Points = 0;
            Hand = new List<AnswerCard>();
            hubContext = playerHubContext;
        }

        public async Task ShowHand()
        {
            await hubContext.ShowHand(Hand);
        }

        public async Task WaitForNextRound()
        {
            await hubContext.WaitForNextRound();
        }
    }
}
