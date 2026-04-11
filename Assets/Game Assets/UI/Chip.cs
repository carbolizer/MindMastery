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
    public GameObject ClosestTile = null;

    public ChipState m_state = ChipState.Lazy;

    public bool m_overBox = true;
    public bool m_onTable = false;
    public bool m_hovered = false;

    public Vector3 m_parentOffset;
    public GameObject m_posParent;

    private Vector3 ClickStartOffset = new();

    public ChipType m_value = ChipType.Ten;


    void Start()
    {
        RefreshValue();
        //Debug.Log("test");
        m_state = ChipState.Lazy;
        GetComponent<UIButton>().ClickFunction.AddListener(OnDragStart);
    }



    void RefreshValue()
    {
        //Pick a new sprite based on value
        var sr = GetComponent<SpriteRenderer>();

        switch (m_value)
        {
            case ChipType.Ten:
                sr.color = Color.white;
                break;
            case ChipType.Twenty:
                sr.color = Color.blue;
                break;
            case ChipType.Fifty:
                sr.color = Color.red;
                break;
            case ChipType.OneHundered:
                sr.color = Color.yellow;
                break;
            case ChipType.TwoHunderedFifty:
                sr.color = Color.yellowGreen;
                break;
            case ChipType.FiveHundered:
                sr.color = Color.orange;
                break;
            case ChipType.SevenHundered:
                sr.color = Color.purple;
                break;
            case ChipType.OneThousand:
                sr.color = Color.teal;
                break;
            case ChipType.OneThousandFiveHundered:
                sr.color = Color.tan;
                break;
            case ChipType.ThreeThousand:
                sr.color = Color.violet;
                break;
            case ChipType.FourThousand:
                sr.color = Color.navyBlue;
                break;
            case ChipType.FiveThousand:
                sr.color = Color.brown;
                break;
            case ChipType.TenThousand:
                sr.color = Color.cornsilk;
                break;
            case ChipType.FiftyThousand:
                sr.color = Color.aquamarine;
                break;
            case ChipType.OneHunderedThousand:
                sr.color = Color.black;
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
                    ClickStartOffset = Vector2.zero; 
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
                } else
                {
                    transform.position = m_parentOffset + m_posParent.transform.position;
                }
                break;

                case ChipState.Held:
                transform.position = GlobalGameManager.Instance.CursorPos - ClickStartOffset;
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

    public void OnDragStart()
    {
        m_state = ChipState.Held;
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
