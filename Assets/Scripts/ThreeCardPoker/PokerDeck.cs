using System.Collections.Generic;
using UnityEngine;

public class PokerDeck
{
    private List<PokerCard> cards = new List<PokerCard>();

    public PokerDeck()
    {
        foreach (PokerCard.Suit s in System.Enum.GetValues(typeof(PokerCard.Suit)))
        {
            foreach (PokerCard.Rank r in System.Enum.GetValues(typeof(PokerCard.Rank)))
            {
                cards.Add(new PokerCard(r, s));
            }
        }
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            PokerCard temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    public PokerCard Draw()
    {
        if (cards.Count == 0) return null;
        PokerCard c = cards[0];
        cards.RemoveAt(0);
        return c;
    }
}