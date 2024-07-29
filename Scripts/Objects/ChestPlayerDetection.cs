using System.Collections;
using UnityEngine;
using static Revo.Methods.LootSystem;

public class ChestPlayerDetection : MonoBehaviour
{
    private Animator m_Animator;
    public LootEntry[] lootEntries;
    private Transform lootTransform;
    private bool chestLooted;
    private MonsterSpawn monsterSpawn;

    void OnEnable()
    {
        monsterSpawn = GetComponentInParent<MonsterSpawn>();
        m_Animator = GetComponentInParent<Animator>();
        lootTransform = transform.Find("LootDrop");
        chestLooted = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !chestLooted)
        {
            m_Animator.SetTrigger("open");
            chestLooted = true;
            StartCoroutine(LootDropping());
        }
    }
    private IEnumerator LootDropping()
    {
        yield return new WaitForSeconds(1f);
        GameObject selectedObject = DropLoot(lootEntries);

        if (selectedObject != null)
        {
            Instantiate(selectedObject, lootTransform.position, lootTransform.rotation);
        }
        StartCoroutine(DespawnChest());

    }
    private IEnumerator DespawnChest()
    {
        yield return new WaitForSeconds(5f);
        monsterSpawn.ChestLooted();
        Destroy(m_Animator.gameObject);
    }
}

