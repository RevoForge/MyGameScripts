using System.Collections;
using UnityEngine;
using static Revo.Methods.ObjectInteraction;

public class Destructable : MonoBehaviour
{
    [Header("Destructable Stats")]
    [Tooltip("Loses 1 Health per melee hit")]
    public int objectHealth = 10;
    [Tooltip("Loses 2 Health per melee hit from weak damage weapon")]
    public bool useWeakDamage = false;
    public DamageType weakDamageType;
    [Header("Sound Effect")]
    public bool playSoundEffect = true;
    public AudioClip destuctableHit;
    public AudioClip destuctableDestroyed;

    // Variables for shaking effect
    [Header("Shake Settings")]
    public int numberOfShakes = 3;
    public float shakeMagnitude = 0.1f;
    public float shakeInterval = 0.1f;

    // For Testing
    [Header("Testing")]
    public bool makeShake;

    // Additional variables
    private Vector3 originalPosition;
    private AudioSource audioSource;
    private bool hasBeenHit = false;

    private void Start()
    {
        if (playSoundEffect)
        {
            audioSource = GetComponent<AudioSource>();
        }
        originalPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            if (other.gameObject.TryGetComponent(out PickupAttackMonster pickupAttack) && !hasBeenHit)
            {
                hasBeenHit = true;
                if (useWeakDamage)
                {
                    if (weakDamageType == pickupAttack.selectedDamageType)
                    {
                        objectHealth -= 2;
                    }
                }
                else
                {
                    objectHealth -= 1;
                }
                if (playSoundEffect)
                {
                    audioSource.clip = destuctableHit;
                    audioSource.Play();
                }

                // Shake effect
                StartCoroutine(Shake());

                if (objectHealth <= 0)
                {
                    StartCoroutine(DestroyObject());
                }
            }
            else
            {
                Debug.Log("Not a Melee Weapon");
            }
        }
    }
    private IEnumerator DestroyObject()
    {
        if (playSoundEffect)
        {
            audioSource.clip = destuctableDestroyed;
            audioSource.Play();

            // Wait for the sound effect to finish playing
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        Destroy(gameObject);
    }

    private IEnumerator Shake()
    {
        for (int i = 0; i < numberOfShakes; i++)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.position = originalPosition + shakeOffset;
            yield return new WaitForSeconds(shakeInterval);
            ResetPosition();
        }

        ResetPosition();
        hasBeenHit = false;
    }

    private void ResetPosition()
    {
        transform.position = originalPosition;
    }
    private void Update()
    {
        if (makeShake)
        {
            StartCoroutine(Shake());
            makeShake = false;
        }
    }
}