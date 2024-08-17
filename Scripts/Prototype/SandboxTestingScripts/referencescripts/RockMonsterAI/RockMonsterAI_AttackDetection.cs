using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RockMonsterAI_AttackDetection : MonoBehaviour
{
    private static Dictionary<GameObject, bool> parentCanAnimDict = new Dictionary<GameObject, bool>();
    private Animator animator;
    private GameObject parent;
    private RockMonsterAI rockMonsterAI;

    void OnEnable()
    {
        animator = GetComponentInParent<Animator>();
        rockMonsterAI = GetComponentInParent<RockMonsterAI>();
        parent = animator.gameObject;
        if (!parentCanAnimDict.ContainsKey(parent))
        {
            parentCanAnimDict[parent] = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (parentCanAnimDict[parent] && rockMonsterAI.isAlive && other.CompareTag("Player") && !(animator.GetCurrentAnimatorStateInfo(0).IsName("Attackleft") || animator.GetCurrentAnimatorStateInfo(0).IsName("Attackright") || animator.GetCurrentAnimatorStateInfo(0).IsName("Magic") || animator.GetCurrentAnimatorStateInfo(0).IsName("AttackClap")))
        {
            int[] weights = { 5, 5, 3, 1 };
            int totalWeight = weights.Sum();

            int randomTrigger = Random.Range(0, totalWeight);

            int cumulativeWeight = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                cumulativeWeight += weights[i];
                if (randomTrigger < cumulativeWeight)
                {
                    switch (i)
                    {
                        case 0:
                            animator.SetTrigger("attack1A");
                            break;
                        case 1:
                            animator.SetTrigger("attack1B");
                            break;
                        case 2:
                            animator.SetTrigger("attack2");
                            break;
                        case 3:
                            animator.SetTrigger("magic");
                            break;
                    }
                    break;
                }
            }
            parentCanAnimDict[parent] = false;
            StartCoroutine(ResetTrigger(parent));
        }
    }
    IEnumerator ResetTrigger(GameObject parent)
    {
        yield return new WaitForSeconds(1f);
        parentCanAnimDict[parent] = true;
    }
}
