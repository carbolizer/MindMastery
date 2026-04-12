using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;

    private InputAction left_right;
    private InputAction up_down;

    private Animator animator;



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

        animator = transform.GetChild(1).GetComponent<Animator>();
    }

   

    void FixedUpdate()
    {
        float x = left_right.ReadValue<float>();
        float y = up_down.ReadValue<float>();

        animator.SetInteger("Up-Down", (int)y);
        animator.SetInteger("Left-Right", (int)x);

        Vector2 move = new Vector2(x, y);
        rb.linearVelocity = move * moveSpeed;
    }
}
