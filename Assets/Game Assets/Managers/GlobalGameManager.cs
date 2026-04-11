using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;




public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance { get; private set; }
    public static PlayerData Player;
    public Vector3 CursorPos;
    public CursorState CursorState;



    public GameObject HoveredObject = null;
    public bool HoveringObject = false;
    //Internal logic use only; used to determine HoveredObject
    [SerializeField]
    private List<GameObject> HoveredObjects = new List<GameObject>();



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
        UpdateHoverList();
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
        if (HoveredObjects.Count > 0 && CursorState == CursorState.UP)
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


    

}



public class PlayerData
{
    public Dictionary<int, PokerChip> m_chips = new Dictionary<int, PokerChip>();
    public float m_suspicion = 0; //0%-100%
    //AKA Accuracy of powers
    public float m_sobriety = 100; //0%-100%


    public bool m_usingCheats = false;
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
