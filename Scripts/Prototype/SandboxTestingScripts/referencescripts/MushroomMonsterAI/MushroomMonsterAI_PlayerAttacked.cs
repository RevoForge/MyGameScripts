using System.Collections;
using UnityEngine;

public class MushroomMonsterAI_PlayerAttacked : MonoBehaviour
{
    private bool CanTrigger;
    private MushroomMonsterAI mainAI;

    private void OnEnable()
    {
        mainAI = GetComponentInParent<MushroomMonsterAI>();
        CanTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (CanTrigger && other.CompareTag("Player"))
        {
            mainAI.PlayerAttacked(other.gameObject);
            CanTrigger = false;
            StartCoroutine(ResetTrigger());
        }
    }

    IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(1f);
        CanTrigger = true;
    }
}
