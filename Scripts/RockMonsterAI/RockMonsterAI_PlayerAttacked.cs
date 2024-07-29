using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterAI_PlayerAttacked : MonoBehaviour
{
    private static Dictionary<GameObject, bool> parentCanTriggerDict = new Dictionary<GameObject, bool>();
    private RockMonsterAI mainAI;
    private GameObject parent;

    void OnEnable()
    {
        mainAI = GetComponentInParent<RockMonsterAI>();
        parent = mainAI.gameObject;
        if (!parentCanTriggerDict.ContainsKey(parent))
        {
            parentCanTriggerDict[parent] = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (parentCanTriggerDict[parent] && other.CompareTag("Player"))
        {
            mainAI.PlayerAttacked(other.gameObject);
            parentCanTriggerDict[parent] = false;
            StartCoroutine(ResetTrigger(parent));
        }
    }

    IEnumerator ResetTrigger(GameObject parent)
    {
        yield return new WaitForSeconds(1f);
        parentCanTriggerDict[parent] = true;
    }
}