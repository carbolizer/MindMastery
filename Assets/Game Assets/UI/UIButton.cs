using System;
using UnityEngine;
using UnityEngine.Events;


public enum OutlineUpdateState
{
    UpdateComplete,
    NeedsUpdate
}

public enum SpriteUpdateState
{
    UpdateComplete,
    NeedsUpdate
}



public class UIButton : MonoBehaviour
{
    private bool m_hovered = false;
    [SerializeField]
    private Color m_outlineColor = Color.white;
    [SerializeField]
    private float m_outlineWidth = 15;

    private OutlineUpdateState m_outlineUpdateState = OutlineUpdateState.UpdateComplete;
    private SpriteUpdateState m_spriteUpdateState = SpriteUpdateState.UpdateComplete;

    //Assign a function from a different script here
    [SerializeField]
    public UnityEvent ClickFunction;

    void Start()
    {
        //0.125
        GetComponent<SpriteRenderer>().material.SetColor("_SolidOutline", m_outlineColor);
        GetComponent<SpriteRenderer>().material.SetFloat("_Thickness", m_outlineWidth / ((transform.localScale.x + transform.localScale.y) / 2));
    }

    void Update()
    {
        switch (m_outlineUpdateState)
        {
            case OutlineUpdateState.NeedsUpdate:
                if (m_hovered)
                {
                    //set outline true
                    GetComponent<SpriteRenderer>().material.SetFloat("_OutlineEnabled", 1.0f);
                    
                } else
                {
                    //set outline false
                    GetComponent<SpriteRenderer>().material.SetFloat("_OutlineEnabled", 0.0f);
                }
                m_outlineUpdateState = OutlineUpdateState.UpdateComplete;
                break;
            
            case OutlineUpdateState.UpdateComplete:
                //No outline update needed

                break;
        }

        switch (m_spriteUpdateState)
        {
            case SpriteUpdateState.NeedsUpdate:
                //Swap sprite

                m_spriteUpdateState = SpriteUpdateState.UpdateComplete;
                break;

            case SpriteUpdateState.UpdateComplete:
                //Do Nothing

                break;
        }
    }
    
    public void SetOutlineState(bool val)
    {
        
        m_hovered = val;
        m_outlineUpdateState = OutlineUpdateState.NeedsUpdate;
        
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "UIHB") //UI HitBox
            GlobalGameManager.Instance.AddButtonToQueue(gameObject);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "UIHB") //UI HitBox
            GlobalGameManager.Instance.RemoveButtonFromQueue(gameObject);
    }

    //gets called on button clicked
    public void OnClick()
    {
        ClickFunction?.Invoke();
    }
}
