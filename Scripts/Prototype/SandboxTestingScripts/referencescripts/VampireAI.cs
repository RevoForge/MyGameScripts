using Revo.Methods;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static Revo.Methods.ObjectInteraction;

public class VampireAI : MonoBehaviour, IMonsterAI, IPlayerDetectionAI
{
    private Animator animator;
    private MonsterStats monsterStats;
    private NavMeshAgent navMeshAgent;
    [Tooltip("This is the max distance the AI will wander from its current position at once!")]
    public float MaxWanderDistance = 2f;
    [Tooltip("This is the minimum distance the AI will wander from its current position at once!")]
    public float MinWanderDistance = 0.5f;
    public int Damage { get; set; }
    public bool ShowHealthBar { get; set; }
    public float dodgeAmount = 1.0f;
    public float dodgeDuration = 0.5f;
    private Vector3 dodgeMovement;
    private PlayerStats playerStats;
    private GameObject player;
    private Transform spawn;
    private bool chasingPlayer;
    [HideInInspector] public bool isAlive;
    private float timer = 0f;
    private readonly float interval = 3f;
    private bool hasDestination;
    private int playerdamagereduction = 0;
    private AudioSource audioSource;
    private bool HasSetNextPosition;
    private bool distanceChecking;
    private bool IsMovingToNext;
    private float InternalWaitTime = 5;

    private void Start()
    {
        animator = GetComponent<Animator>();
        monsterStats = GetComponent<MonsterStats>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        isAlive = true;
        chasingPlayer = false;
    }
    private void Update()
    {
        if (spawn == null)
        {
            spawn = transform.parent;
        }
        if (navMeshAgent.speed != 3.5f && animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            navMeshAgent.speed = 3.5f;
        }
        if (navMeshAgent.speed != 1.75f && animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            navMeshAgent.speed = 1.75f;
        }
        if (!IsMovingToNext && !chasingPlayer)
        {
            HasSetNextPosition = false;

            InternalWaitTime -= Time.deltaTime;

            if (InternalWaitTime < 0f)
            {
                InternalWaitTime = 30;
                IsMovingToNext = true;
                animator.SetTrigger("walk");
                StartCoroutine(StartWandering());
            }
        }
        if (HasSetNextPosition && !chasingPlayer && !distanceChecking)
        {
            distanceChecking = true;
            StartCoroutine(CheckAgentDistance());
        }
        if (chasingPlayer && isAlive)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                navMeshAgent.SetDestination(player.transform.position);
                if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    monsterStats.HealthRegen();
                    hasDestination = false;
                }
                else if (!hasDestination)
                {
                    hasDestination = true;
                    monsterStats.StopHealthRegen();
                }
            }
            if (hasDestination)
            {
                navMeshAgent.SetDestination(player.transform.position);
                if (navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance && !animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                {
                    animator.SetTrigger("run");
                }
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                    {
                        animator.ResetTrigger("run");
                        animator.SetTrigger("stoprun");
                    }
                    Vector3 targetDirection = player.transform.position - transform.position;
                    targetDirection.y = 0;

                    if (targetDirection.sqrMagnitude > 0 * 0)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, navMeshAgent.angularSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }
    private IEnumerator CheckAgentDistance()
    {
        yield return new WaitForSeconds(0.1f);
        while (!HasReachedDestination())
        {
            yield return new WaitForSeconds(0.5f);
        }
        animator.SetTrigger("stopwalk");
        IsMovingToNext = false;
        distanceChecking = false;
    }
    private bool HasReachedDestination()
    {
        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }
    public void AttackZoneDetection(AttackZone attackZone)
    {
        if (isAlive)
        {
            switch (attackZone)
            {
                case AttackZone.Front:
                    // Do Something for Front Trigger
                    if (CanPerformAction())
                    {
                        int[] weights = { 5, 5, 2 };
                        int totalWeight = weights.Sum();

                        int randomTrigger = Random.Range(0, totalWeight);

                        int cumulativeWeight = 0;
                        for (int i = 0; i < weights.Length; i++)
                        {
                            cumulativeWeight += weights[i];
                            if (randomTrigger < cumulativeWeight)
                            {
                                switch (i)
                                {
                                    case 0:
                                        animator.SetTrigger("attackleft");
                                        break;
                                    case 1:
                                        animator.SetTrigger("attackright");
                                        break;
                                    case 2:
                                        animator.SetTrigger("attack2hand");
                                        break;
                                }
                                break;
                            }
                        }
                    }
                    break;
                case AttackZone.Back:
                    // Do Something for Back Trigger
                    break;
                case AttackZone.Left:
                    // Do Something for Left Trigger
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("StrafeRight"))
                    {
                        animator.SetTrigger("straferight");
                        dodgeMovement = transform.right * dodgeAmount;
                        StartCoroutine(CharacterControllerDodging.PerformDodgeMovement(navMeshAgent, dodgeMovement, dodgeDuration, transform));
                    }
                    break;
                case AttackZone.Right:
                    // Do Something for Right Trigger
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("StrafeLeft"))
                    {
                        animator.SetTrigger("strafeleft");
                        dodgeMovement = -transform.right * dodgeAmount;
                        StartCoroutine(CharacterControllerDodging.PerformDodgeMovement(navMeshAgent, dodgeMovement, dodgeDuration, transform));
                    }
                    break;
                case AttackZone.Center:
                    // Do Something for Center Trigger
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("JumpBack"))
                    {
                        animator.SetTrigger("jumpback");
                        dodgeMovement = -transform.forward * dodgeAmount;
                        StartCoroutine(CharacterControllerDodging.PerformDodgeMovement(navMeshAgent, dodgeMovement, dodgeDuration, transform));
                    }
                    break;
            }
        }
    }
    private bool CanPerformAction()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = currentState.IsName("AttackLeft") || currentState.IsName("AttackRight") || currentState.IsName("Attack2Hand");
        bool isStrafing = currentState.IsName("StrafeRight") || currentState.IsName("StrafeLeft");
        bool isDodging = currentState.IsName("JumpBack");

        return !isAttacking && !isStrafing && !isDodging;
    }
    public void PlayerLost()
    {
        if (isAlive)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                animator.ResetTrigger("run");
                animator.SetTrigger("stoprun");
            }
            chasingPlayer = false;
            hasDestination = false;
            navMeshAgent.SetDestination(transform.position);
            monsterStats.HealthRegen();
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
    public void TookDamage(bool isranged, GameObject target)
    {
        if (isAlive)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                animator.SetTrigger("gothit");

                timer = 0f;
            }
            if (isranged)
            {
                if (!chasingPlayer)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                    {
                        animator.SetTrigger("run");
                    }
                    if (ShowHealthBar)
                    {
                        monsterStats.ActivateHealthBar();
                    }
                    StartCoroutine(PlayerDetected(PlayerDetection.FindChildWithTag(target.transform, "MainCamera")));
                }
            }
        }
    }
    public IEnumerator PlayerDetected(GameObject target)
    {
        if (isAlive && !chasingPlayer)
        {
            animator.SetTrigger("stopwalk");
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            player = target;
            if (ShowHealthBar)
            {
                monsterStats.ActivateHealthBar();
            }
            yield return new WaitForSeconds(0.5f);
            chasingPlayer = true;

            animator.SetTrigger("run");
        }
        yield break;
    }
    public void Death()
    {
        navMeshAgent.isStopped = true;
        isAlive = false;
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
        animator.ResetTrigger("run");
        animator.SetTrigger("stoprun");
        animator.SetTrigger("death");
    }
    public IEnumerator StartWandering()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            yield return null;
        }
        SetAIDestination(CalculateRandomPosition(transform, MaxWanderDistance, MinWanderDistance));
    }

    public void SetAIDestination(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
                SetAIDestination(CalculateRandomPosition(transform, MaxWanderDistance, MinWanderDistance));
        }
        else
        {
            HasSetNextPosition = true;
        }
    }
}
