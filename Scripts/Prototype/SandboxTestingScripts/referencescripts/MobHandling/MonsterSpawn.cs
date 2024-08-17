using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Revo.Methods.ObjectInteraction;

public class MonsterSpawn : MonoBehaviour
{
    [Header("Monster Prefab Settings")]
    public GameObject mainMonsterPrefab;
    public GameObject fakeMonsterPrefab;
    public bool useFakeMonsterPrefab;
    public float fakeMonsterChance;
    [Header("Main Monster Settings")]
    public int damage = 10;
    public float respawnDelay = 30f;
    public int monsterHealth = 100;
    public int experience = 100;
    public bool showHealthBar = true;
    [Header("Boss Class Monsters Get 5x Health, EXP and 3x damage")]
    public bool isBoss = false;
    public bool canHeal = false;
    private GameObject currentMonster;
    private bool isRespawning;
    private MonsterStats monsterStats;
    private NavMeshAgent navMeshAgent;
    private Transform[] damageboxes;
    private bool looted;
    public bool canDroploot;
    private IMonsterAI validAI;


    private void OnEnable()
    {
        if (!isBoss)
        {
            if (useFakeMonsterPrefab && Random.value < fakeMonsterChance)
            {
                Invoke(nameof(SpawnModel), 1f);
            }
            else
            {
                Invoke(nameof(SpawnMonster), 1f);
            }
        }
        looted = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }

    private void SpawnModel()
    {
        currentMonster = Instantiate(fakeMonsterPrefab, transform.position, transform.rotation);
        currentMonster.transform.SetParent(transform);
        StartCoroutine(RespawnModel());
    }

    private void SpawnMonster()
    {
        canDroploot = Random.value < 0.5f;
        currentMonster = Instantiate(mainMonsterPrefab, transform.position, transform.rotation);
        currentMonster.transform.SetParent(transform);
        monsterStats = currentMonster.GetComponent<MonsterStats>();
        monsterStats.SetMonsterStats(monsterHealth, experience);
        monsterStats.canHeal = canHeal;
        monsterStats.canDropLoot = canDroploot;

        if (currentMonster.TryGetComponent(out validAI))
        {
            validAI.Damage = damage;
            validAI.ShowHealthBar = showHealthBar;
        }

        monsterStats.MonsterSpawnEvent += ForMonsterSpawn;
        isRespawning = false;
    }
    public void SpawnBoss()
    {
        canDroploot = true;
        currentMonster = Instantiate(mainMonsterPrefab, transform.position, transform.rotation);
        currentMonster.transform.SetParent(transform);
        monsterStats = currentMonster.GetComponent<MonsterStats>();
        monsterStats.canHeal = canHeal;
        monsterStats.canDropLoot = canDroploot;
        monsterStats.SetMonsterStats(monsterHealth * 5, experience * 5);
        monsterStats.isBoss = isBoss;
        currentMonster.transform.localScale *= 1.5f;

        if (currentMonster.TryGetComponent(out validAI))
        {
            validAI.Damage = damage * 3;
            validAI.ShowHealthBar = showHealthBar;
        }

        Transform[] damageboxes = currentMonster.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform damagebox in damageboxes)
        {
            if (damagebox.name == "PlayerContact")
            {
                damagebox.localScale *= 1.5f;
            }
        }
        if (currentMonster.TryGetComponent(out navMeshAgent))
        {
            navMeshAgent.stoppingDistance *= 1.5f;
        }
    }



    private void ForMonsterSpawn()
    {
        if (!isRespawning)
        {
            StartCoroutine(RespawnMonster());
        }
    }

    private IEnumerator RespawnMonster()
    {
        isRespawning = true;
        float delay = isBoss ? respawnDelay * 10 : respawnDelay;
        yield return new WaitForSeconds(delay);
        SpawnMonster();
    }
    private IEnumerator RespawnModel()
    {
        if (!looted)
        {
            isRespawning = true;
            yield return new WaitForSeconds(respawnDelay * 2);
            Destroy(currentMonster);
            SpawnMonster();
        }
    }
    public void ChestLooted()
    {
        looted = true;
        ForMonsterSpawn();
    }

}
