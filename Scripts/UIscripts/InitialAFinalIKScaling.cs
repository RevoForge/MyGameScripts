using RootMotion.FinalIK;
using System.Collections;
using UnityEngine;

public class InitialFinalIKScaling : MonoBehaviour
{
    private VRIK ik;
    public float scaleMlp = 1f;
    private float delay = 1f;

    void Start()
    {
        ik = GetComponent<VRIK>();
        StartCoroutine(AvatarResizeWithDelay());
    }
    private IEnumerator AvatarResizeWithDelay()
    {
        yield return new WaitForSeconds(delay);
        float sizeF = (ik.solver.spine.headTarget.position.y - ik.references.root.position.y) / (ik.references.head.position.y - ik.references.root.position.y);
        ik.references.root.localScale *= sizeF * scaleMlp;
    }
    void OnEnable()
    {
        StartCoroutine(AvatarResizeWithDelay());
    }
}
