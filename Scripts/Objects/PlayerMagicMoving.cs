using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Revo.Methods.ObjectInteraction;

public class PlayerMagicMoving : MonoBehaviour
{
    [HideInInspector] public float speed = 1f;
    [HideInInspector] public float spellTime = 1f;
    [HideInInspector] public PlayerStats mainAI;
    [HideInInspector] public int baseDamage = 1;
    private bool CanTriggerRange;
    private bool hasHit;
    private ParticleSystem acid;
    private ParticleSystem flame;
    private SphereCollider impactZone;
    public DamageType selectedDamageType;

    private void OnEnable()
    {
        hasHit = false;
        CanTriggerRange = true;
        Transform acidtransform = transform.Find("AcidCloud");
        if (acidtransform != null) { acid = acidtransform.GetComponent<ParticleSystem>(); }
        Transform flametransform = transform.Find("FlameCloud");
        if (flametransform != null) { flame = flametransform.GetComponent<ParticleSystem>(); }
        impactZone = GetComponent<SphereCollider>();
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
        if (!CanTriggerRange || !other.CompareTag("Mob"))
        {
            return;
        }
        impactZone.radius = 10;
        HashSet<MonsterStats> uniqueMonsterStats = new();
        MonsterStats[] monsterStats = other.GetComponentsInParent<MonsterStats>();
        foreach (MonsterStats monsterStat in monsterStats)
        {
            uniqueMonsterStats.Add(monsterStat);
        }
        foreach (MonsterStats monsterStat in uniqueMonsterStats)
        {
            mainAI.AttackMonster(monsterStat, baseDamage, true, (int)selectedDamageType);
        }
        if (flame != null && !flame.emission.enabled)
        {
            var emission = flame.emission;
            emission.enabled = true;
        }
        if (acid != null && !acid.emission.enabled)
        {
            var emission = acid.emission;
            emission.enabled = true;
        }
        CanTriggerRange = false;
        hasHit = true;
        StartCoroutine(ResetTrigger());
        Destroy(gameObject, spellTime);
    }

    private IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(1f);
        CanTriggerRange = true;
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(spellTime * 3);
        if (!hasHit)
        {
            Destroy(gameObject);
        }
    }
}
