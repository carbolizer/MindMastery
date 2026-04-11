using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BlackJack : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        State betting   = new("betting",    () => {}, (_) => {}, () => {});
        State dealing   = new("dealing",    () => {}, (_) => {}, () => {});
        State hit       = new("hit",        () => {}, (_) => {}, () => {});
        State stand     = new("stand",      () => {}, (_) => {}, () => {});
        State resolve   = new("resolve",    () => {}, (_) => {}, () => {});

        // Transition startDeal = new("dealing", );
    }

    // Update is called once per frame
    void Update() {
        stateMachine.Update(Time.deltaTime);
    }

    private Deck deck = new();
    private StateMachine stateMachine = new();
    private List<Card> dealerCards = new();
    private List<Card> playerCards = new();
}
