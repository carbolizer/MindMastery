using UnityEngine;

public class DistanceEnable : MonoBehaviour
{
    private GameObject Player;
    private float distanceToPlayer = 0;
    void Start()
    {
        Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        if (distanceToPlayer <= GlobalGameManager.Instance.GrabDistanceThreshold)
        {
            if (TryGetComponent(out BoxCollider2D bc))
            {
                bc.enabled = true;
            }
            if (TryGetComponent(out CircleCollider2D cc))
            {
                cc.enabled = true;
            }
            if (TryGetComponent(out CapsuleCollider2D cpc))
            {
                cpc.enabled = true;
            }
            
        } else
        {
            if (TryGetComponent(out BoxCollider2D bc))
            {
                bc.enabled = false;
            }
            if (TryGetComponent(out CircleCollider2D cc))
            {
                cc.enabled = false;
            }
            if (TryGetComponent(out CapsuleCollider2D cpc))
            {
                cpc.enabled = false;
            }
        }
    }
}
