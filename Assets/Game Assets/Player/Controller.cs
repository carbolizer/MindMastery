using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;

    private InputAction left_right;
    private InputAction up_down;



    private Rigidbody2D rb;
    private float moveSpeed = 8f;

    void Awake()
    {
        var actionMap = inputActions.FindActionMap("Default");
        left_right = actionMap.FindAction("LeftRight");
        up_down = actionMap.FindAction("UpDown");

        left_right.Enable();
        up_down.Enable();

        rb = GetComponent<Rigidbody2D>();
    }

   

    void FixedUpdate()
    {
        float x = left_right.ReadValue<float>();
        float y = up_down.ReadValue<float>();

        Debug.Log("x: " + x + " y: " + y);

        rb.transform.position += new Vector3(x, y, rb.transform.position.z) * Time.deltaTime * moveSpeed;
    }
}
