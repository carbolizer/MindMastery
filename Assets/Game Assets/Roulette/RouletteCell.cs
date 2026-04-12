using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RouletteCell : MonoBehaviour
{
    public RouletteCellType Cell;
    public bool doesWinNext = false;
    void Start()
    {   
        if (int.TryParse(gameObject.name, out int val))
        {
            Cell = (RouletteCellType)System.Enum.GetValues(typeof(RouletteCellType)).GetValue(val);
        }
        
    } 
}
