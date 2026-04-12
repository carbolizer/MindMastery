using UnityEngine;

[System.Serializable]
public class PokerCard
{
    public enum Suit { Clubs, Diamonds, Hearts, Spades }
    public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public Rank rank;
    public Suit suit;

    public PokerCard(Rank r, Suit s)
    {
        rank = r;
        suit = s;
    }

    // Returns names like "2-S", "10-H", or "A-C" to match your file names
    public string GetSpriteName()
    {
        string r = ((int)rank <= 10) ? ((int)rank).ToString() : rank.ToString()[0].ToString();
        string s = suit.ToString()[0].ToString();
        return $"{r}-{s}";
    }
}