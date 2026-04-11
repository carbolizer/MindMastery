using System;
using UnityEngine;
using UnityEngine.Events;

public class UIButton : MonoBehaviour
{
    public Color m_outlineColor = Color.white;
    //Assign a function from a different script here
    [SerializeField]
    public UnityEvent ClickFunction;

    void Start()
    {
        var outline = gameObject.AddComponent<Outline>();
        outline.enabled = false;
        outline.OutlineColor = m_outlineColor;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = 10;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "UIHB")
            GlobalGameManager.Instance.AddButtonToQueue(gameObject);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "UIHB")
            GlobalGameManager.Instance.RemoveButtonFromQueue(gameObject);
    }

    //gets called on button clicked
    public void OnClick()
    {
        ClickFunction?.Invoke();
    }
}
