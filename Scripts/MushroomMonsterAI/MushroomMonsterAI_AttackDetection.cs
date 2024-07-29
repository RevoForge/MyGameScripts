using System.Linq;
using UnityEngine;

public class MushroomMonsterAI_AttackDetection : MonoBehaviour
{
    private Animator animator;
    private GameObject parent;
    private MushroomMonsterAI monsterAI;

    void OnEnable()
    {
        animator = GetComponentInParent<Animator>();
        parent = animator.gameObject;
        monsterAI = parent.GetComponent<MushroomMonsterAI>();
    }

    void OnTriggerStay(Collider other)
    {
        if (monsterAI.isAlive && other.CompareTag("Player") && !(animator.GetCurrentAnimatorStateInfo(0).IsName("MushAttack01") || animator.GetCurrentAnimatorStateInfo(0).IsName("MushAttack02") || animator.GetCurrentAnimatorStateInfo(0).IsName("MushAttack03")))
        {
            int[] weights = { 5, 1, 5 };
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
                            animator.SetTrigger("attack1");
                            break;
                        case 1:
                            animator.SetTrigger("attack2");
                            break;
                        case 2:
                            animator.SetTrigger("attack3");
                            break;
                    }
                    break;
                }
            }
        }
    }
}
