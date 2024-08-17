using UnityEngine;

public class SkillMovementandDamage : MonoBehaviour
{
    public Skill SkillScriptable;
    public float speed = 1.0f;
    public float DeathTimer = 5.0f;
    private Vector3 forward;
    [HideInInspector]
    public RRSkillSystem rrSkillSystem;
    public int damage;
    private bool MonsterDead;
    [HideInInspector]
    public bool isRight;
    [HideInInspector]
    public PlayerStats playerStats;
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
        //Debug.Log("Hit Something");
        if (other.gameObject.CompareTag("Mob"))
        {
            // need to add logic to check if multiple mobs are hit in case of aoe or make a seperate class for it
            MonsterStats monsterStats = other.gameObject.GetComponent<MonsterStats>();
            //Debug.Log("Hit Mob");
            if (rrSkillSystem != null)
            {
                //Debug.Log("Skill System Exists");
                /* proto code left for reference to update MonsterStats
                //If Monster was killed this hit
                if ()
                {
                    MonsterDead = isDead;
                }
                // If Monster is already dead before hitting
                if (isNegativeHealthPrevented)
                {
                    // Dont add skill XP for hitting a Dead mob
                    Destroy(gameObject);
                }
                */
                if (monsterStats.isAlive)
                {
                    // Player call for attack
                    playerStats.SkillAttackMonster(monsterStats, SkillScriptable);
                    // skill system call for adding exp on successful attack
                    rrSkillSystem.AddSkillXP(MonsterDead, isRight);
                    //TODO: Change to account for AOE and/or DOT
                    Destroy(gameObject);
                }
            }
        }
    }
}
