using System.Collections;
using UnityEngine;
using static Revo.Methods.ObjectInteraction;

public class AI_AttackZone : MonoBehaviour
{
    private IPlayerDetectionAI mainAI;
    public AttackZone attackZone;
    private bool canAttack;

    void OnEnable()
    {
        mainAI = GetComponentInParent<IPlayerDetectionAI>();
        canAttack = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canAttack)
        {
            mainAI.AttackZoneDetection(attackZone);
            canAttack = false;
            StartCoroutine(ResetTrigger());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = true;
        }
    }

    IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }
}
