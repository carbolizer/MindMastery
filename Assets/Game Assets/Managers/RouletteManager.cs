using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RouletteManager : MonoBehaviour
{

    public static RouletteManager Instance { get; private set; }
    [SerializeField]
    private GameObject TableHB;

    public List<GameObject> WinningCells = new List<GameObject>();

    


    public GameObject[] Cells;
    public List<GameObject> ExtraCells;
    public Dictionary<AssociatedNumberMode, RouletteTableEntry> RouletteTable = new Dictionary<AssociatedNumberMode, RouletteTableEntry>();
    public Sprite MindOnSpr;
    public Sprite MindOffSpr;
    public GameObject powerBtn;
    public bool poswersOn = false;
    public List<GameObject> labels = new List<GameObject>();
    public bool UsedPowersThisRound = false;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Cells = new GameObject[transform.childCount];
            
            for (int i = 0; i < transform.childCount; i++)
            {
                for (int j = 0; j < transform.transform.childCount; j++)
                {
                    if (int.Parse(transform.GetChild(j).gameObject.name) == i)
                    {
                        Cells[i] = transform.GetChild(j).gameObject;
                        break;
                    }
                }
            }
        }
    }

    void Update()
    {

        if (poswersOn)
        {
            UsedPowersThisRound = true;
        }
        foreach (var obj in labels)
        {
            
            var objLabel = obj.GetComponent<LabelData>();
            if (objLabel.animator == null) return;
            if (poswersOn && objLabel.DoesWin)
            {
                objLabel.GetComponent<TextMeshPro>().color = Color.purple;
            } else
            {
                objLabel.GetComponent<TextMeshPro>().color = Color.white;
            }
            if (objLabel.animator != null)
            {
                //objLabel.animator.SetBool("ShouldBlink", poswersOn && objLabel.DoesWin);
            }
        }
        
    }
    public void RefreshLabelWinStates()
    {
        foreach (var obj in labels)
        {
            if (obj == null) continue;

            var label = obj.GetComponent<LabelData>();
            if (label == null) continue;

            var rc = obj.GetComponent<RouletteCell>();

            bool wins = false;

            if (rc != null)
            {
                int index = (int)rc.Cell;

                if (index < 37)
                {
                    if (index >= 0 && index < Cells.Length && Cells[index] != null)
                        wins = Cells[index].GetComponent<RouletteCell>().doesWinNext;
                }
                else
                {
                    var mode = (AssociatedNumberMode)(index - 37);
                    wins = RouletteTable.TryGetValue(mode, out var entry) && entry.m_wonNextRoll;
                }
            }

            label.DoesWin = wins;
        }
    }

    public void OnToggleMind()
    {
        UsedPowersThisRound = true;
        poswersOn = !poswersOn;
        if (poswersOn)
        {
            powerBtn.GetComponent<SpriteRenderer>().sprite = MindOnSpr;
        } else
        {
            powerBtn.GetComponent<SpriteRenderer>().sprite = MindOffSpr;
        }
        

    }

    void Start()
    {
        powerBtn = GameObject.Find("MindBtn");
        GlobalGameManager.Instance.currentGame = CurrentGame.Roulette;
    }

}
