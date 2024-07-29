using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Revo.Methods.ObjectInteraction;

public class AI_PlayerAttacked : MonoBehaviour
{
    private static Dictionary<GameObject, bool> parentCanTriggerDict = new();
    private IPlayerDetectionAI mainAI;
    private GameObject parent;

    void Start()
    {
        mainAI = GetComponentInParent<IPlayerDetectionAI>();
        parent = (mainAI as MonoBehaviour).gameObject;
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
        yield return new WaitForSeconds(0.5f);
        parentCanTriggerDict[parent] = true;
    }
}
