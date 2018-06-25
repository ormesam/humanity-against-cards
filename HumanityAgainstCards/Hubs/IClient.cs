using HumanityAgainstCards.Entities;
using System;
using System.Collections.Generic;

namespace HumanityAgainstCards.Hubs
{
    public interface IClient
    {
        void PlayerJoined(string name);
        void RoomCodeChanged(string roomCode);
        void NewQuestion(AnswerCard question);
        void ShowHand(IList<AnswerCard> hand);
        void ShowVotingCards(IList<VotingCard> votingCards);
        void ShowWinningCard(string name, Guid cardId, int voteCount);
        void UpdateLeaderboard(IList<Player> leaderboard);
        void SetTimer(int seconds);
    }
}