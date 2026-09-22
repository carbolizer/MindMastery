# State of Mind

State of Mind is a Unity game-jam project built around casino-style mini-games and a shared player state. The repository includes Blackjack, Roulette, and Three Card Poker scenes along with common UI, player, card, and state-management systems.

My main work was on the **Three Card Poker** portion of the project.

## Three Card Poker

The current implementation includes:

- Ante and Pair Plus betting
- Play-or-fold decision flow
- a three-card hand evaluator
- dealer qualification rules
- Pair Plus payout multipliers
- shared player balance integration
- card reveal timing and round cleanup
- a suspicion mechanic tied into the project's broader player state
- UI updates for balance, wager, status, and outcomes

## My contributions

My commits include:

- adding the Three Card Poker scene and related gameplay work
- updating and fixing the Three Card Poker game manager
- adding work around the Pair Plus and mind-power mechanics
- adding navigation back from the Three Card Poker experience
- integration fixes during the game jam

This was a collaborative game-jam repository, so other mini-games and shared systems were built by other members of the team.

## Tech

- **Engine:** Unity 6
- **Language:** C#
- **UI:** TextMeshPro / Unity UI

## Code worth looking at

```text
Assets/Scripts/ThreeCardPoker/
├── Three Card Game Manager.cs
├── HandEvaluator.cs
├── PokerCard.cs
├── PokerDeck.cs
├── CardVisual.cs
├── PairPlus.cs
└── MindPowers.cs
```

The hand evaluator separates card-ranking logic from the scene/game-state controller, while the game manager handles betting, dealing, payouts, UI state, and integration with the shared player balance.

## Running the project

This repository was built with **Unity 6000.3.10f1**.

1. Clone the repository.
2. Open it in Unity Hub with the matching Unity version.
3. Open a scene under `Assets/Scenes/` such as `ThreeCardPoker.unity`.
4. Enter Play mode from the Unity editor.
