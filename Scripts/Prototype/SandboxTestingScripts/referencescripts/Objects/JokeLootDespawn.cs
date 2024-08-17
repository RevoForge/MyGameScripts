using System.Collections;
using UnityEngine;

public class JokeLootDespawn : MonoBehaviour
{
    private AudioSource m_AudioSource;

    private void OnEnable()
    {
        m_AudioSource = GetComponent<AudioSource>();
        StartCoroutine(DestroyJokeLoot());

    }

    private void OnCollisionEnter(Collision collision)
    {
        m_AudioSource.Play();
    }
    private IEnumerator DestroyJokeLoot()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

}
