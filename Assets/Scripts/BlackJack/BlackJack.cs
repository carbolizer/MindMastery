using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BlackJack : MonoBehaviour {
// public
    public float actionDelay = 0.25f; // seconds

    /*
        Create blackjack states
        Create transitions between states
        Add transitions to states
        Add states to state machine
    */
    void Start() {
        deck = new();
        
        State bet       = new("bet",        () => { shouldPlayAgain = false; placedBet = false; playerBet = 0; }, (dt) => {}, () => {});
        State deal      = new("deal",       () => { playerCards.Clear(); dealerCards.Clear(); dealState = 0; bust = false; blackJack = false; playerSum = 0; dealerSum = 0; }, UpdateDeal, () => {});
        State play      = new("play",       () => { shouldHit = false; shouldStand = false; blackJack = playerSum == 21; }, (dt) => {}, () => {});
        State hit       = new("hit",        () => { shouldHit = false; hitComplete = false; }, UpdateHit, () => {});
        State stand     = new("stand",      () => { shouldStand = false; standDone = false; }, UpdateStand, () => {});
        State resolve   = new("resolve",    Resolve, (dt) => {}, () => {});
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

    void Update() { stateMachine.Update(Time.deltaTime); }

    public void ChangeBet(int amount)   { if (playerBet + amount > 0) playerBet += amount; }
    public void PlaceBet()              { placedBet = true; }
    public void Hit()                   { shouldHit = true; }
    public void Stand()                 { shouldStand = true; }
    public void Quit()                  { shouldQuit = true; }
    public void PlayAgain()             { shouldPlayAgain = true; }

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

    private int             sessionEarnings = 0;

    private float           actionTimer     = 0f;
    private int             dealState       = 0;

    private void UpdateDeal(float dt) {
        actionTimer += Time.deltaTime;

        if (actionTimer <= actionDelay) return;

        switch (dealState) {
            case 0: playerCards.Add(deck.PullTopCard()); EvaluatePlayerSum(); break;
            case 1: dealerCards.Add(deck.PullTopCard()); EvaluateDealerSum(); break;
            case 2: playerCards.Add(deck.PullTopCard()); EvaluatePlayerSum(); break;
            case 3: dealerCards.Add(deck.PullTopCard()); EvaluateDealerSum(); break;
        }

        dealState += 1;
        actionTimer = 0f;
    }

    private void UpdateHit(float dt) {
        actionTimer += Time.deltaTime;

        if (actionTimer <= actionDelay) return;

        playerCards.Add(deck.PullTopCard());
        EvaluatePlayerSum();
        if (playerSum > 21) bust = true;
        hitComplete = true;

        actionTimer = 0f;
    }

    private void UpdateStand(float dt) {
        actionTimer += Time.deltaTime;

        if (actionTimer <= actionDelay) return;

        if (dealerSum < 17) {
            dealerCards.Add(deck.PullTopCard());
            EvaluateDealerSum();
        } else standDone = true;

        actionTimer = 0f;
    }

    private void Resolve() {
        if (bust)                                           sessionEarnings -= playerBet;
        else if (blackJack && dealerSum != 21)              sessionEarnings += (int)(playerBet * 1.5f);
        else if (dealerSum > 21 || playerSum > dealerSum)   sessionEarnings += playerBet;
        else if (playerSum < dealerSum)                     sessionEarnings -= playerBet;
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
            if (card.name[0] == 'A') {
                if (dealerSum + card.value > 21) dealerSum += 1;
                else dealerSum += card.value;
                continue;
            }

            dealerSum += card.value;
        }
    }
}
