using System.Collections.Generic;
using UnityEngine;

public class ChipBoxInteraction : MonoBehaviour
{
    [SerializeField]
    private GameObject ChipPrefab; //Len 15
    //3 rows of 5, (8, 5) 
    

    
    void Start()
    {
        float xSpacing = 8 / 3;
        float ySpacing = 5 / 5;

        ChipType[] chipTypes = (ChipType[])System.Enum.GetValues(typeof(ChipType));
        var parent = transform.GetChild(0);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                //Place Chips (8, 5)
                int index = j + (i * 5);
                GameObject obj = Instantiate(ChipPrefab);
                obj.GetComponent<Chip>().m_posParent = parent.gameObject;
                obj.GetComponent<Chip>().m_value = chipTypes[index];
                obj.GetComponent<Chip>().m_parentOffset = new Vector3(j * xSpacing, i * ySpacing, -24);
                
                obj.transform.position = new Vector3(j * xSpacing, i * ySpacing, -24);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
