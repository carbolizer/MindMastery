using UnityEngine;

public enum ChipState
{
    Lazy, //In box (new)
    Held, //Out of box (held)
    OnTable, //On Table
    DeletionScheduled //To be deleted, after bet or put back in box

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
    private Material ColorMat;

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
        ColorMat = Instantiate(ColorMat);
        RefreshColorValue();
        m_state = ChipState.Lazy;
        GetComponent<UIButton>().ClickFunction.AddListener(OnDragStart);
        
    }



    void RefreshColorValue()
    {
        var mat1 = GetComponent<SpriteRenderer>().material;
        Color col = new Color();
        GetComponent<Tooltip>().m_tooltip = "$";

        switch (m_value)
        {
            case ChipType.Ten:
                col = Color.deepPink;
                GetComponent<Tooltip>().m_tooltip += "10";
                break;
            case ChipType.Twenty:
                col = Color.blue;
                GetComponent<Tooltip>().m_tooltip += "20";
                break;
            case ChipType.Fifty:
                col = Color.red;
                GetComponent<Tooltip>().m_tooltip += "50";
                break;
            case ChipType.OneHundered:
                col = Color.yellow;
                GetComponent<Tooltip>().m_tooltip += "100";
                break;
            case ChipType.TwoHunderedFifty:
                col = Color.yellowGreen;
                GetComponent<Tooltip>().m_tooltip += "250";
                break;
            case ChipType.FiveHundered:
                col = Color.orange;
                GetComponent<Tooltip>().m_tooltip += "500";
                break;
            case ChipType.SevenHundered:
                col = Color.purple;
                GetComponent<Tooltip>().m_tooltip += "700";
                break;
            case ChipType.OneThousand:
                col = Color.teal;
                GetComponent<Tooltip>().m_tooltip += "1K";
                break;
            case ChipType.OneThousandFiveHundered:
                col = Color.tan;
                GetComponent<Tooltip>().m_tooltip += "1.5K";
                break;
            case ChipType.ThreeThousand: 
                col = Color.violet;
                GetComponent<Tooltip>().m_tooltip += "3K";
                break;
            case ChipType.FourThousand:    
                col = Color.navyBlue;
                GetComponent<Tooltip>().m_tooltip += "4K";
                break;
            case ChipType.FiveThousand:
                col = Color.brown;
                GetComponent<Tooltip>().m_tooltip += "5K";
                break;
            case ChipType.TenThousand:
                col = Color.cornsilk;
                GetComponent<Tooltip>().m_tooltip += "10K";
                break;
            case ChipType.FiftyThousand:
                col = Color.aquamarine;
                GetComponent<Tooltip>().m_tooltip += "50K";
                break;
            case ChipType.OneHunderedThousand:
                col = Color.black;
                GetComponent<Tooltip>().m_tooltip += "100K";
                break;
        }
        GetComponent<Tooltip>().m_baseColor = col;
        ColorMat.SetColor("_Color", col);
        mat1.SetTexture("_MainTex", ColorMat.GetTexture("_MainTex"));
        GetComponent<SpriteRenderer>().materials = new Material[] { mat1, ColorMat };
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
                transform.position = new Vector3(
                    m_parentOffset.x + m_posParent.transform.position.x,
                    m_parentOffset.y + m_posParent.transform.position.y,
                    -25
                );
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
                    //GlobalGameManager.Player.m_chips[(int)m_value].m_amount -= 1;
                } else
                {
                    transform.position = new Vector3(
                        m_parentOffset.x + m_posParent.transform.position.x,
                        m_parentOffset.y + m_posParent.transform.position.y,
                        -25
                    );
                    
                }
                break;

                case ChipState.Held:
                transform.position = new Vector3(
                    GlobalGameManager.Instance.CursorPos.x - ClickStartOffset.x,
                    GlobalGameManager.Instance.CursorPos.y - ClickStartOffset.y,
                    -25
                );
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

            switch (GlobalGameManager.Instance.currentGame)
            {   
                case CurrentGame.None:
                //Roaming
                break;


                case CurrentGame.Roulette:

                //Loops through all tiles in the game (roulette) and sets ClosestTile to the one that the
                //Chip is on
                //To make a new one, if you dont have places that are crowded together, you can just use OnCollisionEnter/Exit2D
                //To set the Closest Tile
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
                break;
            }
            
        }
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void OnDragStart()
    {
        m_state = ChipState.Held;
    }

    //Table Must have a Collider2D marked as "Trigger", and a rigidbody with continuous collision
    //detection. you may freeze pos and rotation and disable gravity on it if it is static
    //Same for ChipBox, though you can just use the one i made across both games
    void OnTriggerEnter2D(Collider2D collision)
    {   
        if (!GlobalGameManager.Instance.DisableChipHover)
        {
            if (collision.gameObject.name == "TableHB")
            {
                GlobalGameManager.Instance.ChipsOnTable.Add(gameObject); 
                m_onTable = true;
            }
            if (collision.gameObject.name == "ChipBox")
            {
                m_overBox = true;
            }
        }
        
    }

    

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!GlobalGameManager.Instance.DisableChipHover)
        {
            if (collision.gameObject.name == "TableHB")
            {
                GlobalGameManager.Instance.ChipsOnTable.Remove(gameObject);
                m_onTable = false;
                ClosestTile = null;
                
            }
            if (collision.gameObject.name == "ChipBox")
            {
                m_overBox = false;
            }
        }
        
    }
}
