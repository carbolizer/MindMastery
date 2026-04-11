using UnityEngine;
using UnityEngine.InputSystem;





public class GenericUI : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private GameObject PointerPrefab;
    private GameObject AlivePP;


    void Start()
    {
        cam = Camera.main;
        AlivePP = Instantiate(PointerPrefab, transform);
        AlivePP.transform.position = new Vector3(transform.position.x - 0.9f, transform.position.y - 0.7f, transform.position.z);
        Cursor.visible = false;
        GlobalGameManager.Instance.CursorState = CursorState.UP;
    }

    void OnGUI()
    {
        Vector3 point = new();
        Event   currentEvent = Event.current;
        Vector2 mousePos = new Vector2
        {
            x = currentEvent.mousePosition.x,
            y = cam.pixelHeight - currentEvent.mousePosition.y
        };

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane)); 

        /*
        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + point.ToString("F3"));
        GUILayout.EndArea();
        */

        if (Mouse.current.leftButton.isPressed)
        {
            AlivePP.GetComponent<Pointer>().ClickDown();
            GlobalGameManager.Instance.CursorState = CursorState.DOWN;
        } else
        {
            AlivePP.GetComponent<Pointer>().ClickRelease();
            GlobalGameManager.Instance.CursorState = CursorState.UP;
        }

        transform.position = point;

    }

    void Update()
    {   
        //Call HoveredButton's function
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (GlobalGameManager.Instance.HoveringObject)
            {
                GlobalGameManager.Instance.HoveredObject.GetComponent<UIButton>().OnClick();
            }
            
        }
    }
}