using UnityEngine;
using System.Collections; // Required for Coroutines
using System.Collections.Generic; // Required for List<>
using TMPro; // Required for TextMeshPro

public class ThreeCardGameManager : MonoBehaviour
{
    public enum GameState { Ante, Decision, Showdown } //
    public GameState currentState; //

    [Header("Game Settings")]
    public int bank = 1000; //
    public int anteBet = 10; //
    public int pairPlusBet = 0; //
    public int pairPlusIncrement = 10; //
    public float cardDelay = 0.25f; //

    [Header("UI References")]
    public TextMeshProUGUI bankText; //
    public TextMeshProUGUI statusText; //
    public GameObject pairPlusButton; // Drag the Pair Plus Button GameObject here

    [Header("Visual References")]
    public CardVisual[] playerCardVisuals; //
    public CardVisual[] dealerCardVisuals; //

    private PokerDeck deck; //
    private List<PokerCard> playerHand = new List<PokerCard>(); //
    private List<PokerCard> dealerHand = new List<PokerCard>(); //

    void Start() { StartGame(); } //

    public void StartGame()
    {
        deck = new PokerDeck(); //
        playerHand.Clear(); //
        dealerHand.Clear(); //

        // Reset Visuals
        foreach (var card in playerCardVisuals) card.SetCard(null, false); //
        foreach (var card in dealerCardVisuals) card.SetCard(null, false); //

        pairPlusBet = 0;

        // Ensure the side-bet button is available for a new hand
        if (pairPlusButton != null) pairPlusButton.SetActive(true);

        currentState = GameState.Ante; //
        UpdateUI("Place your Ante and Pair Plus bets."); //
    }

    public void TogglePairPlus()
    {
        if (currentState != GameState.Ante) return; //

        if (pairPlusBet == 0)
        {
            if (bank >= pairPlusIncrement)
            {
                pairPlusBet = pairPlusIncrement;
                bank -= pairPlusBet;
                UpdateUI("Pair Plus Active: $" + pairPlusBet);

                // Hide button immediately after betting
                if (pairPlusButton != null) pairPlusButton.SetActive(false);
            }
            else
            {
                UpdateUI("Not enough bank for Pair Plus!"); //
            }
        }
    }

    public void Deal()
    {
        if (currentState != GameState.Ante) return; //
        if (bank < anteBet) { UpdateUI("Not enough bank for Ante!"); return; } //

        bank -= anteBet;

        // Hide side-bet button if the player deals without betting Pair Plus
        if (pairPlusButton != null) pairPlusButton.SetActive(false);

        StartCoroutine(DealCardsRoutine()); //
    }

    private IEnumerator DealCardsRoutine()
    {
        playerHand.Clear(); //
        dealerHand.Clear(); //

        // One-by-one dealing
        for (int i = 0; i < 3; i++)
        {
            playerHand.Add(deck.Draw());
            playerCardVisuals[i].SetCard(playerHand[i], true); //
            yield return new WaitForSeconds(cardDelay);

            dealerHand.Add(deck.Draw());
            dealerCardVisuals[i].SetCard(dealerHand[i], false); //
            yield return new WaitForSeconds(cardDelay);
        }

        currentState = GameState.Decision; //
        UpdateUI("Cards Dealt. Play ($" + anteBet + ") or Fold?"); //
    }

    public void Play()
    {
        if (currentState != GameState.Decision) return; //
        bank -= anteBet; // In 3-Card Poker, the Play bet equals the Ante
        currentState = GameState.Showdown; //
        StartCoroutine(ResolveRoutine()); //
    }

    public void Fold()
    {
        if (currentState != GameState.Decision) return; //
        UpdateUI("Folded. Bets lost."); //
        ClearTableVisuals();
        StartGame(); //
    }

    private IEnumerator ResolveRoutine()
    {
        // Reveal Dealer cards sequentially
        UpdateUI("Revealing Dealer's Hand...");
        for (int i = 0; i < 3; i++)
        {
            dealerCardVisuals[i].SetCard(dealerHand[i], true); //
            yield return new WaitForSeconds(cardDelay * 2);
        }

        HandEvaluator pEval = new HandEvaluator(playerHand); //
        HandEvaluator dEval = new HandEvaluator(dealerHand); //

        string resultMessage = "";

        // 1. Resolve Pair Plus
        if (pairPlusBet > 0)
        {
            int multiplier = GetPairPlusMultiplier(pEval.Rank); //
            if (multiplier > 0)
            {
                int win = pairPlusBet * multiplier;
                bank += pairPlusBet + win;
                resultMessage += "Pair Plus Wins $" + win + "! ";
            }
        }

        // 2. Resolve Main Hand
        if (!dEval.DealerQualifies()) //
        {
            bank += (anteBet * 2) + anteBet; // Ante wins 1:1, Play pushes
            resultMessage += "Dealer doesn't qualify. Ante Wins!";
        }
        else
        {
            int result = CompareHands(pEval, dEval); //
            if (result > 0)
            {
                bank += (anteBet * 4);
                resultMessage += "Player Wins Main Hand!";
            }
            else if (result < 0) resultMessage += "Dealer Wins Main Hand.";
            else { bank += (anteBet * 2); resultMessage += "Main Hand Push."; }
        }

        UpdateUI(resultMessage); //

        // 3. Post-Game Cleanup
        yield return new WaitForSeconds(3.0f);
        ClearTableVisuals();
        StartGame(); // Automatically resets state to Ante and clears hand lists
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
        if (p.Rank != d.Rank) return p.Rank.CompareTo(d.Rank); //
        return p.HighCardValue.CompareTo(d.HighCardValue); //
    }

    private void ClearTableVisuals()
    {
        foreach (var card in playerCardVisuals) card.SetCard(null, false); //
        foreach (var card in dealerCardVisuals) card.SetCard(null, false); //
    }

    void UpdateUI(string message)
    {
        if (bankText != null) bankText.text = "Bank: $" + bank; //
        if (statusText != null) statusText.text = message; //
    }
}