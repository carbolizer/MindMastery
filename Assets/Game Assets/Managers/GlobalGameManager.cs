using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;
using TMPro;




public enum GameScenes
{
    Default,
    Roulette,
    Blackjack,
    ThreeCardPoker,
    LoseScreen,
    WinScreen,
    MainMenu
}


public enum CurrentGame
{
    None, //Roaming casino
    Roulette,
    BlackJack,
    ThreeCardPoker
}




public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance { get; private set; }
    public GameObject FadeOutObj;
    public static PlayerData Player;
    public Vector3 CursorPos;
    public CursorState CursorState;

    public CurrentGame currentGame;

    public bool PlayLoseAnim = false;


    public GameObject HoveredObject = null;
    public bool HoveringObject = false;

    //Internal logic use only; used to determine HoveredObject
    [SerializeField]
    private List<GameObject> HoveredObjects = new List<GameObject>();
    public List<GameObject> ChipsOnTable;

    public bool DisableChipHover = false;
    [System.NonSerialized]
    public float GrabDistanceThreshold = 5.0f;
    [System.NonSerialized]
    public float DrinkTimer = 0;
    public bool CanDrink = true;

    public GameObject MindLabel;
    public GameObject SusLabel;
    public GameObject MoneyLabel;




    public GameScenes CurrentScene = GameScenes.Default;


    public void DestroyChipsOnTable()
    {
        var list = Instance.ChipsOnTable;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            Destroy(list[i]);
        }
        list.Clear();
    }

    public static void DestroyChipsOnTable(List<GameObject> list)
    {
    

        for (int i = list.Count - 1; i >= 0; i--)
        {
            Destroy(list[i]);
        }
        list.Clear();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Player = new();
            DontDestroyOnLoad(gameObject);
            InitializeChips();
        }
    }

    void Update()
    {

        if (Player.m_money < 10 && currentGame == CurrentGame.None)
        {
            Player.m_state = PlayerSpecialState.Broke;
        }

        if (Player.m_suspicion >= 100)
        {
            Player.m_state = PlayerSpecialState.Criminal;
        }

        if (Player.m_sobriety <= 0)
        {
            Player.m_state = PlayerSpecialState.Drunk;
        }
        MindLabel = GameObject.Find("Label1");
        SusLabel = GameObject.Find("Label2");
        MoneyLabel = GameObject.Find("Label3");

        if (MindLabel != null)  MindLabel.GetComponent<TextMeshPro>().text = $"mind {(int)Player.m_sobriety}%";
        if (SusLabel != null) SusLabel.GetComponent<TextMeshPro>().text = $"suspicion {(int)Player.m_suspicion}%";
        if (MoneyLabel != null) MoneyLabel.GetComponent<TextMeshPro>().text = $"money ${(int)Player.m_money}";


        switch (Player.m_state)
        {
            case PlayerSpecialState.None:
            UpdateHoverList();
            UpdateSobriety();
            UpdateSuspicion();
            break;

            case PlayerSpecialState.Criminal:
            PlayLoseAnim = true;
            break;

            case PlayerSpecialState.Drunk:
            PlayLoseAnim = true;
            break;

            case PlayerSpecialState.Broke:
            PlayLoseAnim = true;
            break;
        }

        if (PlayLoseAnim)
        {
            //Call the fade out to new scene
            Instance.FadeOutObj.GetComponent<FadeOutFuncs>().animator.SetBool("GameLose", true);
            PlayLoseAnim = false;
        }
        
    }

    public void OnDrink()
    {
        int drinkAmount = Random.Range(2, 15);
        int susAmount = Random.Range(3, 13);


        GlobalGameManager.Instance.DrinkTimer = 10;
        GlobalGameManager.Instance.CanDrink = false;

        if (GlobalGameManager.Player.m_sobriety >= drinkAmount)
        {
            GlobalGameManager.Player.m_sobriety -= drinkAmount;
            GlobalGameManager.Player.m_suspicion -= susAmount;
            Debug.Log($"Drink drank: -{drinkAmount} sobriety, -{susAmount} suspicion");
        } else
        {
            GlobalGameManager.Player.m_sobriety = 0;
            GlobalGameManager.Player.m_state = PlayerSpecialState.Drunk;
            Debug.Log("Player Drunk! Game Over");
        }
        
        
    }

    private void InitializeChips()
    {
        Player.m_chips[10] = new PokerChip{m_amount = 2, m_value = 10};
        Player.m_chips[20] = new PokerChip{m_amount = 0, m_value = 20};
        Player.m_chips[50] = new PokerChip{m_amount = 0, m_value = 50};
        Player.m_chips[100] = new PokerChip{m_amount = 0, m_value = 100};
        Player.m_chips[250] = new PokerChip{m_amount = 0, m_value = 250};
        Player.m_chips[500] = new PokerChip{m_amount = 0, m_value = 500};
        Player.m_chips[700] = new PokerChip{m_amount = 0, m_value = 700};
        Player.m_chips[1_000] = new PokerChip{m_amount = 0, m_value = 1_000};
        Player.m_chips[1_500] = new PokerChip{m_amount = 0, m_value = 1_500};   
        Player.m_chips[3_000] = new PokerChip{m_amount = 0, m_value = 3_000};
        Player.m_chips[4_000] = new PokerChip{m_amount = 0, m_value = 4_000};
        Player.m_chips[5_000] = new PokerChip{m_amount = 0, m_value = 5_000};
        Player.m_chips[10_000] = new PokerChip{m_amount = 0, m_value = 10_000};
        Player.m_chips[50_000] = new PokerChip{m_amount = 0, m_value = 50_000};
        Player.m_chips[100_000] = new PokerChip{m_amount = 0, m_value = 100_000};

    }


    private void UpdateHoverList()
    {
        if (HoveredObjects.Count > 0 && CursorState == CursorState.UP && !Instance.DisableChipHover)
        {   
            if (HoveredObjects[HoveredObjects.Count - 1] != HoveredObject)
            {
                if (HoveredObject != null)
                    HoveredObject.GetComponent<UIButton>().SetOutlineState(false);
                    
                
                HoveredObject = HoveredObjects[HoveredObjects.Count - 1];
                HoveringObject = true;


                HoveredObject.GetComponent<UIButton>().SetOutlineState(true);

            }
        }
        else
        {
            if (HoveredObject != null)
                HoveredObject.GetComponent<UIButton>().SetOutlineState(false);
                
            HoveredObject = null;
            HoveringObject = false;
        }
    }

    

    public void AddButtonToQueue(GameObject obj)
    {
        if (!HoveredObjects.Contains(obj))
            HoveredObjects.Add(obj);
    }

    public void RemoveButtonFromQueue(GameObject obj)
    {
        HoveredObjects.Remove(obj);
    }

    public void UpdateSuspicionBy(int amount)
    {
        if (Player.m_suspicion + amount >= 100)
        {
            Player.m_suspicion = 100;
            Player.m_state = PlayerSpecialState.Criminal;
            Debug.Log("Player Caught! Game Over");
        } else if (Player.m_suspicion + amount < 0)
        {
            Player.m_suspicion = 0;
        } else
        {
            Player.m_suspicion += amount;
        }
        
    }

    public void UpdateSobriety()
    {
        if (Instance.DrinkTimer <= 0 && Player.m_sobriety < 100)
        {
            Player.m_sobriety += Time.deltaTime / 2; //Regen 1 sobriety every 2 sec
        }

        if (Player.m_sobriety > 100)
        {
            Player.m_sobriety = 100;
        }
    }

    public void UpdateSuspicion()
    {
        if (Player.m_suspicion > 0)
        {
            Player.m_suspicion -= Time.deltaTime / 5; //Naturally lose 1 sus every 5 sec
        }

        if (Player.m_suspicion < 0)
        {
            Player.m_suspicion = 0;
        }
    }

    public void OnGameBack()
    {
        
    }

    public void PlayAgain()
    {
        var blackJack = FindObjectOfType<BlackJack>();
        if (blackJack != null) blackJack.PlayAgain();
    }

    public void Hit()
    {
        var blackJack = FindObjectOfType<BlackJack>();
        if (blackJack != null) blackJack.Hit();
    }

    public void Stand()
    {
        var blackJack = FindObjectOfType<BlackJack>();
        if (blackJack != null) blackJack.Stand();
    }

    public void Quit()
    {
        var blackJack = FindObjectOfType<BlackJack>();
        if (blackJack != null) blackJack.Quit();
    }

    public void FlipHiddenCard()
    {
        var blackJack = FindObjectOfType<BlackJack>();
        if (blackJack != null) blackJack.FlipHiddenCard();
    }

    public void PlaceBet()
    {
        var blackJack = FindObjectOfType<BlackJack>();
        if (blackJack != null) blackJack.PlaceBet();
    }

    // accepts the SceneSwap component directly from the button that was clicked
    public void OnUIButtonSceneChange(SceneSwap swapData)
    {
        if (swapData == null) return;

        // If heading back to the roaming casino, reset the game state
        if (swapData.ToScene == GameScenes.Default)
        {
            currentGame = CurrentGame.None;
        }

        int newScene = (int)swapData.ToScene;
        FadeOutObj.GetComponent<FadeOutFuncs>().animator.SetInteger("SceneID", newScene);
        FadeOutObj.GetComponent<FadeOutFuncs>().animator.SetBool("SwitchScene", true);
    }

    public void SwitchScene()
    {
        
    }

    public void OnSelectNewGameScene()
    {
        int newScene = (int)HoveredObject.GetComponent<SceneSwap>().ToScene;
        FadeOutObj.GetComponent<FadeOutFuncs>().animator.SetInteger("SceneID", newScene);
        FadeOutObj.GetComponent<FadeOutFuncs>().animator.SetBool("SwitchScene", true);
        
    }
    

}

public enum PlayerSpecialState
{
    None,
    Drunk,
    Criminal,
    Broke
}


public class PlayerData
{
    public Dictionary<int, PokerChip> m_chips = new Dictionary<int, PokerChip>();
    public float m_money = 200; //Directly replated to chips
    public float m_suspicion = 0; //0%-100%
    //AKA Accuracy of powers
    public float m_sobriety = 100; //0%-100%

    //Do we make the game over if you are fully drunk?
    public PlayerSpecialState m_state = PlayerSpecialState.None;


    public bool m_usingCheats = false;

    //Refeshes chip count based on money
    public void RefreshChipCount()
    {
        ChipType[] chipTypes = (ChipType[])System.Enum.GetValues(typeof(ChipType));
        float amount = m_money;

        for (int i = chipTypes.Length - 1; i >= 0; i--)
        {
            int chipValue = (int)chipTypes[i];
            int chipsOfThisType = 0;

            while (amount >= chipValue)
            {
                amount -= chipValue;
                chipsOfThisType++;
            }

            m_chips[chipValue].m_amount = chipsOfThisType;
        }
    }
}


public class PokerChip
{
    public int m_amount = 0;
    public int m_value = 20;
}


public enum CursorState
{
    UP,
    DOWN
}


