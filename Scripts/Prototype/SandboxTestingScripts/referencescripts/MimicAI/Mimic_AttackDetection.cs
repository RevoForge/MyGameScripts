using UnityEngine;

public class MimicAttackDetection : MonoBehaviour
{
    private MimicAI mainAI;
    private Animator animator;


    void OnEnable()
    {
        animator = GetComponentInParent<Animator>();
        mainAI = GetComponentInParent<MimicAI>();
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !animator.GetCurrentAnimatorStateInfo(0).IsName("attack1") && mainAI.isAlive)
        {
            animator.SetTrigger("attack1");
        }
    }
}
