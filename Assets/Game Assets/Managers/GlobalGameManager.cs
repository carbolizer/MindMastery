using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance { get; private set; }
    public static PlayerData Player;
    public Vector3 CursorPos;
    public CursorState CursorState;



    public GameObject HoveredObject = null;
    public bool HoveringObject = false;
    public List<GameObject> HoveredObjects = new List<GameObject>();

    private void UpdateHoverList()
    {
        if (HoveredObjects.Count > 0)
        {   
            if (HoveredObjects[HoveredObjects.Count - 1] != HoveredObject)
            {
                if (HoveredObject != null)
                    HoveredObject.GetComponent<Outline>().enabled = false;
                

                HoveredObject = HoveredObjects[HoveredObjects.Count - 1];

                HoveredObject.GetComponent<Outline>().enabled = true;
                HoveringObject = true;
                
            }
        }
        else
        {
            if (HoveredObject != null)
                HoveredObject.GetComponent<Outline>().enabled = false;
            HoveredObject = null;
            HoveringObject = false;
        }
    }

    

    public void AddButtonToQueue(GameObject obj)
    {
        if (!HoveredObjects.Contains(obj))
            HoveredObjects.Add(obj);
    }

    public void RemoveButtonFromQueue(GameObject obj)
    {
        HoveredObjects.Remove(obj);
    }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Player = new();
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        UpdateHoverList();
    }

}



public class PlayerData
{
    public float m_cash = 0;
    public float m_suspicion = 0;
    public float m_sobriety = 100;




}

public enum CursorState
{
    UP,
    DOWN
}
