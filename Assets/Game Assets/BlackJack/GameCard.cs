using UnityEngine;

public class GameCard : MonoBehaviour
{
    public CardValue cardValue = CardValue.Ace;
    public CardSuit cardSuit = CardSuit.Clubs;
    public string cardName = "A-C";

    public bool flipped = false;

    //for card flip
    private Animator animator;
    

    public Sprite back;
    public Sprite front;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        back = Resources.Load<Sprite>("Cards/back");
        front = Resources.Load<Sprite>("Cards/" + cardName);
        RefreshVisual();
        
    }

    public void FlipCardOver()
    {
        if (animator.GetBool("ShowingFront"))
        {
            GetComponent<SpriteRenderer>().sprite = back;
        } else
        {
            GetComponent<SpriteRenderer>().sprite = front;
        }
        animator.SetBool("ShowingFront", !animator.GetBool("ShowingFront"));
    }

    public void FlipCard()
    {
        if (animator.GetBool("CanFlip"))
            animator.SetBool("FlipRequested", true);
    }


    public void OnFlipFinished()
    {
        animator.SetBool("CanFlip", true);
    }

    public void OnFlipStart()
    {
        animator.SetBool("CanFlip", false);
        animator.SetBool("FlipRequested", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshVisual()
    {
        back = Resources.Load<Sprite>("Cards/back");
        front = Resources.Load<Sprite>("Cards/" + cardName);
        if (flipped)
        {
            GetComponent<SpriteRenderer>().sprite = back;
            animator.SetBool("ShowingFront", false);
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = front;
            animator.SetBool("ShowingFront", true);
        }
    }

}
