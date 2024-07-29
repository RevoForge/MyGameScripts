using UnityEngine;

public class WanderingAI_AttackDetection : MonoBehaviour
{
    private BasicWanderingAI mainAI;

    void Start()
    {
        mainAI = GetComponentInParent<BasicWanderingAI>();
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mainAI.PlayerInAttackRange();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mainAI.PlayerOutOfAttackRange();
        }
    }
}
