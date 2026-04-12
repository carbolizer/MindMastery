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
