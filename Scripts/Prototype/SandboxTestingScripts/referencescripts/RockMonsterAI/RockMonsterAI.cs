using Revo.Methods;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Revo.Methods.ObjectInteraction;

public class RockMonsterAI : MonoBehaviour, IMonsterAI
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
    [HideInInspector]
    public bool isRubble;
    private float origstop;
    private GameObject playerdetection;
    private Vector3 originalScale;
    private MonsterStats monsterStats;
    private float timer = 0f;
    private readonly float interval = 3f;
    public float dodgeAmount = 1.0f;
    public float dodgeDuration = 0.5f;
    private Vector3 dodgeMovement;
    private int playerdamagereduction = 0;

    void OnEnable()
    {
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        monsterStats = GetComponent<MonsterStats>();
        origstop = Agent.stoppingDistance;
        isAlive = true;
        isRubble = true;
        Transform childTransform = transform.Find("RockMonsterPlayerDetection");
        if (childTransform != null)
        {
            playerdetection = childTransform.gameObject;
        }
        originalScale = playerdetection.transform.localScale;
    }

    public void PlayerAttacked(GameObject player)
    {
        if (isAlive && !animator.GetCurrentAnimatorStateInfo(0).IsName("RubblePose"))
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
            animator.SetFloat("locomotion", Agent.remainingDistance <= Agent.stoppingDistance ? 0f : 1f);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
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
            Debug.Log("GoingHome");
            Agent.stoppingDistance = 0.1f;
            Agent.destination = spawn.position;
            StartCoroutine(CheckReachedHome());
            monsterStats.HealthRegen();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dodgeMovement = -transform.forward * dodgeAmount;
            StartCoroutine(CharacterControllerDodging.PerformDodgeMovement(Agent, dodgeMovement, dodgeDuration, transform));
        }
    }

    private IEnumerator CheckReachedHome()
    {
        animator.SetFloat("locomotion", 1f);
        while (Agent.remainingDistance >= Agent.stoppingDistance)
        {
            if (!chasingPlayer)
            {
                yield return null;
            }
            else
            {
                yield break;
            }
        }
        animator.SetFloat("locomotion", 0f);
        animator.SetTrigger("idleToRubble");
        isRubble = true;
        if (ShowHealthBar)
        {
            monsterStats.DeActivateHealthBar();
        }
        Agent.stoppingDistance = origstop;
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
        monsterStats.StopHealthRegen();
        if (isAlive)
        {
            if (ShowHealthBar)
            {
                monsterStats.ActivateHealthBar();
            }
            playerdetection.transform.localScale = new Vector3(0f, 15f, 15f);
            Agent.stoppingDistance = origstop;
            targetPlayer = player;
            chasingPlayer = true;
            if (isRubble)
            {
                animator.SetTrigger("rubbleToIdle");
                isRubble = false;
            }
            Agent.speed = increaseChaseSpeed ? (Agent.speed * 2f) : Agent.speed;
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
        isAlive = false;
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
        Agent.isStopped = true;
        animator.SetTrigger("death");
    }
}
