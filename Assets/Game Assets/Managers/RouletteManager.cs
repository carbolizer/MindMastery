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
        foreach (var obj in labels)
        {
            
            var objLabel = obj.GetComponent<LabelData>();
            if (objLabel.animator == null) return;
            if (poswersOn && objLabel.DoesWin)
            {
                objLabel.GetComponent<TextMeshPro>().color = Color.purple;
                objLabel.animator.SetBool("ShouldBlink", true);
            } else
            {
                objLabel.GetComponent<TextMeshPro>().color = Color.white;
                objLabel.animator.SetBool("ShouldBlink", false);
            }
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
