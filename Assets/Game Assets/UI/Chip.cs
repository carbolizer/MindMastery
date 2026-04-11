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
            float dist = float.MaxValue;
            foreach (var cell in RouletteManager.Instance.Cells)
            {
                float newDist = Vector3.Distance(cell.transform.position, transform.position);
                if (newDist < dist)
                {
                   dist = newDist;
                   ClosestTile = cell;
                }
                
            }

            foreach (var cell in RouletteManager.Instance.ExtraCells)
            {
                float newDist = Vector3.Distance(cell.transform.position, transform.position);
                if (newDist < dist)
                {
                   dist = newDist;
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
        }
    }
}
