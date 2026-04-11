using UnityEngine;

public class Pointer : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField]
    private Sprite offSpr;
    [SerializeField]
    private Sprite onSpr;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = offSpr;
    }

    public void ClickDown()
    {
        sr.sprite = onSpr;
    }

    public void ClickRelease()
    {
        sr.sprite = offSpr;
    }
}
