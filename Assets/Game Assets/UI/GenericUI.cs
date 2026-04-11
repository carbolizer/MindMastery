using UnityEngine;
using UnityEngine.InputSystem;





public class GenericUI : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private GameObject PointerPrefab;
    private GameObject AlivePP;
    private Pointer pointer;


    void Start()
    {
        cam = Camera.main;
        AlivePP = Instantiate(PointerPrefab, transform);
        AlivePP.transform.position = new Vector3(transform.position.x - 0.9f, transform.position.y - 0.7f, transform.position.z);
        Cursor.visible = false;
        GlobalGameManager.Instance.CursorState = CursorState.UP;

        pointer = AlivePP.GetComponent<Pointer>();
    }

    void OnGUI()
    {
        

    }

    void Update()
    {   

        Vector2 mousePos = Mouse.current.position.ReadValue();
        

        Vector3 point = cam.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane)
        );
        GlobalGameManager.Instance.CursorPos = point;
        transform.position = point;

        

        if (Mouse.current.leftButton.isPressed)
        {
            pointer.ClickDown();
            GlobalGameManager.Instance.CursorState = CursorState.DOWN;
        } else
        {
            pointer.ClickRelease();
            GlobalGameManager.Instance.CursorState = CursorState.UP;
        }

        transform.position = point;




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