using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ThreeCardGameManager : MonoBehaviour
{
    public enum GameState { Ante, Decision, Showdown }
    public GameState currentState;

    [Header("Game Settings")]
    public int bank = 1000;
    public int anteBet = 10;
    public int pairPlusBet = 0; // The side bet amount
    public int pairPlusIncrement = 10;

    [Header("UI Text References")]
    public TextMeshProUGUI bankText;
    public TextMeshProUGUI statusText;

    [Header("Visual References")]
    public CardVisual[] playerCardVisuals;
    public CardVisual[] dealerCardVisuals;

    private PokerDeck deck;
    private List<PokerCard> playerHand = new List<PokerCard>();
    private List<PokerCard> dealerHand = new List<PokerCard>();

    void Start() { StartGame(); }

    public void StartGame()
    {
        deck = new PokerDeck();
        playerHand.Clear();
        dealerHand.Clear();
        foreach (var card in playerCardVisuals) card.SetCard(null, false);
        foreach (var card in dealerCardVisuals) card.SetCard(null, false);
        currentState = GameState.Ante;
        UpdateUI("Place your Ante and Pair Plus bets.");
    }

    // New method for the Pair Plus button
    public void TogglePairPlus()
    {
        if (currentState != GameState.Ante) return;

        // Simple toggle: 0 or 10. You could also make this increment.
        if (pairPlusBet == 0) pairPlusBet = pairPlusIncrement;
        else pairPlusBet = 0;

        UpdateUI(pairPlusBet > 0 ? "Pair Plus Active: $" + pairPlusBet : "Pair Plus Disabled.");
    }

    public void Deal()
    {
        if (currentState != GameState.Ante) return;

        // Subtract both bets at once
        bank -= (anteBet + pairPlusBet);

        playerHand.Clear();
        dealerHand.Clear();

        for (int i = 0; i < 3; i++)
        {
            playerHand.Add(deck.Draw());
            dealerHand.Add(deck.Draw());
            playerCardVisuals[i].SetCard(playerHand[i], true);
            dealerCardVisuals[i].SetCard(dealerHand[i], false);
        }
        currentState = GameState.Decision;
        UpdateUI("Play ($" + anteBet + ") or Fold?");
    }

    public void Play()
    {
        if (currentState != GameState.Decision) return;
        bank -= anteBet;
        currentState = GameState.Showdown;
        Resolve();
    }

    public void Fold()
    {
        if (currentState != GameState.Decision) return;
        // Pair Plus is also lost on a fold
        pairPlusBet = 0;
        StartGame();
    }

    private void Resolve()
    {
        for (int i = 0; i < 3; i++) dealerCardVisuals[i].SetCard(dealerHand[i], true);

        HandEvaluator pEval = new HandEvaluator(playerHand);
        HandEvaluator dEval = new HandEvaluator(dealerHand);

        string resultMessage = "";

        // 1. Resolve Pair Plus Bet (Independent of Dealer)
        if (pairPlusBet > 0)
        {
            int multiplier = GetPairPlusMultiplier(pEval.Rank);
            if (multiplier > 0)
            {
                int win = pairPlusBet * multiplier;
                bank += pairPlusBet + win; // Return original bet + winnings
                resultMessage += "Pair Plus Wins $" + win + "! ";
            }
            pairPlusBet = 0; // Reset for next round
        }

        // 2. Resolve Main Game
        if (!dEval.DealerQualifies())
        {
            bank += (anteBet * 2) + anteBet;
            resultMessage += "Dealer doesn't qualify. You win Ante!";
        }
        else
        {
            int result = CompareHands(pEval, dEval);
            if (result > 0)
            {
                bank += (anteBet * 2) + (anteBet * 2);
                resultMessage += "You Win Main Hand!";
            }
            else if (result < 0) resultMessage += "Dealer Wins Main Hand.";
            else { bank += anteBet + anteBet; resultMessage += "Main Hand Push."; }
        }

        UpdateUI(resultMessage);
        currentState = GameState.Ante;
    }

    private int GetPairPlusMultiplier(PokerHandRank rank)
    {
        switch (rank)
        {
            case PokerHandRank.StraightFlush: return 40;
            case PokerHandRank.ThreeOfAKind: return 30;
            case PokerHandRank.Straight: return 6;
            case PokerHandRank.Flush: return 3;
            case PokerHandRank.Pair: return 1;
            default: return 0;
        }
    }

    private int CompareHands(HandEvaluator p, HandEvaluator d)
    {
        if (p.Rank != d.Rank) return p.Rank.CompareTo(d.Rank);
        return p.HighCardValue.CompareTo(d.HighCardValue);
    }

    void UpdateUI(string message)
    {
        if (bankText != null) bankText.text = "Bank: $" + bank;
        if (statusText != null) statusText.text = message;
    }
}