using UnityEngine;
using static Revo.Methods.ObjectInteraction;

public class Projectile : MonoBehaviour 
{
    public GameObject ExplosionPrefab;
    public float DestroyExplosion = 4.0f;
    public float DestroyChildren = 2.0f;
    [HideInInspector] public float speed = 10.0f; 
    public Transform castSpot;
    [HideInInspector]public PlayerStats mainAI;
    [HideInInspector] public int baseDamage = 1;
    private bool hasHit;
    [HideInInspector] public DamageType selectedDamageType;

    private void Update()
    {
        if (!hasHit)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Mob"))
        {
            return;
        }
        hasHit = true;
        // Particle Methods
        var exp = Instantiate(ExplosionPrefab, transform.position, ExplosionPrefab.transform.rotation);
        Destroy(exp, DestroyExplosion);
        Transform child;
        child = transform.GetChild(0);
        transform.DetachChildren();
        Destroy(child.gameObject, DestroyChildren);
        // Monster Damage Methods
        if (other.TryGetComponent(out MonsterStats monsterStat))
        {
            mainAI.AttackMonster(monsterStat, baseDamage, true, (int)selectedDamageType);
        }
        //-----------------------
        Destroy(gameObject);
    }
}
