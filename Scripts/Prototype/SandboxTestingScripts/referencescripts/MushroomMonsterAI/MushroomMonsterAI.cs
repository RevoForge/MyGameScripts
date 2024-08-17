using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Revo.Methods.ObjectInteraction;

public class MushroomMonsterAI : MonoBehaviour, IMonsterAI
{
    public int Damage { get; set; }
    public bool ShowHealthBar { get; set; }
    private PlayerStats playerStats;
    private Animator animator;
    [HideInInspector] public bool isAlive;
    private Transform castSpot;
    public GameObject spellPrefab;
    public bool increaseChaseSpeed;
    private bool chasingPlayer;
    private GameObject targetPlayer;
    private Transform spawn;
    private NavMeshAgent Agent;
    private GameObject playerdetection;
    private Vector3 originalScale;
    private float ostopping;
    [HideInInspector] public bool isStatue;
    private MonsterStats monsterStats;
    private float timer = 0f;
    private readonly float interval = 3f;
    private int playerdamagereduction = 0;

    private void OnEnable()
    {
        isStatue = true;
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        monsterStats = GetComponent<MonsterStats>();
        isAlive = true;
        Transform childTransform = transform.Find("CastSource");
        if (childTransform != null)
        {
            castSpot = childTransform;
        }
        Transform childTransform2 = transform.Find("MushroomPlayerDetection");
        if (childTransform2 != null)
        {
            playerdetection = childTransform.gameObject;
        }
        originalScale = playerdetection.transform.localScale;
        ostopping = Agent.stoppingDistance;
    }

    void Update()
    {
        if (spawn == null)
        {
            Transform parentTransform = transform.parent;
            if (parentTransform != null)
            {
                spawn = parentTransform;
            }
        }
        if (chasingPlayer && isAlive)
        {
            Agent.SetDestination(targetPlayer.transform.position);
            if (Agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                if (!Agent.isStopped)
                {
                    Agent.isStopped = true;
                    monsterStats.HealthRegen();
                }
            }
            else if (Agent.isStopped)
            {
                Agent.isStopped = false;
                monsterStats.StopHealthRegen();
            }
            if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                Vector3 targetDirection = targetPlayer.transform.position - transform.position;
                targetDirection.y = 0;

                if (targetDirection.sqrMagnitude > 0 * 0)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Agent.angularSpeed * Time.deltaTime);
                }
            }
        }
    }

    public void CastMagic()
    {
        if (isAlive)
        {
            var newSpell = Instantiate(spellPrefab, castSpot.position, castSpot.rotation);
            var magic = newSpell.GetComponent<MushroomMagicMoving>();
            magic.mainAI = this;

        }
    }

    public void PlayerAttacked(GameObject player)
    {
        if (isAlive)
        {
            playerStats = player.GetComponent<PlayerStats>();
            playerStats.TakeDamage(Damage);
            if (playerdamagereduction == 0)
            {
                playerdamagereduction = playerStats.ReduceDamage();
                if (Damage <= playerdamagereduction)
                {
                    monsterStats.SetMonsterStatsOverleveled();
                }
            }
        }

    }
    public void GoHome()
    {
        Agent.stoppingDistance = 0.1f;
        Agent.destination = spawn.position;
        StartCoroutine(CheckReachedHome());
    }

    private IEnumerator CheckReachedHome()
    {
        while (Agent.remainingDistance >= Agent.stoppingDistance)
        {
            yield return null;
        }
        if (ShowHealthBar)
        {
            monsterStats.DeActivateHealthBar();
        }
        animator.SetFloat("locomotion", 0f);
        isStatue = true;
        animator.SetTrigger("goStatue");

    }

    public void TookDamage()
    {
        if (isAlive)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                animator.SetTrigger("gotHit");

                timer = 0f;
            }

        }
    }

    public void PlayerDetected(GameObject player)
    {
        if (isAlive)
        {
            if (isStatue)
            {
                animator.SetTrigger("goMonster");
                isStatue = false;
            }
            Agent.stoppingDistance = ostopping;
            playerdetection.transform.localScale = new Vector3(10f, 10f, 10f);
            targetPlayer = player;
            animator.SetFloat("locomotion", 1f);
            chasingPlayer = true;
            if (ShowHealthBar)
            {
                monsterStats.ActivateHealthBar();
            }
            Agent.speed = increaseChaseSpeed ? (Agent.speed * 2f) : Agent.speed;
        }
    }

    public void PlayerLost()
    {
        if (isAlive)
        {
            chasingPlayer = false;
            playerdetection.transform.localScale = originalScale;
            GoHome();
            Agent.speed = increaseChaseSpeed ? (Agent.speed / 2f) : Agent.speed;
        }
    }

    public void Death()
    {
        isAlive = false;
        Agent.isStopped = true;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, false);
            }
            else if (parameter.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(parameter.name);
            }
        }
        animator.SetFloat("locomotion", 0f);
        animator.SetTrigger("death");
    }
}
