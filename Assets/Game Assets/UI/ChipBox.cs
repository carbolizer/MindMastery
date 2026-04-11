using UnityEngine;

public class ChipBox : MonoBehaviour
{
    //Animation triggers for chip box
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "UIHB")
        {
            transform.GetChild(0).GetComponent<Animator>().SetBool("CursorInChipBox", true);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "UIHB")
        {
            transform.GetChild(0).GetComponent<Animator>().SetBool("CursorInChipBox", false);
        }
    }
}
