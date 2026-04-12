using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ChipPosition
{
    public Vector2 position;
    public ChipType chipType;
}


public class ChipBoxInteraction : MonoBehaviour
{
    [SerializeField]
    private GameObject ChipPrefab; 
    public List<ChipPosition> chipPositions = new();
    private Dictionary<int, GameObject> spawnedChips = new();
    
    void RefreshChips()
    {
        foreach (var chip in GlobalGameManager.Player.m_chips)
        {
           //Debug.Log(chip.Value.m_amount);
        }

        ChipType[] chipTypes = (ChipType[])System.Enum.GetValues(typeof(ChipType));
        var parent = transform.GetChild(0);

        for (int i = 0; i < chipPositions.Count; i++)
        {
            var item = chipPositions[i];

            //Skip if already spawned
            if (spawnedChips.ContainsKey(i))
            {
                var existing = spawnedChips[i];

                
                if (existing == null || !existing.GetComponent<Chip>().m_overBox)
                {
                    spawnedChips.Remove(i);
                }
                else
                {
                    continue;
                }
            }

            var chipData = GlobalGameManager.Player.m_chips[(int)item.chipType];
            

            if (chipData.m_amount > 0)
            {
                Vector3 pos = new Vector3(item.position.x, item.position.y + 4, -24);

                GameObject obj = Instantiate(ChipPrefab, pos, Quaternion.identity);
                obj.GetComponent<SpriteRenderer>().enabled = false;
                obj.transform.parent = GameObject.Find("ChipsContainer").transform;
                var chipComponent = obj.GetComponent<Chip>();
                chipComponent.m_posParent = parent.gameObject;
                chipComponent.m_value = item.chipType;
                chipComponent.m_parentOffset = pos;
                chipData.m_amount -=  1;

                spawnedChips[i] = obj;
                
     
            }
        }
    }
    
    void Start()
    {
        RefreshChips();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshChips();
    }
}
//(-1.5, -3) (-.75, -3) (0, -3) (.75, -3) (1.5, -3) Top
//(-1.5, -3.75) (-.75, -3.75) (0, -3.75) (.75, 3.75) (1.5, 3.75) Mid
//(-1.5, -4.5) (-.75, -4.5) (0, -4.5) (.75, -3) (1.5, -4.5) Bot