using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGoFoward : MonoBehaviour
{
    public Skill SkillScriptable;
    public float speed = 1.0f;
    public float DeathTimer = 5.0f;
    private Vector3 forward;
    [HideInInspector]
    public RRSkillSystem rrSkillSystem;
    // set skill type on prefab
    public SkillType SkillTypeRef;
    public int damage;
    private bool MonsterDead;
    [HideInInspector]
    public bool isRight;
    // Start is called before the first frame update
    void Start()
    {
        forward = Camera.main.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        DeathTimer -= Time.deltaTime;
        if (DeathTimer <= 0)
        {
            Destroy(gameObject);
        }
        transform.Translate(speed * Time.deltaTime * forward);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit Something");
        if (other.gameObject.CompareTag("Mob"))
        {
            Debug.Log("Hit Mob");
            if (rrSkillSystem != null)
            {
                Debug.Log("Skill System Exists");
                (bool isDead, bool isNegativeHealthPrevented) = other.gameObject.GetComponent<MonsterHealth>().TakeDamage(damage);
                //If Monster was killed this hit
                if (isDead)
                {
                    MonsterDead = isDead;
                }
                // If Monster is already dead before hitting
                if (isNegativeHealthPrevented)
                {
                    // Dont add skill XP for hitting a Dead mob
                    Destroy(gameObject);
                }
                else
                {
                    rrSkillSystem.AddSkillXP(MonsterDead, isRight); // Add Skill XP for hitting a Live mob
                    Destroy(gameObject);
                }
            }
        }
    }
}
