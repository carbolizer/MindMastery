using UnityEngine;
using System.Collections.Generic;

public struct Card {
    public Card(string name = "", int value = 0, bool hidden = false) { this.name = name; this.value = value; this.hidden = hidden; }
    public string name;
    public int value;
    public bool hidden;
}

public class Deck {
// public
    public Deck() {
        for (int i = 0; i < 4; ++i) {
            string suit = "";
            
            switch (i) {
                case 0: suit = "-C"; break;
                case 1: suit = "-D"; break;
                case 2: suit = "-H"; break;
                case 3: suit = "-S"; break;
            }

            // Number cards
            for (int j = 2; j <= 10; ++j)
                cards.Add(new Card(j.ToString() + suit, j));

            // Face cards
            cards.Add(new Card("J" + suit, 10));
            cards.Add(new Card("K" + suit, 10));
            cards.Add(new Card("Q" + suit, 10));
            cards.Add(new Card("A" + suit, 11));
        }

        Shuffle();
    }

    public void Shuffle() {
        int n = cards.Count;
        while (n > 1) {
            n--; int k = Random.Range(0, n);
            (cards[n], cards[k]) = (cards[k], cards[n]);
        }
    }

    public Card PullTopCard() {
        if (cards.Count <= 0) { 
            cards.AddRange(pulled);
            pulled.Clear();
            Shuffle();
        }

        Card topCard = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        pulled.Add(topCard);
        return topCard;
    }

// private
    private List<Card> cards = new();
    private List<Card> pulled = new();
}