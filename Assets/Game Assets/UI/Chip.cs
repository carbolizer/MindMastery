using UnityEngine;

public class Chip : MonoBehaviour
{   
    [SerializeField]
    private GameObject ClosestTile = null;
    private bool m_onTable = false;
    void Start()
    {
        
    }

    
    void Update()
    {
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
    }

    

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "TableHB")
        {
            RouletteManager.Instance.ChipsOnTable.Remove(gameObject);
            m_onTable = false;
            ClosestTile = null;
            
        }
    }
}
