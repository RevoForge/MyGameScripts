using UnityEngine;

public class AudioAnimationEventHandler : MonoBehaviour
{
    private AudioSource audioSource;
    private Animator animator;
    public string animationState;

    private void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();

        // Get the Animator component from the expected parent GameObject
        animator = GetComponent<Animator>();
    }

    public void PlaySound()
    {
        int targetStateHash = Animator.StringToHash(animationState);
        if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == targetStateHash)
        {
            audioSource.Play();
        }
    }
}
