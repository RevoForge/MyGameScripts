using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Revo.Methods.ObjectInteraction;

public class SpiderEggsac : MonoBehaviour
{

    [Header("Destructable Stats")]
    public bool isDestructable = false;
    [Tooltip("Loses 1 Health per melee hit")]
    public int objectHealth = 10;
    [Tooltip("Loses 2 Health per melee hit from weak damage weapon")]
    public bool useWeakDamage = false;
    public DamageType weakDamageType;
    [Header("Sound Effect")]
    public bool playSoundEffect = true;
    public AudioClip destuctableHit;
    public AudioClip destuctableDestroyed;

    [Header("Shake Settings")]
    public int numberOfShakes = 5;
    public float shakeMagnitude = 0.1f;
    public float shakeInterval = 0.1f;

    [Header("Spider Settings")]
    public GameObject spiderPrefab;
    public int spiderSpawnAmt = 20;
    public int spawnDelay = 5;
    public Transform spawnPoint;
    public bool startSpawnOnEnable = false;


    // For Testing
    [Header("Testing")]
    public bool makeShake;

    // Additional variables
    private Vector3 originalPosition;
    private bool hasBeenHit = false;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        if (playSoundEffect)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (startSpawnOnEnable)
        {
            StartCoroutine(SpiderSpawn());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && isDestructable)
        {
            if (other.gameObject.TryGetComponent(out PickupAttackMonster pickupAttack) && !hasBeenHit)
            {
                hasBeenHit = true;
                StartCoroutine(HitReset());
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
                if (objectHealth <= 0)
                {
                    StartCoroutine(DestroyObject());
                }
            }
            }
    }
    public void StartSpawn()
    {
        StartCoroutine(SpiderSpawn());
    }
    private IEnumerator SpiderSpawn()
    {
        for (int i = 0; i < spiderSpawnAmt; i++)
        {
            StartCoroutine(Shake());
            Instantiate(spiderPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    private IEnumerator HitReset()
    {
        yield return new WaitForSeconds(0.5f);
        hasBeenHit = false;
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
        else
        {
            StopAllCoroutines();
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
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
