using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Revo.Methods.ObjectInteraction;

public class MimicAI : MonoBehaviour, IMonsterAI
{
    public int Damage { get; set; }
    public bool ShowHealthBar { get; set; }
    private Animator animator;
    private PlayerStats playerStats;
    [HideInInspector]
    public bool isAlive;
    public bool increaseChaseSpeed;
    private bool chasingPlayer;
    private GameObject targetPlayer;
    private Transform spawn;
    private NavMeshAgent Agent;
    private GameObject playerdetection;
    private GameObject attackdetection;
    private Vector3 originalScale;
    private Vector3 attackScale;
    private float ostopping;
    [HideInInspector]
    public bool isShut;
    private MonsterStats monsterStats;
    private float timer = 0f;
    private readonly float interval = 3f;
    private int playerdamagereduction = 0;


    void OnEnable()
    {
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        monsterStats = GetComponent<MonsterStats>();
        isAlive = true;
        Transform childTransform = transform.Find("MimicPlayerDetection");
        if (childTransform != null)
        {
            playerdetection = childTransform.gameObject;
        }
        Transform child2Transform = transform.Find("MimicPlayerAttack");
        if (child2Transform != null)
        {
            attackdetection = child2Transform.gameObject;
        }
        originalScale = playerdetection.transform.localScale;
        attackScale = attackdetection.transform.localScale;
        attackdetection.transform.localScale = new Vector3(0f, 0f, 0f);
        ostopping = Agent.stoppingDistance;
        isShut = true;
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
    public void GoHome()
    {
        if (isAlive)
        {
            Agent.stoppingDistance = 0.1f;
            Agent.destination = spawn.position;
            StartCoroutine(CheckReachedHome());
        }

    }

    private IEnumerator CheckReachedHome()
    {
        while (Agent.remainingDistance >= Agent.stoppingDistance)
        {
            yield return null;
        }
        isShut = true;
        animator.SetTrigger("shut");
        animator.SetFloat("locomotion", 0f);
        if (ShowHealthBar)
        {
            monsterStats.DeActivateHealthBar();
        }
        playerdetection.GetComponent<BoxCollider>().enabled = true;
        playerdetection.GetComponent<CapsuleCollider>().enabled = false;
        attackdetection.transform.localScale = new Vector3(0f, 0f, 0f);
        gameObject.transform.rotation = spawn.transform.rotation;
    }

    public void PlayerDetected(GameObject player)
    {
        if (isAlive)
        {
            isShut = false;
            Agent.stoppingDistance = ostopping;
            playerdetection.transform.localScale = new Vector3(0f, 15f, 15f);
            playerdetection.GetComponent<CapsuleCollider>().enabled = true;
            playerdetection.GetComponent<BoxCollider>().enabled = false;
            targetPlayer = player;
            chasingPlayer = true;
            animator.SetTrigger("open");
            animator.SetFloat("locomotion", 1f);
            if (ShowHealthBar)
            {
                monsterStats.ActivateHealthBar();
            }
            attackdetection.transform.localScale = attackScale;
            Agent.speed = increaseChaseSpeed ? (Agent.speed * 2f) : Agent.speed;
        }
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

    public void PlayerLost()
    {
        if (isAlive)
        {
            playerdetection.transform.localScale = originalScale;
            chasingPlayer = false;
            GoHome();
            Agent.speed = increaseChaseSpeed ? (Agent.speed / 2f) : Agent.speed;
        }
    }

    public void Death()
    {
        animator.SetFloat("locomotion", 0f);
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
        isAlive = false;
        Agent.isStopped = true;
        animator.SetTrigger("death");
    }
}
