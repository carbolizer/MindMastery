using UnityEngine;

public class DragScript : MonoBehaviour
{
    private Vector3 ClickStartOffset = new();
    [SerializeField]
    private bool m_Grabbed = false;

    void Start()
    {
        
    }

    void Update()
    {
        switch (GlobalGameManager.Instance.CursorState)
        {
            case CursorState.DOWN:
            if (m_Grabbed)
            {
                transform.position = GlobalGameManager.Instance.CursorPos - ClickStartOffset;
            }
            break;

            case CursorState.UP:
            OnDragEnd();
            break;
        }
    }

    public void OnDragStart()
    {
        m_Grabbed = true;
        ClickStartOffset = GlobalGameManager.Instance.CursorPos - transform.position;  
    }

    public void OnDragEnd()
    {
        m_Grabbed = false;
        ClickStartOffset = Vector2.zero;  
    }

}
