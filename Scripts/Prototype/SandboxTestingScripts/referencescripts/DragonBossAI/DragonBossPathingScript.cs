using Revo.Methods;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Revo.Methods.ObjectInteraction;

public class DragonBossPathingScript : MonoBehaviour, IMonsterAI
{
    public int Damage { get; set; }
    public bool ShowHealthBar { get; set; }
    public Transform[] destinations;
    public float offsetChangeSpeed = 1.0f;
    public float offset1 = 0.0f;
    public float offset2 = 2.0f;

    private NavMeshAgent Agent;
    private int currentDestinationIndex;
    private float currentOffset;
    private bool isMovingToOffset2;
    private Animator animator;
    private bool usedNavLink;
    private int offMeshLinkCount = 0;
    private Transform skeleton;
    private bool isChasing;
    private GameObject targetPlayer;
    [HideInInspector]
    public bool isAlive;
    private float timer = 0f;
    private readonly float interval = 3f;
    private MonsterStats monsterStats;
    private PlayerStats playerStats;
    public bool increaseChaseSpeed;
    private bool landed;
    private int playerdamagereduction = 0;

    private void OnEnable()
    {
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        monsterStats = GetComponent<MonsterStats>();
        skeleton = transform.Find("root");
        currentDestinationIndex = 0;
        currentOffset = offset1;
        isMovingToOffset2 = true;
        usedNavLink = false;
        landed = false;
        isChasing = false;

        if (destinations.Length > 0)
        {
            Agent.SetDestination(destinations[currentDestinationIndex].position);
        }
    }

    private void Update()
    {
        if (!isChasing)
        {

            if (Agent.pathPending || !Agent.hasPath)
                return;
            if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                currentDestinationIndex++;
                if (currentDestinationIndex >= destinations.Length)
                {
                    currentDestinationIndex = 0;
                }

                Agent.SetDestination(destinations[currentDestinationIndex].position);
                animator.SetTrigger("Fly");
            }
        }
        if (isChasing)
        {
            Agent.SetDestination(targetPlayer.transform.position);
            if (Agent.remainingDistance > Agent.stoppingDistance && landed && Mathf.Approximately(currentOffset, offset1) && !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.SetTrigger("Walk");
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
        if (Agent.isOnOffMeshLink)
        {
            if (!usedNavLink)
            {
                if (offMeshLinkCount == 0)
                {
                    animator.SetTrigger("Falling");
                    StartCoroutine(RotateOverTime(-135f));
                    usedNavLink = true;
                    offMeshLinkCount++;
                }
                else if (offMeshLinkCount == 1)
                {
                    StartCoroutine(RotateOverTime(-135f));
                    Agent.speed /= 2;
                    usedNavLink = true;
                    offMeshLinkCount = 0;
                    landed = false;
                }
            }
        }
        else if (!isChasing || !landed)
        {
            if (usedNavLink)
            {
                StartCoroutine(RotateOverTime(-90f));
                if (offMeshLinkCount != 0)
                {
                    animator.SetTrigger("Fly");
                }
                else
                {
                    Agent.speed *= 2;
                }
                usedNavLink = false;
                landed = true;
            }
        }
        if (!isChasing || (isChasing && (!Mathf.Approximately(currentOffset, offset1))))
        {
            float targetOffset = isChasing ? offset1 : (usedNavLink ? offset1 : (isMovingToOffset2 ? offset2 : offset1));
            float finaloffsetChangeSpeed = usedNavLink ? offsetChangeSpeed * 5 : offsetChangeSpeed;
            if (currentOffset != targetOffset)
            {
                float newOffset = Mathf.MoveTowards(currentOffset, targetOffset, finaloffsetChangeSpeed * Time.deltaTime);
                currentOffset = newOffset;
                Agent.baseOffset = currentOffset;
            }
            if (currentOffset == offset2 && isMovingToOffset2)
            {
                isMovingToOffset2 = false;
            }
            else if (currentOffset == offset1 && !isMovingToOffset2)
            {
                isMovingToOffset2 = true;
            }
        }

    }
    IEnumerator RotateOverTime(float targetXRotation)
    {
        float duration = 1.0f;
        float elapsedTime = 0.0f;

        Quaternion startRotation = skeleton.localRotation;
        Quaternion targetRotation = Quaternion.Euler(targetXRotation, startRotation.eulerAngles.y, startRotation.eulerAngles.z);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            skeleton.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        skeleton.localRotation = targetRotation;
    }
    public void TookDamage(bool isranged, GameObject target)
    {
        if (isAlive)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                animator.SetTrigger("gotHit");

                timer = 0f;
            }
            if (isranged)
            {
                if (!isChasing)
                {
                    if (ShowHealthBar)
                    {
                        monsterStats.ActivateHealthBar();
                    }
                    PlayerDetected(PlayerDetection.FindChildWithTag(target.transform, "MainCamera"));
                }
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
            targetPlayer = player;
            isChasing = true;
            Agent.speed = increaseChaseSpeed ? (Agent.speed * 2f) : Agent.speed;
        }
    }

    public void PlayerLost()
    {
        if (isAlive)
        {
            isChasing = false;
            Agent.speed = increaseChaseSpeed ? (Agent.speed / 2f) : Agent.speed;
            if (ShowHealthBar)
            {
                monsterStats.DeActivateHealthBar();
            }
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
        animator.SetTrigger("Death");
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
}
