using UnityEngine;

public class Sarcophagus : MonoBehaviour
{
    private Animator m_Animator;
    private AudioSource m_AudioSource;
    private bool isPlaying;


    private void OnEnable()
    {
        m_Animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_Animator.SetTrigger("PlayerEnters");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_Animator.SetTrigger("PlayerLeaves");
        }
    }
    private void Update()
    {
        AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

        if (!isPlaying && (stateInfo.IsName("Opening") || stateInfo.IsName("Closing")) && stateInfo.normalizedTime < 1.0f)
        {
            m_AudioSource.Play();
            isPlaying = true;
        }
        else if (isPlaying && !(stateInfo.IsName("Opening") || stateInfo.IsName("Closing")) || stateInfo.normalizedTime >= 1.0f)
        {
            m_AudioSource.Stop();
            isPlaying = false;
        }
    }

}
