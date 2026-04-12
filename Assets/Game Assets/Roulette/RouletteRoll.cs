using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RouletteCellType
{
    Zero,

    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Eleven,
    Twelve,

    Thirteen,
    Fourteen,
    Fifteen,
    Sixteen,
    Seventeen,
    Eightteen,
    Nineteen,
    Twenty,
    TwentyOne,
    TwentyTwo,
    TwentyThree,
    TwentyFour,

    TwentyFive,
    TwentySix,
    TwentySeven,
    TwentyEight,
    TwentyNine,
    Thirty,
    ThirtyOne,
    ThirtyTwo,
    ThirtyThree,
    ThirtyFour,
    ThirtyFive,
    ThirtySix,

    Column1,
    Column2,
    Column3,

    Dozen1,
    Dozen2,
    Dozen3,

    Low,
    High,

    Red,
    Black,

    Even,
    Odd

    


}

public enum WheelStates
{
    Stopped,
    Spinning,
    DisplayingResults,
    Awarding
}

//Each rotation is offset by 9.72 deg
//Start offset is 4.86 deg


public class RouletteRoll : MonoBehaviour
{
    public const int RouletteNums = 36;
    public float bet;
    
    private int lastWin;
    public bool rolling = false;
    private Animator anim;


    //Number Adjacency unused
    //   -2  +1 +4
    //     \ | /
    //-3 -      - +3
    //     / | \
    //   -4 -1  +2

    public Dictionary<AssociatedNumberMode, RouletteTableEntry> RouletteTable = new Dictionary<AssociatedNumberMode, RouletteTableEntry>();

    
    
    
    void Start()
    {
        RouletteTable[AssociatedNumberMode.Low] = new RouletteTableEntry{m_name = "Low 1-18", m_payoutMultiplier = 1, m_mode = AssociatedNumberMode.Low};
        RouletteTable[AssociatedNumberMode.High] = new RouletteTableEntry{m_name = "High 19-36", m_payoutMultiplier = 1, m_mode = AssociatedNumberMode.High};

        RouletteTable[AssociatedNumberMode.Red] = new RouletteTableEntry{m_name = "Red", m_payoutMultiplier = 1, m_mode = AssociatedNumberMode.Red, m_associatedNumbers = new int[] {1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36}};
        RouletteTable[AssociatedNumberMode.Black] = new RouletteTableEntry{m_name = "Black", m_payoutMultiplier = 1, m_mode = AssociatedNumberMode.Black, m_associatedNumbers = new int[] {2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35}};

        RouletteTable[AssociatedNumberMode.Odd] = new RouletteTableEntry{m_name = "Odd", m_payoutMultiplier = 1, m_mode = AssociatedNumberMode.Odd};
        RouletteTable[AssociatedNumberMode.Even] = new RouletteTableEntry{m_name = "Even", m_payoutMultiplier = 1, m_mode = AssociatedNumberMode.Even};

        RouletteTable[AssociatedNumberMode.Dozen1] = new RouletteTableEntry{m_name = "Dozen1", m_payoutMultiplier = 3, m_mode = AssociatedNumberMode.Dozen1, m_customMultiplier = 1};
        RouletteTable[AssociatedNumberMode.Dozen2] = new RouletteTableEntry{m_name = "Dozen2", m_payoutMultiplier = 3, m_mode = AssociatedNumberMode.Dozen2, m_customMultiplier = 2};
        RouletteTable[AssociatedNumberMode.Dozen3] = new RouletteTableEntry{m_name = "Dozen3", m_payoutMultiplier = 3, m_mode = AssociatedNumberMode.Dozen3, m_customMultiplier = 3};

        RouletteTable[AssociatedNumberMode.Column1] = new RouletteTableEntry{m_name = "Column1", m_payoutMultiplier = 2, m_mode = AssociatedNumberMode.Column1, m_customMultiplier = 1};
        RouletteTable[AssociatedNumberMode.Column2] = new RouletteTableEntry{m_name = "Column2", m_payoutMultiplier = 2, m_mode = AssociatedNumberMode.Column2, m_customMultiplier = 2};
        RouletteTable[AssociatedNumberMode.Column3] = new RouletteTableEntry{m_name = "Column3", m_payoutMultiplier = 2, m_mode = AssociatedNumberMode.Column3, m_customMultiplier = 3};

        RouletteTable[AssociatedNumberMode.Single] = new RouletteTableEntry{m_name = "Single", m_payoutMultiplier = 35, m_mode = AssociatedNumberMode.Single};


        RouletteManager.Instance.RouletteTable = RouletteTable;
        anim = GetComponent<Animator>();
        PreRoll();
    }


    public void PreRoll()
    {
        RouletteManager.Instance.WinningCells.Clear();

        
        lastWin = UnityEngine.Random.Range(0, RouletteNums);

        for (int i = 0; i < 11; i++)
        {
            RouletteTable[(AssociatedNumberMode)i].m_wonNextRoll = RouletteTable[(AssociatedNumberMode)i].DidWin(lastWin);
        } 

        foreach (var obj in RouletteManager.Instance.Cells)
        {
            obj.GetComponent<SpriteRenderer>().enabled = false;


            var RC = obj.GetComponent<RouletteCell>();

            if ((int)obj.GetComponent<RouletteCell>().Cell == lastWin)
            {
                
                RouletteManager.Instance.WinningCells.Add(obj);
                RC.doesWinNext = true;

                //Debug
                obj.GetComponent<SpriteRenderer>().color = Color.blue;
                obj.GetComponent<SpriteRenderer>().enabled = true;

            } else
            {
                
                RC.doesWinNext = false;

                //Debug
                obj.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        foreach (var obj in RouletteManager.Instance.ExtraCells)
        {
            obj.GetComponent<SpriteRenderer>().enabled = false;


            var RC = obj.GetComponent<RouletteCell>();

            if (RouletteTable[(AssociatedNumberMode)(int)RC.Cell - 37].m_wonNextRoll)
            {
                RouletteManager.Instance.WinningCells.Add(obj);
                RC.doesWinNext = true;

                //Debug
                obj.GetComponent<SpriteRenderer>().color = Color.blue;
                obj.GetComponent<SpriteRenderer>().enabled = true;
            } else
            {
                obj.GetComponent<SpriteRenderer>().color = Color.white;
                RC.doesWinNext = false;
            }
            
        }
        
        //Debug.Log(lastWin);

    }

    void Update()
    {
        if (GlobalGameManager.Instance.ChipsOnTable.Count > 0 && !rolling)
        {
            GetComponent<CircleCollider2D>().enabled = true;
        } else
        {
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    public void OnFinishSpin()
    {
        GetComponent<CircleCollider2D>().enabled = true;
        rolling = false;
        anim.SetBool("StartSpin", false);

        bet = 0;
        //Animation

        

        float totalResult = 0;

        foreach (var chip in GlobalGameManager.Instance.ChipsOnTable)
        {
            int chipValue = (int)chip.GetComponent<Chip>().m_value;
            GameObject cell = chip.GetComponent<Chip>().ClosestTile;

            float multiplier = -1; // default = lose

            if (RouletteManager.Instance.Cells.Contains(cell))
            {
                if (int.Parse(cell.name) == lastWin)
                {
                    multiplier = 35; // single number payout
                }
            }

            foreach (var EC in RouletteManager.Instance.ExtraCells)
            {
                if (cell == EC)
                {
                    var entry = RouletteTable[(AssociatedNumberMode)
                        (int)EC.GetComponent<RouletteCell>().Cell - 37];

                    if (entry.m_wonNextRoll)
                        multiplier = entry.m_payoutMultiplier;
                }
            }

            totalResult += chipValue * multiplier;
        } 
        GlobalGameManager.Player.m_money += totalResult;
            
        GlobalGameManager.Instance.DestroyChipsOnTable();
        //if (didWin) GlobalGameManager.Player.m_money += bet * currentPrizeMultiplier;
        Debug.Log("Money after roll: " + GlobalGameManager.Player.m_money);
        GlobalGameManager.Player.RefreshChipCount();
        GlobalGameManager.Instance.DisableChipHover = false;

        //Award player then do a new preroll
        PreRoll();
    }

    //Run Preroll after rolling
    public void Roll()
    {
        if (!rolling)
        {
            GetComponent<CircleCollider2D>().enabled = false;
            rolling = true;
            anim.SetBool("StartSpin", true);
            GlobalGameManager.Instance.DisableChipHover = true;
            
            
        }
        
    }
}

//Spoof for testing
public class PlayerBet
{
    public AssociatedNumberMode m_betType = AssociatedNumberMode.Single;
    public int m_betValue = 0;
}

public enum AssociatedNumberMode
{
    Column1,
    Column2,
    Column3,

    Dozen1,
    Dozen2,
    Dozen3,

    Low,
    High,

    Red,
    Black,

    Even,
    Odd,
    Single
}

public class RouletteTableEntry
{
    public string m_name;
    public int m_payoutMultiplier;
    public AssociatedNumberMode m_mode;
    public int m_customMultiplier = 1; //Generic multiplier, used in columns
    public int[] m_associatedNumbers;
    public bool m_wonNextRoll = false;

    public bool DidWin(int num)
    {
        switch (m_mode)
        {   
            //Always wins
            case AssociatedNumberMode.Single:
            return true;

            case AssociatedNumberMode.Black:
            if (m_associatedNumbers.Contains(num))
            {
                return true;
            }
            break;

            case AssociatedNumberMode.Red:
            if (m_associatedNumbers.Contains(num))
            {
                return true;
            }
            break;

            case AssociatedNumberMode.Even:
            if (num % 2 == 0 && num != 0)
            {
                return true;
            }
            break;

            case AssociatedNumberMode.Odd:
            if (num % 2 != 0 && num != 0)
            {
                return true;
            }
            break;

            case AssociatedNumberMode.High:
            if (num >= 19 && num <= 36)
            {
                return true;
            }
            break;

            case AssociatedNumberMode.Low:
            if (num >= 1 && num <= 18)
            {
                return true;
            }
            break;

            case AssociatedNumberMode.Dozen1:
            case AssociatedNumberMode.Dozen2:
            case AssociatedNumberMode.Dozen3:
            if (num > 12 * (m_customMultiplier - 1) && num <= 12 * m_customMultiplier && num != 0)
            {
                return true;
            }
            break;

            case AssociatedNumberMode.Column1:
            case AssociatedNumberMode.Column2:
            case AssociatedNumberMode.Column3:
            for (int i = m_customMultiplier; i < 36; i += 3)
            {
                if (num == i)
                {
                    return true;
                }
            }
            break;
        }



        return false;;
    }

}