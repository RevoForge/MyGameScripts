using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Revo.Methods.LootSystem;
using static Revo.Methods.ObjectInteraction;

public class MonsterStats : MonoBehaviour
{
    private int maxHealth = 100;
    private int experiencePoints = 100;
    private BasicWanderingAI basicWanderingAI = null;
    private DragonBossPathingScript dragonBossAI = null;
    private DevilAI devilAI = null;
    private MimicAI mimicAI = null;
    private RockMonsterAI rockMonsterAI = null;
    private MushroomMonsterAI mushroomMonsterAI = null;
    private SpiderAI spiderAI = null;
    private bool isAlive;
    private bool isInvincible = false;
    private Slider healthSlider;
    private bool chasingPlayer = false;
    [HideInInspector]
    public bool isBoss;
    [HideInInspector]
    public bool canHeal;
    private bool hasHealed;
    public DamageType weaknessDamageType;
    private bool fightStarted;
    public LootEntry[] lootEntries;
    private Transform lootTransform;
    [HideInInspector]
    public bool canDropLoot = false;
    public int numberOfItemsToDrop = 1;
    private int currentHealth;

    public delegate void OnMonsterDeath(int experience, bool isBoss);
    public static event OnMonsterDeath MonsterDeathEvent;
    public delegate void ForMonsterDeath();
    public event ForMonsterDeath MonsterSpawnEvent;


    private void OnEnable()
    {
        currentHealth = maxHealth;
        basicWanderingAI = GetComponent<BasicWanderingAI>();
        mimicAI = GetComponent<MimicAI>();
        rockMonsterAI = GetComponent<RockMonsterAI>();
        mushroomMonsterAI = GetComponent<MushroomMonsterAI>();
        dragonBossAI = GetComponent<DragonBossPathingScript>();
        devilAI = GetComponent<DevilAI>();
        spiderAI = GetComponent<SpiderAI>();
        lootTransform = transform.Find("LootDrop");
        healthSlider = GetComponentInChildren<Slider>();
        isAlive = true;
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage, bool isranged, GameObject target, int attacktype)
    {
        if (isAlive)
        {
            if (mushroomMonsterAI != null) { isInvincible = mushroomMonsterAI.isStatue; }
            if (rockMonsterAI != null) { isInvincible = rockMonsterAI.isRubble; }
            if (mimicAI != null) { isInvincible = mimicAI.isShut; }

            if (!isInvincible)
            {
                fightStarted = true;
                if (attacktype == (int)weaknessDamageType)
                {
                    currentHealth -= (damage * 2);
                }
                else
                {
                    currentHealth -= damage;
                }
                if (healthSlider != null)
                {
                    healthSlider.value = currentHealth;
                }
                if (basicWanderingAI != null && isAlive) { basicWanderingAI.TookDamage(isranged, target); }
                if (rockMonsterAI != null && isAlive) { rockMonsterAI.TookDamage(); }
                if (mushroomMonsterAI != null && isAlive) { mushroomMonsterAI.TookDamage(); }
                if (mimicAI != null && isAlive) { mimicAI.TookDamage(); }
                if (devilAI != null && isAlive) { devilAI.TookDamage(isranged, target); }
                if (dragonBossAI != null && isAlive) { dragonBossAI.TookDamage(isranged, target); }
                if (spiderAI != null && isAlive) { spiderAI.TookDamage(isranged, target); }
            }
        }


        if (currentHealth <= 0 && isAlive)
        {
            isAlive = false;
            Die();

        }
    }
    private void Update()
    {
        if (!hasHealed && canHeal && fightStarted && (currentHealth <= (maxHealth / 2)))
        {
            StartCoroutine(MidFightRegen());
        }
    }
    public void ActivateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
        }
    }
    public void DeActivateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
        }
    }
    public void SetMonsterStats(int health, int exp)
    {
        maxHealth = health;
        experiencePoints = exp;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
        currentHealth = health;
    }
    public void SetMonsterStatsOverleveled()
    {
        experiencePoints = 0;
    }
    public void HealthRegen()
    {
        chasingPlayer = false;
        StartCoroutine(RegenerateHealth());
    }
    private IEnumerator RegenerateHealth()
    {
        while (!chasingPlayer && currentHealth < maxHealth)
        {
            currentHealth += 100;
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }
            yield return new WaitForSeconds(1f);
        }

        if (!chasingPlayer)
        {
            currentHealth = maxHealth;
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }
        }
    }

    public void StopHealthRegen()
    {
        chasingPlayer = true;
    }
    public IEnumerator MidFightRegen()
    {
        hasHealed = true;
        if (devilAI != null) { devilAI.StartHealing(); }
        while (currentHealth < maxHealth)
        {
            currentHealth += 100;
            if (healthSlider != null && currentHealth < maxHealth)
            {
                healthSlider.value = currentHealth;
            }
            yield return new WaitForSeconds(0.1f);
        }
        currentHealth = maxHealth; if (healthSlider != null) { healthSlider.value = currentHealth; }
        if (devilAI != null) { devilAI.StopHealing(); }
    }
    private void Die()
    {
        if (healthSlider != null) { healthSlider.gameObject.SetActive(false); }
        if (basicWanderingAI != null) { basicWanderingAI.Death(); }
        if (mimicAI != null) { mimicAI.Death(); }
        if (rockMonsterAI != null) { rockMonsterAI.Death(); }
        if (mushroomMonsterAI != null) { mushroomMonsterAI.Death(); }
        if (devilAI != null) { devilAI.Death(); };
        if (dragonBossAI != null) { dragonBossAI.Death(); }
        if (spiderAI != null) { spiderAI.Death(); }
        MonsterDeathEvent?.Invoke(experiencePoints, isBoss);
        MonsterSpawnEvent?.Invoke();
        if (canDropLoot)
        {

            for (int i = 0; i < numberOfItemsToDrop; i++)
            {
                GameObject selectedObject = DropLoot(lootEntries);

                if (selectedObject != null && lootTransform != null)
                {
                    Instantiate(selectedObject, lootTransform.position, lootTransform.rotation);
                }
            }
        }

        StartCoroutine(DestroyAfterDelay(10f));
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
