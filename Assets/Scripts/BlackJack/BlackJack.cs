using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;


public class BlackJack : MonoBehaviour {
// public
    public float actionDelay = 0.25f;   // seconds
    public GameObject playerHand;
    public GameObject dealerHand;
    public GameObject playerCardStart;  // where the players hand starts on screen
    public GameObject dealerCardStart;  // where the dealers hand starts on screen
    public GameObject cardPrefab;
    public GameObject againButton;
    public GameObject playButton;
    public float cardOffset = 48f;
    public float cardWidth = 64f;
    public TextMeshPro playerCardValue;
    public TextMeshPro dealerCardValue;
    public TextMeshPro stateText;
    public TextMeshPro betAmountText;
    public GameObject chipBox;

    public PlayState m_playState = PlayState.Betting;

    private float bustTimer = 0f;
    private bool waitingAfterBust = false;



    public enum PlayState
    {
        Betting,
        Playing,
        Resetting
    }
    /*
        Create blackjack states
        Create transitions between states
        Add transitions to states
        Add states to state machine
    */
    void Start() {
        deck = new();
        stateText.text = "";
        againButton.SetActive(false);
        playButton.SetActive(false);
        
        State bet       = new("bet",        () => { shouldPlayAgain = false; placedBet = false; playerBet = 0; resolveTimer = 0f; playButton.SetActive(false); m_playState = PlayState.Betting; chipBox.SetActive(true); }, (dt) => { playButton.SetActive(playerBet > 0); }, () => { playButton.SetActive(false); chipBox.SetActive(false); });
        State deal = new("deal",
            () =>
            {
                shouldPlayAgain = false;
                resolveTimer = 0f;

               
                GlobalGameManager.Player.m_money -= playerBet;
                GlobalGameManager.Instance.DestroyChipsOnTable();

                DestroyCards();
                playerCards.Clear();
                dealerCards.Clear();
                dealerCardUIs.Clear();
                dealState = 0;
                canSwitchDealerCard = false;
                bust = false;
                blackJack = false;
                playerSum = 0;
                dealerSum = 0;
            },
            UpdateDeal,
            () => {}
            );
        State play      = new("play",       () => { shouldHit = false; shouldStand = false; blackJack = playerSum == 21; }, (dt) => {}, () => {});
        State hit       = new("hit",        () => { shouldHit = false; hitComplete = false; }, UpdateHit, () => {});
        State stand     = new("stand",      () => { shouldStand = false; standDone = false; RevealCard(1); }, UpdateStand, () => {});
        State resolve   = new("resolve",    Resolve, UpdateResolve, () => { stateText.text = ""; againButton.SetActive(false); });
        State quit      = new("quit",       () => { shouldQuit = false; }, (dt) => {}, () => {});

        Transition startBet         = new("bet",        () => { return shouldPlayAgain; });
        Transition startDeal        = new("deal",       () => { return placedBet; });
        Transition startPlay        = new("play",       () => { return dealerCards.Count == 2 && playerCards.Count == 2; });
        Transition startHit         = new("hit",        () => { return shouldHit; });
        Transition startStand       = new("stand",      () => { return shouldStand; });
        Transition bustToResolve    = new("resolve",    () => { return bust; });
        Transition hitTo21Resolve   = new("resolve",    () => { return hitComplete && playerSum == 21; });
        Transition hitToPlay        = new("play",       () => { return hitComplete; });
        Transition blackJackToResolve = new("resolve",  () => { return blackJack; });
        Transition standToResolve   = new("resolve",    () => { return standDone; });
        Transition startQuit        = new("quit",       () => { return shouldQuit; });

        bet.AddTransition(startDeal);
        bet.AddTransition(startQuit);
        deal.AddTransition(startPlay);
        play.AddTransition(blackJackToResolve);
        play.AddTransition(startHit);
        play.AddTransition(startStand);
        hit.AddTransition(bustToResolve);
        hit.AddTransition(hitTo21Resolve);
        hit.AddTransition(hitToPlay);
        stand.AddTransition(standToResolve);
        resolve.AddTransition(startBet);

        stateMachine.AddState(bet);
        stateMachine.AddState(deal);
        stateMachine.AddState(play);
        stateMachine.AddState(hit);
        stateMachine.AddState(stand);
        stateMachine.AddState(resolve);
        stateMachine.AddState(quit);

        stateMachine.SetState("bet");

        
    }

    void UpdateTable()
    {
        int bet = 0;
        foreach (var chip in GlobalGameManager.Instance.ChipsOnTable)
        {   
            bet += (int)chip.GetComponent<Chip>().m_value;
        }
        playerBet = bet;
    }

    void Update() { 
        stateMachine.Update(Time.deltaTime);
        playerCardValue.text = playerSum.ToString();
        dealerCardValue.text = dealerSum.ToString();
        betAmountText.text = playerBet.ToString();
        
        switch (m_playState)
        {
            case PlayState.Betting:
            UpdateTable();
            playButton.SetActive(playerBet > 0);
            againButton.SetActive(false);
            break;

            case PlayState.Playing:
            playButton.SetActive(false);
            break;

            case PlayState.Resetting:
            againButton.SetActive(true);
            break;
        }
        
        





    }

    public void ChangeBet(int amount)   { if (playerBet + amount >= 0) playerBet += amount; }
    public void PlaceBet()              { placedBet = true; }
    public void Hit()                   { shouldHit = true; }
    public void Stand()                 { shouldStand = true; }
    public void Quit()                  { shouldQuit = true; }
    public void PlayAgain()             { if (m_playState == PlayState.Playing) { m_playState = PlayState.Resetting; } else { m_playState = PlayState.Playing; } shouldPlayAgain = true; if (playerBet > 0) placedBet = true; }

// private
    private Deck            deck;
    private StateMachine    stateMachine    = new();
    private List<Card>      dealerCards     = new();
    private List<Card>      playerCards     = new();
    private int             dealerSum       = 0;
    private int             playerSum       = 0;

    private int             playerBet       = 0;

    private bool            placedBet       = false;
    private bool            shouldHit       = false;
    private bool            shouldStand     = false;
    private bool            shouldQuit      = false;
    private bool            shouldPlayAgain = false;
    private bool            bust            = false;
    private bool            blackJack       = false;
    private bool            hitComplete     = false;
    private bool            standDone       = false;
    private bool            canSwitchDealerCard = false;

    private int             sessionEarnings = 0;
    private float           resolveTimer    = 0f;

    private struct CardUI { public Card card; public GameObject obj; }
    private List<CardUI>    dealerCardUIs   = new();

    private float           actionTimer     = 0f;
    private int             dealState       = 0;

    private float playerCardStartOffset     = 0f;
    private float dealerCardStartOffset     = 0f;

    private void UpdateDeal(float dt) {
        actionTimer += Time.deltaTime;

        if (actionTimer <= actionDelay) return;

        switch (dealState) {
            case 0: 
                var pcard1 = deck.PullTopCard(); 
                playerCards.Add(pcard1); 
                EvaluatePlayerSum(); 
                AddCardToUI(pcard1, playerCardStart.transform, true); 
                break;
            case 1: 
                var dcard1 = deck.PullTopCard();
                dealerCards.Add(dcard1); 
                EvaluateDealerSum(); 
                AddCardToUI(dcard1, dealerCardStart.transform, false);
                break;
            case 2:
                var pcard2 = deck.PullTopCard(); 
                playerCards.Add(pcard2);
                EvaluatePlayerSum();
                AddCardToUI(pcard2, playerCardStart.transform, true);
                break;
            case 3:
                var dcard2 = deck.PullTopCard();
                dcard2.hidden = true;
                dealerCards.Add(dcard2); 
                EvaluateDealerSum(); 
                AddCardToUI(dcard2, dealerCardStart.transform, false);
                break;
        }

        dealState += 1;
        actionTimer = 0f;
    }

    private void UpdateHit(float dt) {
        actionTimer += Time.deltaTime;

        if (playerSum > 21)
        {
            bust = true;
            waitingAfterBust = true;
            bustTimer = 0f;
        }

        // if (actionTimer <= actionDelay) return;

        var card = deck.PullTopCard();
        playerCards.Add(card);
        EvaluatePlayerSum();
        AddCardToUI(card, playerCardStart.transform, true);
        if (playerSum > 21) bust = true;
        hitComplete = true;

        actionTimer = 0f;
    }

    private void UpdateStand(float dt) {
        actionTimer += Time.deltaTime;

        if (actionTimer <= actionDelay) return;

        if (dealerSum < 17) {
            var card = deck.PullTopCard();
            dealerCards.Add(card);
            EvaluateDealerSum();
            AddCardToUI(card, dealerCardStart.transform, false);
        } else standDone = true;

        actionTimer = 0f;
    }

    private void Resolve()
    {
        int payout = 0;

        if (bust)
        {
            payout = 0;
            stateText.text = "BUST";
        }
        else if (blackJack && dealerSum != 21)
        {
            payout = Mathf.RoundToInt(playerBet * 2.5f); // original + 1.5x
            stateText.text = "BLACKJACK";
        }
        else if (dealerSum > 21 || playerSum > dealerSum)
        {
            payout = playerBet * 2; // original + win
            stateText.text = "WIN";
        }
        else if (playerSum < dealerSum)
        {
            payout = 0;
            stateText.text = "DEALER WINS";
        }
        else
        {
            payout = playerBet; // push (refund)
            stateText.text = "PUSH";
        }

        //Pay Player
        GlobalGameManager.Player.m_money += payout;
        GlobalGameManager.Player.RefreshChipCount();

        Debug.Log("Money after round: " + GlobalGameManager.Player.m_money);
    }

    private void EvaluatePlayerSum() {
        playerSum = 0;

        foreach (Card card in playerCards) {
            if (card.name[0] == 'A') { 
                if (playerSum + card.value > 21) playerSum += 1;
                else playerSum += card.value;
                continue;
            }

            playerSum += card.value;
        }
    }

    private void EvaluateDealerSum() {
        dealerSum = 0;

        foreach (Card card in dealerCards) {
            if (card.hidden) continue;
            if (card.name[0] == 'A') {
                if (dealerSum + card.value > 21) dealerSum += 1;
                else dealerSum += card.value;
                continue;
            }

            dealerSum += card.value;
        }
    }

    private void UpdateResolve(float dt)
    {
        // If bust, delay reset
        if (waitingAfterBust)
        {
            bustTimer += dt;

            if (bustTimer >= 3f)
            {
                waitingAfterBust = false;
                bustTimer = 0f;

                DestroyCards();
                playerCards.Clear();
                dealerCards.Clear();
                dealerCardUIs.Clear();

                playerSum = 0;
                dealerSum = 0;

                againButton.SetActive(true);
            }

            return;
        }

        // normal resolve timing
        resolveTimer += dt;

        if (resolveTimer >= 1f && !againButton.activeSelf)
        {
            DestroyCards();
            playerCards.Clear();
            dealerCards.Clear();
            dealerCardUIs.Clear();

            playerSum = 0;
            dealerSum = 0;

            againButton.SetActive(true);
        }
    }

    private void RevealCard(int index) {
        if (index >= dealerCardUIs.Count) return;
        var cui = dealerCardUIs[index];
        cui.obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Cards/" + cui.card.name);
    }

    public void FlipHiddenCard() {
        if (canSwitchDealerCard) { SwitchDealerCard(); canSwitchDealerCard = false; return; }
        for (int i = 0; i < dealerCardUIs.Count; i++) {
            if (dealerCardUIs[i].card.hidden) {
                // find and unhide the matching card in dealerCards
                for (int j = 0; j < dealerCards.Count; j++) {
                    if (dealerCards[j].name == dealerCardUIs[i].card.name) {
                        Card c = dealerCards[j];
                        c.hidden = false;
                        dealerCards[j] = c;
                        break;
                    }
                }
                RevealCard(i);
                canSwitchDealerCard = true;
                EvaluateDealerSum();
                return;
            }
        }
    }

    private void SwitchDealerCard() {
        Card newCard = deck.PullTopCard();
        dealerCards[1] = newCard;
        var cui = dealerCardUIs[1];
        cui.card = newCard;
        cui.obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/" + newCard.name);
        dealerCardUIs[1] = cui;
        EvaluateDealerSum();
    }

    private void AddCardToUI(Card card, Transform transform, bool player) {
        var cardSprite = Resources.Load<Sprite>(card.hidden ? "Cards/back" : "Cards/" + card.name);
        var obj = Instantiate(cardPrefab, transform);

        

        if (player) {
            int n = playerCards.Count;
            float totalWidth = (n - 1) * cardOffset + cardWidth;
            playerHand.transform.localPosition = new Vector3(-totalWidth / 2 / 16f, playerHand.transform.localPosition.y, 0);
            obj.transform.localPosition = new Vector3((n - 1) * cardOffset, 0, -10 - n);
        }
        else {
            int n = dealerCards.Count;
            float totalWidth = (n - 1) * cardOffset + cardWidth;
            dealerHand.transform.localPosition = new Vector3(-totalWidth / 2 / 16f, dealerHand.transform.localPosition.y, 0);
            obj.transform.localPosition = new Vector3((n - 1) * cardOffset, 0, -10 - n);
            dealerCardUIs.Add(new CardUI { card = card, obj = obj });
        }
        obj.GetComponent<SpriteRenderer>().sprite = cardSprite;
    }

    private void DestroyCards() {
        foreach (Transform child in playerCardStart.transform) Destroy(child.gameObject);
        foreach (Transform child in dealerCardStart.transform) Destroy(child.gameObject);
    }
}
