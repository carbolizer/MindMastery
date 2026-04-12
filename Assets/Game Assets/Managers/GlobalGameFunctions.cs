using UnityEngine;

public class GobalGameFunctions : MonoBehaviour
{
    public void Function1()
    {
        Debug.Log("hello from func 1");
    }

    public void Function2()
    {
        Debug.Log("hello from func 2");
    }

    public void OnDrink()
    {
        int drinkAmount = Random.Range(2, 15);
        int susAmount = Random.Range(3, 13);


        GlobalGameManager.Instance.DrinkTimer = 10;
        GlobalGameManager.Instance.CanDrink = false;

        if (GlobalGameManager.Player.m_sobriety >= drinkAmount)
        {
            GlobalGameManager.Player.m_sobriety -= drinkAmount;
            GlobalGameManager.Player.m_suspicion -= susAmount;
            Debug.Log($"Drink drank: -{drinkAmount} sobriety, -{susAmount} suspicion");
        } else
        {
            GlobalGameManager.Player.m_sobriety = 0;
            GlobalGameManager.Player.m_state = PlayerSpecialState.Drunk;
            Debug.Log("Player Drunk! Game Over");
        }
        
        
    }

    

    
    



    void Update()
    {
        if (GlobalGameManager.Instance.DrinkTimer > 0 && !GlobalGameManager.Instance.CanDrink)
        {
            GlobalGameManager.Instance.DrinkTimer -= Time.deltaTime;
        } else if (!GlobalGameManager.Instance.CanDrink)
        {
            GlobalGameManager.Instance.CanDrink = true;
            GlobalGameManager.Instance.DrinkTimer = 0;
        }
    }
}
