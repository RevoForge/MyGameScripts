using Revo.Methods;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Revo.Methods.ObjectInteraction;

public class BasicWanderingAI : MonoBehaviour, IMonsterAI
{
    public int Damage { get; set; }
    public bool ShowHealthBar { get; set; }
    [Header("Wandering Settings")]
    public bool canWander = true;
    private bool canWanderinitial;
    [Tooltip("This is the max distance the AI will wander from its current position at once!")]
    public float MaxWanderDistance = 5f;
    [Tooltip("This is the minimum distance the AI will wander from its current position at once!")]
    public float MinWanderDistance = 1f;
    [Tooltip("This is how long the AI will wait once it reaches its new position while wandering!")]
    public float WanderIdleTime = 5f;
    public bool increaseChaseSpeed = false;
    private Animator aIAnimator;
    private bool IsMovingToNext;
    private bool HasSetNextPosition;
    private float InternalWaitTime;
    private NavMeshAgent Agent;
    private bool rested;
    private bool chasingPlayer;
    private GameObject targetPlayer;
    private PlayerStats playerStats;
    private bool isAlive;
    public float restedChance = 0.2f;
    [Header("Flying Monster Settings")]
    public bool doesFly = false;
    public float flyingHeight = 3f;
    public float flyingHeightChangeSpeed = 0.5f;
    private bool onGround;
    private MonsterStats monsterStats;
    private float timer = 0f;
    private readonly float interval = 3f;
    private readonly int MaxRetries = 3;
    private int currentRetries = 0;
    public float dodgeAmount = 1.0f;
    public float dodgeDuration = 0.5f;
    private Vector3 dodgeMovement;
    private int playerdamagereduction = 0;
    private readonly float tolerance = 1f;
    private float agentORGspeed;

    void OnEnable()
    {
        Agent = GetComponent<NavMeshAgent>();
        aIAnimator = GetComponent<Animator>();
        monsterStats = GetComponent<MonsterStats>();
        canWanderinitial = canWander;
        isAlive = true;
        onGround = true;
        IsMovingToNext = true;
        rested = false;
        agentORGspeed = Agent.speed;
        InternalWaitTime = WanderIdleTime;
        if (canWander)
        {
            aIAnimator.SetBool("isMoving", true);
        }
        StartCoroutine(StartWandering());
    }
    private void Update()
    {
        if (canWander && isAlive)
        {

            if (!IsMovingToNext && !chasingPlayer)
            {
                HasSetNextPosition = false;

                InternalWaitTime -= Time.deltaTime;

                if (InternalWaitTime < 0f)
                {
                    if (Random.value < restedChance && !rested)
                    {
                        IsMovingToNext = true;
                        StartResting();
                    }
                    else
                    {
                        IsMovingToNext = true;
                        InternalWaitTime = WanderIdleTime;
                        aIAnimator.SetBool("isMoving", true);
                        rested = false;
                        StartCoroutine(StartWandering());
                    }
                }
            }

            if (HasSetNextPosition && !chasingPlayer)
            {
                StartCoroutine(CheckAgentDistance());
            }

            if (chasingPlayer && isAlive)
            {
                if (aIAnimator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                {
                    rested = false;
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
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dodgeMovement = -transform.forward * dodgeAmount;
            StartCoroutine(CharacterControllerDodging.PerformDodgeMovement(Agent, dodgeMovement, dodgeDuration, transform));
        }
    }

    public void StartResting()
    {
        aIAnimator.SetBool("Resting", true);
        if (!doesFly)
        {
            float minWaitTime = 10f;
            float maxWaitTime = 30f;
            float randomWaitTime = Random.Range(minWaitTime, maxWaitTime);
            StartCoroutine(StopRestingAfterDelay(randomWaitTime));
        }
        if (doesFly)
        {
            StartCoroutine(ChangeBaseOffsetOverTime());
            IsMovingToNext = true;
            InternalWaitTime = WanderIdleTime;
            rested = true;
            StartCoroutine(StartWandering());
        }
    }

    private IEnumerator StopRestingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        aIAnimator.SetBool("Resting", false);
        IsMovingToNext = false;
        rested = true;
    }

    private IEnumerator CheckAgentDistance()
    {
        yield return new WaitForSeconds(1f);
        bool hasReachedDestination = Agent.remainingDistance <= Agent.stoppingDistance;
        if (!doesFly && hasReachedDestination)
        {
            IsMovingToNext = false;
            aIAnimator.SetBool("isMoving", false);
            InternalWaitTime = WanderIdleTime;
        }
        if (doesFly && hasReachedDestination && rested)
        {
            IsMovingToNext = false;
            aIAnimator.SetBool("Resting", false);
            onGround = Mathf.Approximately(Agent.baseOffset, 0f);
            Agent.baseOffset = onGround ? 0f : flyingHeight;
            StartCoroutine(ChangeBaseOffsetOverTime());
            InternalWaitTime = WanderIdleTime;
        }
        if (doesFly && hasReachedDestination && !rested)
        {
            IsMovingToNext = false;
            aIAnimator.SetBool("isMoving", false);
            InternalWaitTime = WanderIdleTime;
        }
    }
    private IEnumerator ChangeBaseOffsetOverTime()
    {
        float startBaseOffset = Agent.baseOffset;
        float targetBaseOffset = onGround ? flyingHeight : 0f;

        if (Mathf.Approximately(startBaseOffset, targetBaseOffset))
        {
            onGround = Mathf.Approximately(targetBaseOffset, 0f);
            yield return null;
        }

        float duration = flyingHeightChangeSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float newOffset = Mathf.Lerp(startBaseOffset, targetBaseOffset, t);
            Agent.baseOffset = newOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Agent.baseOffset = targetBaseOffset;
        onGround = Mathf.Approximately(targetBaseOffset, 0f);
    }



    public void PlayerDetected(GameObject player)
    {
        if (isAlive)
        {
            aIAnimator.SetBool("Resting", false);
            if (doesFly && rested)
            {
                IsMovingToNext = false;
                bool isOnGround = Mathf.Abs(Agent.baseOffset - 0f) <= tolerance;
                Agent.baseOffset = isOnGround ? 0f : flyingHeight;
                StartCoroutine(ChangeBaseOffsetOverTime());
                InternalWaitTime = WanderIdleTime;
            }
            if (ShowHealthBar)
            {
                monsterStats.ActivateHealthBar();
            }
            canWander = true;
            targetPlayer = player;
            chasingPlayer = true;
            if (increaseChaseSpeed)
            {
                Agent.speed *= 2f;
            }
            monsterStats.StopHealthRegen();
            aIAnimator.SetTrigger("PlayerFound");
        }
    }

    public void PlayerLost()
    {
        if (isAlive)
        {
            if (ShowHealthBar)
            {
                monsterStats.DeActivateHealthBar();
            }
            canWander = canWanderinitial;
            chasingPlayer = false;
            if (!canWander)
            {
                Agent.SetDestination(transform.position);
            }
            aIAnimator.SetTrigger("PlayerLost");
            if (increaseChaseSpeed)
            {
                Agent.speed = agentORGspeed;
            }
            monsterStats.HealthRegen();
        }
    }

    public void PlayerInAttackRange()
    {
        if (isAlive)
        {
            aIAnimator.SetBool("PlayerInAttackRange", true);
        }
    }

    public void PlayerOutOfAttackRange()
    {
        if (isAlive)
        {
            aIAnimator.SetBool("PlayerInAttackRange", false);
        }
    }
    public void PlayerAttacked(GameObject player)
    {
        if (isAlive)
        {
            if (playerStats == null)
            {
                playerStats = player.GetComponent<PlayerStats>();
            }
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
    public void TookDamage(bool isranged, GameObject target)
    {
        if (isAlive)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                aIAnimator.SetTrigger("MobHit");
                timer = 0f;
            }
            if (isranged)
            {
                if(playerStats == null)
                {
                    playerStats = target.GetComponent<PlayerStats>();
                }
                if (playerdamagereduction == 0)
                {
                    playerdamagereduction = playerStats.ReduceDamage();
                    if (Damage <= playerdamagereduction)
                    {
                        monsterStats.SetMonsterStatsOverleveled();
                    }
                }
                if (!chasingPlayer)
                {
                    targetPlayer = PlayerDetection.FindChildWithTag(target.transform, "MainCamera");
                    PlayerDetected(targetPlayer);
                }
            }
        }
    }

    public IEnumerator StartWandering()
    {
        if (doesFly && rested)
        {
            while (!aIAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fly"))
            {
                yield return null;
            }
            SetAIDestination(CalculateRandomPosition(transform, MaxWanderDistance, MinWanderDistance));
        }

        if (!doesFly || (doesFly && !rested))
        {
            while (!aIAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                yield return null;
            }
            SetAIDestination(CalculateRandomPosition(transform, MaxWanderDistance, MinWanderDistance));
        }
    }

    public void SetAIDestination(Vector3 target)
    {
        Agent.SetDestination(target);
        if (Agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            currentRetries++;
            if (currentRetries <= MaxRetries)
            {
                SetAIDestination(CalculateRandomPosition(transform, MaxWanderDistance, MinWanderDistance));
            }
            else
            {
                MaxWanderDistance *= 2;
            }
        }
        else
        {
            currentRetries = 0;
            HasSetNextPosition = true;
        }
    }

    public void Death()
    {
        isAlive = false;
        Agent.isStopped = true;

        foreach (AnimatorControllerParameter parameter in aIAnimator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                aIAnimator.SetBool(parameter.name, false);
            }
            else if (parameter.type == AnimatorControllerParameterType.Trigger)
            {
                aIAnimator.ResetTrigger(parameter.name);
            }
        }
        aIAnimator.SetTrigger("Death");
    }
}