using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOutFuncs : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }



    public void SwapScene()
    {
        Debug.Log("Swap to lsoe img " + GlobalGameManager.Player.m_state);
        SceneManager.LoadScene(animator.GetInteger("SceneID"));
    }

    public void SceneSwapFinished()
    {
        animator.SetBool("SwitchScene", false);
    }
}
