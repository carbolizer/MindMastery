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
        switch (animator.GetInteger("SceneID"))
        {
            case 0:
            //SceneManager.LoadScene();
            break;
            case 1:
            //Roulette
            break;
            case 2:
            //Blackjack
            break;
            case 3:
            //3 card poker
            break;
            case 4:
            //LoseScreen
            break;
            case 5:
            //Win Screen
            break;
            case 6:
            //Main Menu
            break;
        }
    }

    public void SceneSwapFinished()
    {
        animator.SetBool("SwitchScene", false);
    }
}
