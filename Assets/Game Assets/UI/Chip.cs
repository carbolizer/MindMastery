using UnityEngine;

public enum ChipState
{
    Lazy, //In box (new)
    Held, //Out of box (held)
    OnTable,
    DeletionScheduled

}
public enum ChipType
{
    Ten = 10,
    Twenty = 20,
    Fifty = 50,
    OneHundered = 100,
    TwoHunderedFifty = 250,
    FiveHundered = 500,
    SevenHundered = 700,
    OneThousand = 1_000,
    OneThousandFiveHundered = 1_500,
    ThreeThousand = 3_000,
    FourThousand = 4_000,
    FiveThousand = 5_000,
    TenThousand = 10_000,
    FiftyThousand = 50_000,
    OneHunderedThousand = 100_000

}

public class Chip : MonoBehaviour
{   
    [SerializeField]
    private GameObject ClosestTile = null;

    public ChipState m_state = ChipState.Lazy;

    public bool m_overBox = true;
    public bool m_onTable = false;
    public bool m_hovered = false;

    public Vector3 m_parentOffset;
    public GameObject m_posParent;
    public ChipType m_value = ChipType.Ten;


    void Start()
    {
        RefreshValue();
        Debug.Log("test");
        m_state = ChipState.Lazy;
    }



    void RefreshValue()
    {
        //Pick a new sprite based on value
        var SRC = GetComponent<SpriteRenderer>().color;
        switch (m_value)
        {
            case ChipType.Ten:
            SRC = Color.white;
            break;
            case ChipType.Twenty:
            SRC = Color.blue;
            break;
            case ChipType.Fifty:
            SRC = Color.red;
            break;
            case ChipType.OneHundered:
            SRC = Color.yellow;
            break;
            case ChipType.TwoHunderedFifty:
            SRC = Color.yellowGreen;
            break;
            case ChipType.FiveHundered:
            SRC = Color.orange;
            break;
            case ChipType.SevenHundered:
            SRC = Color.purple;
            break;
            case ChipType.OneThousand:
            SRC = Color.teal;
            break;
            case ChipType.OneThousandFiveHundered:
            SRC = Color.tan;
            break;
            case ChipType.ThreeThousand:
            SRC = Color.violet;
            break;
            case ChipType.FourThousand:
            SRC = Color.navyBlue;
            break;
            case ChipType.FiveThousand:
            SRC = Color.brown;
            break;
            case ChipType.TenThousand:
            SRC = Color.cornsilk;
            break;
            case ChipType.FiftyThousand:
            SRC = Color.aquamarine;
            break;
            case ChipType.OneHunderedThousand:
            SRC = Color.black;
            break;

        }
    }

    

    void Update()
    {   

        if (GlobalGameManager.Instance.HoveredObject == gameObject)
        {
            m_hovered = true;
        } else
        {
            m_hovered = false;
        }
        

        if (m_state == ChipState.DeletionScheduled)
        {
            if (m_overBox)
            {
                GlobalGameManager.Player.m_chips[(int)m_value].m_amount += 1;
                Destroy(gameObject);
            }
        }


        if (GlobalGameManager.Instance.CursorState == CursorState.UP)
        {
            switch (m_state)
            {
                case ChipState.Lazy:
                transform.position = m_parentOffset + m_posParent.transform.position;
                break;

                case ChipState.Held:
                if (m_overBox)
                {
                    m_state = ChipState.DeletionScheduled;
                } else
                {
                    m_state = ChipState.OnTable;
                }
                break;

            }
        } else
        {
            switch (m_state)
            {
                case ChipState.Lazy:
                if (m_hovered)
                {
                    m_state = ChipState.Held;
                    GlobalGameManager.Player.m_chips[(int)m_value].m_amount -= 1;
                }
                break;

                case ChipState.OnTable:
                if (m_hovered)
                {
                    m_state = ChipState.Held;
                }
                break;
            }
        }
        
          
        

        //Update box chips here

        if (m_onTable)
        {
            ClosestTile = null;
            foreach (var cell in RouletteManager.Instance.Cells)
            {
                Rect tileRect = new Rect(
                    new Vector2(cell.transform.position.x - cell.transform.localScale.x / 2, cell.transform.position.y - cell.transform.localScale.y / 2),
                    new Vector2(cell.transform.localScale.x, cell.transform.localScale.y)
                );
                if (tileRect.Contains(transform.position))
                {
                   ClosestTile = cell;
                }
                
            }

            foreach (var cell in RouletteManager.Instance.ExtraCells)
            {
                Rect tileRect = new Rect(
                    new Vector2(cell.transform.position.x - cell.transform.localScale.x / 2, cell.transform.position.y - cell.transform.localScale.y / 2),
                    new Vector2(cell.transform.localScale.x, cell.transform.localScale.y)
                );
                if (tileRect.Contains(transform.position))
                {
                   ClosestTile = cell;
                }
                
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.gameObject.name == "TableHB")
        {
            RouletteManager.Instance.ChipsOnTable.Add(gameObject);
            m_onTable = true;
        }
        if (collision.gameObject.name == "ChipBox")
        {
            m_overBox = true;
        }
    }

    

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "TableHB")
        {
            RouletteManager.Instance.ChipsOnTable.Remove(gameObject);
            m_onTable = false;
            ClosestTile = null;
            
        }
        if (collision.gameObject.name == "ChipBox")
        {
            m_overBox = false;
        }
    }
}
