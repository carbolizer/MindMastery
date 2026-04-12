using UnityEngine;

public class LabelData : MonoBehaviour
{
    public bool DoesWin = false;
    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
}
