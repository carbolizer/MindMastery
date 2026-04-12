using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum RoundState
{
    PlayerTurn,
    DealerTurn,
    RoundOver
}


public enum AnimationState
{
    Waiting,
    Showing,
    Paused
}

public enum GameState
{
    Playing,
    Betting,
    Waiting
}

public enum CardValue
{
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}

public enum CardSuit
{
    Clubs,
    Diamonds,
    Hearts,
    Spades
}

public struct DealStep
{
    public bool isPlayer;
    public bool flipped;

    public DealStep(bool isPlayer, bool flipped)
    {
        this.isPlayer = isPlayer;
        this.flipped = flipped;
    }
}


public class BlackJackManager : MonoBehaviour
{


    
    public static BlackJackManager Instance { get; private set; }



    public List<GameObject> deck = new List<GameObject>();


    public List<GameObject> PlayerCards = new List<GameObject>();
    public List<GameObject> DealerCards = new List<GameObject>();

    public List<GameObject> FlippedCards = new List<GameObject>();

    public AnimationState state = AnimationState.Waiting;

    public GameObject CardPrefab;
    public Vector3 PlayerStartPos;
    public Vector3 DealerStartPos;
    public Vector3 DeckPreviewPos;
    public GameObject DeckPreviewCard;
    private Queue<GameObject> previewQueue = new Queue<GameObject>();
    public Queue<DealStep> dealQueue = new Queue<DealStep>();

    public Dictionary<string, GameObject> MenuButtons = new Dictionary<string, GameObject>();

    public TextMeshPro PlayerLabel;
    public TextMeshPro DealerLabel;
    public float timer = 0;
    public float endTimer = 3;
    public float showCardPause = 0.3f;

    public int playerIndex = 0;
    public int dealerIndex = 0;

    private bool showingPlayer = true;
    public bool powersOn = false;

    public GameState gameState = GameState.Betting;
    
    public RoundState roundState;

    public List<GameObject> TableChips = new List<GameObject>();
    private List<GameObject> processedChips = new List<GameObject>();


    public Sprite mindOnSpr;
    public Sprite mindOffSpr;
    public int PlayerBet = 0;



    public float cardSpacing = 0.5f;
    private bool roundEnding = false;
    private bool roundActive = false;


     void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        DeckPreviewCard = Instantiate(CardPrefab);
        DeckPreviewCard.SetActive(false);

        MenuButtons["play"] = GameObject.Find("PlayBtn");
        MenuButtons["mind"] = GameObject.Find("MindBtn");
        MenuButtons["hit"] = GameObject.Find("HitBtn");
        MenuButtons["stand"] = GameObject.Find("StandBtn");
        MenuButtons["fold"] = GameObject.Find("FoldBtn");

        //DealerLabel.text = "-";

        PreshuffleDeck();
    }
    
    Vector3 GetPlayerCardPos(int index)
    {
        return PlayerStartPos + new Vector3(index * cardSpacing, 0, 0);
    }

    Vector3 GetDealerCardPos(int index)
    {
        return DealerStartPos + new Vector3(index * cardSpacing, 0, 0);
    }

    public void PreshuffleDeck()
    {
        StartDeck();
    }


    void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            GameObject temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    void Draw()
    {
        gameState = GameState.Waiting;
        DealerLabel.text = "Draw";
        PlayerLabel.text = "Draw";
        EndRound(false, true);
    }

    void EvaluateWinner()
    {
        int player = GetHandTotal(PlayerCards);
        int dealer = GetHandTotal(DealerCards);

        if (dealer > 21)
        {
            EndRound(true);
        }
        else if (dealer > player)
        {
            EndRound(false);
        }
        else if (dealer < player)
        {
            EndRound(true);
        }
        else
        {
            Draw();
        }
    }

    private System.Collections.IEnumerator DealerPlay()
    {
        while (GetHandTotal(DealerCards) < GetHandTotal(PlayerCards))
        {
            var card = DrawCard(GetDealerCardPos(DealerCards.Count), !powersOn);
            DealerCards.Add(card);
            UpdateZOrder(card, DealerCards.Count - 1);
            card.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            if (GetHandTotal(DealerCards) > 21)
            {
                EndRound(true); 
                yield break;
            }
        }

        EvaluateWinner();
    }

    public void OnStand()
    {
        if (roundState != RoundState.PlayerTurn) return;

        roundState = RoundState.DealerTurn;
        StartCoroutine(DealerPlay());
    }

    public void OnFold()
    {
        switch (gameState)
        {
            case GameState.Playing:
            EndRound(false);
            gameState = GameState.Waiting;
            break;
        }
    }
    


    public void OnPlayGame()
    {
       
        if (gameState != GameState.Betting)
            return;

        TableChips = new List<GameObject>(GlobalGameManager.Instance.ChipsOnTable);
        roundActive = true;
        gameState = GameState.Playing;

        PlayerCards.Clear();
        DealerCards.Clear();
        FlippedCards.Clear();

        FlippedCards.Add(DeckPreviewCard);

        playerIndex = 0;
        dealerIndex = 0;

        var chips = GlobalGameManager.Instance.ChipsOnTable;

        for (int i = 0; i < chips.Count; i++)
        {
            
            if (chips[i] == null) continue;
            PlayerBet += (int)chips[i].GetComponent<Chip>().m_value;
            chips[i].GetComponent<CircleCollider2D>().enabled = false;
            
            
        }

        DealInitialCards();
    }

    void SyncChips()
    {
        var chips = GlobalGameManager.Instance.ChipsOnTable;

        for (int i = 0; i < chips.Count; i++)
        {
            var chip = chips[i];
            if (chip == null) continue;

            if (processedChips.Contains(chip))
                continue;

            AddChipToBet(chip);
            processedChips.Add(chip);
        }
    }

    void UpdateGameState()
    {
        switch (gameState)
        {
            case GameState.Waiting:
            if (endTimer > 0)
            {
                endTimer -= Time.deltaTime;
            } else
            {
                endTimer = 3;
                
                GlobalGameManager.DestroyChipsOnTable(TableChips);
                ClearTableCards();
                
                gameState = GameState.Betting;
            }
            MenuButtons["play"].SetActive(false);
            break;

            case GameState.Playing:
            SyncChips();
            MenuButtons["fold"].SetActive(true);
            MenuButtons["stand"].SetActive(true);
            MenuButtons["hit"].SetActive(true);
            MenuButtons["play"].SetActive(false);
            break;


            case GameState.Betting:
            MenuButtons["fold"].SetActive(false);
            MenuButtons["stand"].SetActive(false);
            MenuButtons["hit"].SetActive(false);
            MenuButtons["play"].SetActive(true);
            break;
        }
    }

    void OnGameEnd()
    {
        //win / lose logic


        state = AnimationState.Paused;
        PreshuffleDeck();
    }

    int GetCardValue(CardValue value)
    {
        return value switch
        {
            CardValue.Ace => 11,
            CardValue.Two => 2,
            CardValue.Three => 3,
            CardValue.Four => 4,
            CardValue.Five => 5,
            CardValue.Six => 6,
            CardValue.Seven => 7,
            CardValue.Eight => 8,
            CardValue.Nine => 9,
            CardValue.Ten => 10,
            CardValue.Jack => 10,
            CardValue.Queen => 10,
            CardValue.King => 10,
            _ => 0
        };
    }

    

    void Update()
    {



        if (!roundActive)
            return;

        UpdateGameState();
        UpdateUI();

        if (gameState == GameState.Playing)
            UpdateTimer();


        if (GlobalGameManager.Instance.ChipsOnTable.Count > 0)
        {
            MenuButtons["play"].GetComponent<BoxCollider2D>().enabled = true;
        } else
        {
            MenuButtons["play"].GetComponent<BoxCollider2D>().enabled = false;
        }

        if (FlippedCards.Count > 0)
        {
            MenuButtons["mind"].GetComponent<BoxCollider2D>().enabled = true;
        } else
        {
            MenuButtons["mind"].GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void DealInitialCards()
    {
        dealQueue.Clear();

        dealQueue.Enqueue(new DealStep(true, false));  // P1
        dealQueue.Enqueue(new DealStep(false, false)); // D1
        dealQueue.Enqueue(new DealStep(true, false));  // P2
        dealQueue.Enqueue(new DealStep(false, true));  // D2

        timer = 0f;
        state = AnimationState.Waiting;
        roundState = RoundState.PlayerTurn;
    }

    void ClearTableCards()
    {
        foreach (var card in PlayerCards)
        {
            if (card != null)
                Destroy(card);
        }

        foreach (var card in DealerCards)
        {
            if (card != null)
                Destroy(card);
        }

        PlayerCards.Clear();
        DealerCards.Clear();
    }

    public void AddChipToBet(GameObject chipObj)
    {
        if (chipObj == null) return;
        if (gameState != GameState.Playing) return; 

        var chip = chipObj.GetComponent<Chip>();
        if (chip == null) return;

        PlayerBet += (int)chip.m_value;

        chipObj.GetComponent<CircleCollider2D>().enabled = false;

        if (!TableChips.Contains(chipObj))
            TableChips.Add(chipObj);

        GlobalGameManager.Instance.ChipsOnTable.Remove(chipObj);
    }


    void EndRound(bool win, bool draw = false)
    {   
        if (roundEnding) return;
            roundEnding = true;
        gameState = GameState.Waiting;
        if (!draw)
        {
            DealerLabel.text = (win ? "you" : "dealer") + " won";
            PlayerLabel.text = (win ? "+" : "-") + $"${PlayerBet}";
        } else
        {
            DealerLabel.text = "draw";
            PlayerLabel.text = $"+${PlayerBet}";
        }
        

        

        
        

        StartCoroutine(EndRoundRoutine(win));
    }

    private System.Collections.IEnumerator EndRoundRoutine(bool win)
    {
        yield return new WaitForSeconds(3f);

        ClearTableCards();
        GlobalGameManager.DestroyChipsOnTable(TableChips);

        PlayerCards.Clear();
        DealerCards.Clear();
        FlippedCards.Clear();

        dealQueue.Clear();
        previewQueue.Clear();

        PreshuffleDeck();

        if (win)
            GlobalGameManager.Player.m_money += PlayerBet * 2;
        else
            GlobalGameManager.Player.m_money -= PlayerBet;
        GlobalGameManager.Player.RefreshChipCount();

        PlayerBet = 0;

        gameState = GameState.Betting;
        roundState = RoundState.PlayerTurn;

        roundEnding = false;
    }


    public void PlayerHit()
    {
        if (roundState != RoundState.PlayerTurn) return;

        var card = DrawCard(GetPlayerCardPos(PlayerCards.Count));
        PlayerCards.Add(card);
        UpdateZOrder(card, PlayerCards.Count - 1);
        card.SetActive(true);

        if (GetHandTotal(PlayerCards) > 21)
        {
            EndRound(false); 
        }
        UpdateDeckPreview();
    }

    public void DealerHit()
    {
        var card = DrawCard(GetPlayerCardPos(PlayerCards.Count), !powersOn);
        DealerCards.Add(card);
        UpdateZOrder(card, DealerCards.Count - 1);
        card.SetActive(true);
        UpdateDeckPreview();
    }

    void UpdateDeckPreview()
    {
        if (deck.Count == 0)
        {
            DeckPreviewCard.SetActive(false);
            return;
        }

        GameObject nextCard = deck[0];

        var next = nextCard.GetComponent<GameCard>();
        var preview = DeckPreviewCard.GetComponent<GameCard>();

        DeckPreviewCard.transform.position = DeckPreviewPos;
        DeckPreviewCard.SetActive(true);

        preview.cardSuit = next.cardSuit;
        preview.cardValue = next.cardValue;
        preview.cardName = next.cardName;
        preview.flipped = !powersOn;

        preview.RefreshVisual();
    }

    string GetCardCode(CardValue value, CardSuit suit)
    {
        string v = value switch
        {
            CardValue.Ace => "A",
            CardValue.Jack => "J",
            CardValue.Queen => "Q",
            CardValue.King => "K",
            CardValue.Ten => "10",
            CardValue.Nine => "9",
            CardValue.Eight => "8",
            CardValue.Seven => "7",
            CardValue.Six => "6",
            CardValue.Five => "5",
            CardValue.Four => "4",
            CardValue.Three => "3",
            CardValue.Two => "2",
            _ => "?"
        };

        string s = suit switch
        {
            CardSuit.Clubs => "C",
            CardSuit.Diamonds => "D",
            CardSuit.Hearts => "H",
            CardSuit.Spades => "S",
            _ => "?"
        };

        return $"{v}-{s}";
    }

    GameObject GetNextCard()
    {
        if (previewQueue.Count == 0) return null;
            return previewQueue.Peek();
    }

    void StartDeck()
    {
        deck.Clear();
        FlippedCards.Clear();
        FlippedCards.Add(DeckPreviewCard);
        


        foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardValue value in System.Enum.GetValues(typeof(CardValue)))
            {
                GameObject card = Instantiate(CardPrefab, GameObject.Find("Cards").transform);
                card.SetActive(false);

                var comp = card.GetComponent<GameCard>();
                comp.cardSuit = suit;
                comp.cardValue = value;
                comp.cardName = GetCardCode(value, suit);

                deck.Add(card);
            }
        }

        ShuffleDeck();
        previewQueue.Clear();

        foreach (var card in deck)
        {
            previewQueue.Enqueue(card);
        }


        UpdateDeckPreview();
    }

    public GameObject DrawCard(Vector3 pos, bool flipped = false)
    {
        if (deck.Count == 0)
            return null;

        GameObject card = deck[0];
        deck.RemoveAt(0);

        card.transform.position = pos;

        var gc = card.GetComponent<GameCard>();
        gc.flipped = flipped;
        
        gc.RefreshVisual();

        card.SetActive(false);

        return card;
    }

    static void UpdateZOrder(GameObject card, int index)
    {
        card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10 - index);
    }



    int GetHandTotal(List<GameObject> hand)
    {
        int total = 0;
        int aceCount = 0;

        foreach (var cardObj in hand)
        {
            var card = cardObj.GetComponent<GameCard>();
            total += GetCardValue(card.cardValue);

            if (card.cardValue == CardValue.Ace)
                aceCount++;
        }


        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }

    void UpdateUI()
    {
        switch (gameState)
        {
            case GameState.Betting:

            PlayerLabel.text = "you";
            DealerLabel.text = "dealer";
            break;
            case GameState.Playing:
            PlayerLabel.text = GetHandTotal(PlayerCards).ToString();
            DealerLabel.text = GetHandTotal(DealerCards).ToString(); 
            break;
        }
        
    }









    void UpdateTimer()
    {
        switch (state)
        {
            case AnimationState.Waiting:
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    state = AnimationState.Showing;
                }
                break;

            case AnimationState.Showing:

            if (dealQueue.Count == 0)
            {
                state = AnimationState.Paused;
                break;
            }

            DealStep step = dealQueue.Dequeue();

            GameObject card;
            bool addToFlipped = false;
            
            if (step.flipped)
            {
                step.flipped = !powersOn;
                addToFlipped = true;
            }

            if (step.isPlayer)
            {
                card = DrawCard(GetPlayerCardPos(PlayerCards.Count), step.flipped);
                PlayerCards.Add(card);
                UpdateZOrder(card, PlayerCards.Count - 1);
            }
            else
            {
                card = DrawCard(GetDealerCardPos(DealerCards.Count), step.flipped);
                DealerCards.Add(card);
                UpdateZOrder(card, DealerCards.Count - 1);
            }

            if (addToFlipped)
            {
                FlippedCards.Add(card);
            }

            

            card.SetActive(true); 

            UpdateDeckPreview();

            timer = showCardPause;
            state = AnimationState.Waiting;
            break;

            case AnimationState.Paused:
                break;
        }
    }



    public void FlipCards()
    {
        powersOn = !powersOn;

        if (powersOn)
        {
            MenuButtons["mind"].GetComponent<SpriteRenderer>().sprite = mindOnSpr;
            DealerLabel.text = GetHandTotal(DealerCards).ToString();
        } else
        {
            MenuButtons["mind"].GetComponent<SpriteRenderer>().sprite = mindOffSpr;
            //DealerLabel.text = "-";
        }

        foreach (var obj in FlippedCards)
        {
            if (obj == null) continue;
            obj.GetComponent<GameCard>().FlipCard();
        }

       
    }
}
