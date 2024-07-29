using System.Collections;
using UnityEngine;

public class MushroomMagicMoving : MonoBehaviour
{
    public float speed = 2.0f;
    public int spellTime = 3;
    private bool CanTriggerRange;
    [HideInInspector]
    public MushroomMonsterAI mainAI;
    private bool hasHit;


    private void OnEnable()
    {
        hasHit = false;
        CanTriggerRange = true;
        StartCoroutine(Destroy());
    }
    private void Update()
    {
        if (!hasHit)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (CanTriggerRange && other.CompareTag("Player"))
        {
            mainAI.PlayerAttacked(other.gameObject);
            CanTriggerRange = false;
            StartCoroutine(ResetTrigger());
            hasHit = true;
            Destroy(gameObject, spellTime);
        }
    }

    private IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(1f);
        CanTriggerRange = true;
    }
    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(spellTime * 2);
        if (!hasHit)
        {
            Destroy(gameObject);
        }
    }
}
