using System.Collections.Generic;
using System.Linq;

public enum PokerHandRank { HighCard, Pair, Flush, Straight, ThreeOfAKind, StraightFlush }

public class HandEvaluator
{
    public PokerHandRank Rank;
    public int HighCardValue;

    public HandEvaluator(List<PokerCard> hand)
    {
        var sorted = hand.OrderByDescending(c => (int)c.rank).ToList();

        bool isFlush = hand.All(c => c.suit == hand[0].suit);
        bool isStraight = IsStraight(sorted);

        if (isFlush && isStraight) Rank = PokerHandRank.StraightFlush;
        else if (sorted[0].rank == sorted[2].rank) Rank = PokerHandRank.ThreeOfAKind;
        else if (isStraight) Rank = PokerHandRank.Straight;
        else if (isFlush) Rank = PokerHandRank.Flush;
        else if (sorted[0].rank == sorted[1].rank || sorted[1].rank == sorted[2].rank) Rank = PokerHandRank.Pair;
        else Rank = PokerHandRank.HighCard;

        HighCardValue = (int)sorted[0].rank;
    }

    private bool IsStraight(List<PokerCard> sorted)
    {
        if (sorted[0].rank - sorted[1].rank == 1 && sorted[1].rank - sorted[2].rank == 1) return true;
        if (sorted[0].rank == PokerCard.Rank.Ace && sorted[1].rank == PokerCard.Rank.Three && sorted[2].rank == PokerCard.Rank.Two) return true;
        return false;
    }

    public bool DealerQualifies()
    {
        // Dealer needs Queen High (12) or better
        return Rank > PokerHandRank.HighCard || HighCardValue >= 12;
    }
}