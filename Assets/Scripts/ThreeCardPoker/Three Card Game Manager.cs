using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ThreeCardGameManager : MonoBehaviour
{
    public enum GameState { Betting, Decision, Showdown, Locked } // Added Locked state for game over
    public GameState currentState;

    [Header("Financial Settings")]
    public int totalWager = 0;
    public int currentAnte = 0;
    public int currentPairPlus = 0;
    public int currentPlayBet = 0;

    [Header("Cheat Mechanics UI")]
    public Slider suspicionBar;
    public TextMeshProUGUI suspicionPercentageText; // The percentage text
    public int peekPenalty = 34; // Amount sent to GlobalGameManager

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
        if (GlobalGameManager.Instance != null)
        {
            GlobalGameManager.Instance.currentGame = CurrentGame.ThreeCardPoker;
        }

        StartGame();
    }

    // NEW: Update runs every frame to sync the UI with the GlobalGameManager
    void Update()
    {
        if (GlobalGameManager.Instance != null && GlobalGameManager.Player != null)
        {
            float currentSus = GlobalGameManager.Player.m_suspicion;

            // 1. Update the Slider (0.0 to 1.0)
            if (suspicionBar != null)
            {
                suspicionBar.value = currentSus / 100f;
            }

            // 2. Update the Percentage Text (e.g., "34%")
            if (suspicionPercentageText != null)
            {
                suspicionPercentageText.text = Mathf.FloorToInt(currentSus) + "%";
            }

            // 3. Catch the Criminal State triggered by GlobalGameManager
            if (GlobalGameManager.Player.m_state == PlayerSpecialState.Criminal && currentState != GameState.Locked)
            {
                StartCoroutine(BustedRoutine());
            }

            // 4. Continuously update balance UI from the global player profile
            if (balanceText != null)
            {
                balanceText.text = "$" + GlobalGameManager.Player.m_money.ToString("N0");
            }
        }
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
        if (currentState != GameState.Decision) return;

        // Reveal the dealer's cards early
        for (int i = 0; i < 3; i++)
        {
            dealerCardVisuals[i].SetCard(dealerHand[i], true);
        }

        // Apply heat to the GLOBAL manager
        if (GlobalGameManager.Instance != null)
        {
            GlobalGameManager.Instance.UpdateSuspicionBy(peekPenalty);
            UpdateUI("Careful... The dealer is getting suspicious.");
        }
    }

    private IEnumerator BustedRoutine()
    {
        currentState = GameState.Locked; // Lock out further button clicks

        // Wipe the local wagers
        totalWager = 0;
        currentAnte = 0;
        currentPairPlus = 0;
        currentPlayBet = 0;

        UpdateUI("CAUGHT CHEATING! Security is on the way!");
        UpdateWinText(0);

        // We don't need to manually reset the game here because your GlobalGameManager
        // will trigger the "PlayLoseAnim" and switch scenes automatically.
        yield return null;
    }
    // -----------------------

    public void AddAnte(int amount)
    {
        if (currentState != GameState.Betting || GlobalGameManager.Player.m_money < amount) return;
        currentAnte += amount;
        GlobalGameManager.Player.m_money -= amount; // Subtract from GLOBAL money
        GlobalGameManager.Player.RefreshChipCount(); // Update global chips
        totalWager += amount;
        UpdateUI("Ante placed: $" + currentAnte);
    }

    public void AddPairPlus(int amount)
    {
        if (currentState != GameState.Betting || GlobalGameManager.Player.m_money < amount) return;
        currentPairPlus += amount;
        GlobalGameManager.Player.m_money -= amount; // Subtract from GLOBAL money
        GlobalGameManager.Player.RefreshChipCount(); // Update global chips
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
        GlobalGameManager.Player.m_money -= currentPlayBet; // Subtract from GLOBAL money
        GlobalGameManager.Player.RefreshChipCount();
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

        // Add winnings to GLOBAL money
        GlobalGameManager.Player.m_money += sessionWin;
        GlobalGameManager.Player.RefreshChipCount();

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

        // Check if they are broke AFTER the hand is over
        if (GlobalGameManager.Player.m_money < 10)
        {
            UpdateUI("You're broke! Security is escorting you out.");
            GlobalGameManager.Player.m_state = PlayerSpecialState.Broke;
        }
        else
        {
            StartGame(); // Only start a new hand if they can afford it
        }
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
        // Balance text is now handled in Update()
        if (wagerText != null) wagerText.text = "$" + totalWager.ToString("N0");
        if (statusText != null) statusText.text = message;
    }

    void UpdateWinText(int amount)
    {
        if (winText != null) winText.text = amount > 0 ? "$" + amount.ToString("N0") : "";
    }
}