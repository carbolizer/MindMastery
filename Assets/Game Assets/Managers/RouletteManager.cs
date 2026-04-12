using System.Collections.Generic;
using UnityEngine;

public class RouletteManager : MonoBehaviour
{

    public static RouletteManager Instance { get; private set; }
    [SerializeField]
    private GameObject TableHB;

    


    public GameObject[] Cells;
    public List<GameObject> ExtraCells;

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

    void Start()
    {
        GlobalGameManager.Instance.currentGame = CurrentGame.Roulette;
    }

}
