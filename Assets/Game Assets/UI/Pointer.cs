using UnityEditor;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    private SpriteRenderer sr;
    private GameObject Label;

    [SerializeField]
    private Sprite offSpr;
    [SerializeField]
    private Sprite onSpr;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = offSpr;
        Label = transform.GetChild(0).gameObject;
    }

    public void ClickDown()
    {
        if (onSpr != null && sr != null)
            sr.sprite = onSpr;
    }

    public void ClickRelease()
    {
        if (offSpr != null && sr != null)
            sr.sprite = offSpr;
    }

    void Update()
    {
        if (GlobalGameManager.Instance.HoveredObject != null && GlobalGameManager.Instance.HoveredObject.TryGetComponent(out Tooltip tooltip))
        {  
            Label.GetComponent<TMPro.TextMeshPro>().text = tooltip.m_tooltip;
            Label.GetComponent<TMPro.TextMeshPro>().color = tooltip.m_baseColor;
            Label.GetComponent<TMPro.TextMeshPro>().outlineColor = tooltip.m_outlineColor;
            Label.SetActive(true);
        } else
        {
            Label.SetActive(false);
        }
    }
}
