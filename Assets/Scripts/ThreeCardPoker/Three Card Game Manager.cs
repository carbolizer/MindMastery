using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI; 

public class ThreeCardGameManager : MonoBehaviour
{
    public enum GameState { Betting, Decision, Showdown }
    public GameState currentState;

    [Header("Financial Settings")]
    public int balance = 9000;
    public int totalWager = 0;
    public int currentAnte = 0;
    public int currentPairPlus = 0;
    public int currentPlayBet = 0;

    [Header("Cheat Mechanics")]
    public Slider suspicionBar;      
    public float currentSuspicion = 0f;
    public float maxSuspicion = 100f;
    public float suspicionPenalty = 34f; // Takes 3 peeks to hit 100 and bust

    [Header("UI References")]
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI wagerText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI statusText;

    [Header("Visual References")]
    public CardVisual[] playerCardVisuals;
    public CardVisual[] dealerCardVisuals;
    public float cardDelay = 0.25f;

    private PokerDeck deck;
    private List<PokerCard> playerHand = new List<PokerCard>();
    private List<PokerCard> dealerHand = new List<PokerCard>();

    void Start()
    {
        if (suspicionBar != null) suspicionBar.value = 0;
        StartGame();
    }

    public void StartGame()
    {
        deck = new PokerDeck();
        playerHand.Clear();
        dealerHand.Clear();
        foreach (var card in playerCardVisuals) card.SetCard(null, false);
        foreach (var card in dealerCardVisuals) card.SetCard(null, false);

        currentAnte = 0;
        currentPairPlus = 0;
        currentPlayBet = 0;
        totalWager = 0;

        currentState = GameState.Betting;
        UpdateUI("Place your bets using chips.");
    }

    // --- CHEAT MECHANIC ---
    public void PeekAtDealerCards()
    {
        // Only allow cheating after cards are dealt but before the round is resolved
        if (currentState != GameState.Decision) return;

        // Reveal the dealer's cards early
        for (int i = 0; i < 3; i++)
        {
            dealerCardVisuals[i].SetCard(dealerHand[i], true);
        }

        // Apply heat
        currentSuspicion += suspicionPenalty;

        // Update the visual bar (Slider values go from 0 to 1)
        if (suspicionBar != null)
        {
            suspicionBar.value = currentSuspicion / maxSuspicion;
        }

        if (currentSuspicion >= maxSuspicion)
        {
            StartCoroutine(BustedRoutine());
        }
        else
        {
            UpdateUI("Careful... The dealer is getting suspicious.");
        }
    }

    private IEnumerator BustedRoutine()
    {
        currentState = GameState.Showdown; // Lock out further button clicks

        // Wipe the player out
        balance = 0;
        totalWager = 0;
        currentAnte = 0;
        currentPairPlus = 0;
        currentPlayBet = 0;

        UpdateUI("CAUGHT CHEATING! Security kicked you out and confiscated your chips.");
        UpdateWinText(0);

        yield return new WaitForSeconds(4.0f);

        // Reset their suspicion and money
        currentSuspicion = 0f;
        if (suspicionBar != null) suspicionBar.value = 0;
        balance = 1000;

        StartCoroutine(CleanupAfterHand(0f));
    }
    // -----------------------

    public void AddAnte(int amount)
    {
        if (currentState != GameState.Betting || balance < amount) return;
        currentAnte += amount;
        balance -= amount;
        totalWager += amount;
        UpdateUI("Ante placed: $" + currentAnte);
    }

    public void AddPairPlus(int amount)
    {
        if (currentState != GameState.Betting || balance < amount) return;
        currentPairPlus += amount;
        balance -= amount;
        totalWager += amount;
        UpdateUI("Pair Plus placed: $" + currentPairPlus);
    }

    public void Deal()
    {
        if (currentState != GameState.Betting || currentAnte <= 0)
        {
            UpdateUI("You must place an Ante bet first!");
            return;
        }
        StartCoroutine(DealCardsRoutine());
    }

    private IEnumerator DealCardsRoutine()
    {
        for (int i = 0; i < 3; i++)
        {
            playerHand.Add(deck.Draw());
            playerCardVisuals[i].SetCard(playerHand[i], true);
            yield return new WaitForSeconds(cardDelay);

            dealerHand.Add(deck.Draw());
            dealerCardVisuals[i].SetCard(dealerHand[i], false);
            yield return new WaitForSeconds(cardDelay);
        }
        currentState = GameState.Decision;
        UpdateUI("Play or Fold?");
    }

    public void Play()
    {
        if (currentState != GameState.Decision) return;

        currentPlayBet = currentAnte;
        balance -= currentPlayBet;
        totalWager += currentPlayBet;

        currentState = GameState.Showdown;
        StartCoroutine(ResolveRoutine());
    }

    public void Fold()
    {
        if (currentState != GameState.Decision) return;
        UpdateUI("Folded. Bets lost.");
        StartCoroutine(CleanupAfterHand(2.0f));
    }

    private IEnumerator ResolveRoutine()
    {
        // Dealer cards are revealed here (if they weren't already flipped by the cheat button)
        for (int i = 0; i < 3; i++)
        {
            dealerCardVisuals[i].SetCard(dealerHand[i], true);
            yield return new WaitForSeconds(cardDelay * 2);
        }

        HandEvaluator pEval = new HandEvaluator(playerHand);
        HandEvaluator dEval = new HandEvaluator(dealerHand);
        int sessionWin = 0;

        if (currentPairPlus > 0)
        {
            int multiplier = GetPairPlusMultiplier(pEval.Rank);
            if (multiplier > 0) sessionWin += (currentPairPlus + (currentPairPlus * multiplier));
        }

        if (!dEval.DealerQualifies())
        {
            sessionWin += (currentAnte * 2) + currentPlayBet;
        }
        else
        {
            int result = CompareHands(pEval, dEval);
            if (result > 0) sessionWin += (currentAnte * 2) + (currentPlayBet * 2);
            else if (result == 0) sessionWin += currentAnte + currentPlayBet;
        }

        balance += sessionWin;
        UpdateWinText(sessionWin);
        UpdateUI(sessionWin > 0 ? "You Won $" + sessionWin + "!" : "Dealer Wins.");

        yield return new WaitForSeconds(3.0f);
        StartCoroutine(CleanupAfterHand(0f));
    }

    private IEnumerator CleanupAfterHand(float wait)
    {
        yield return new WaitForSeconds(wait);
        foreach (var card in playerCardVisuals) card.SetCard(null, false);
        foreach (var card in dealerCardVisuals) card.SetCard(null, false);
        UpdateWinText(0);
        StartGame();
    }

    private int GetPairPlusMultiplier(PokerHandRank rank)
    {
        switch (rank)
        {
            case PokerHandRank.StraightFlush: return 40;
            case PokerHandRank.ThreeOfAKind: return 30;
            case PokerHandRank.Straight: return 6;
            case PokerHandRank.Flush: return 4;
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
        if (balanceText != null) balanceText.text = "$" + balance.ToString("N0");
        if (wagerText != null) wagerText.text = "$" + totalWager.ToString("N0");
        if (statusText != null) statusText.text = message;
    }

    void UpdateWinText(int amount)
    {
        if (winText != null) winText.text = amount > 0 ? "$" + amount.ToString("N0") : "";
    }
}