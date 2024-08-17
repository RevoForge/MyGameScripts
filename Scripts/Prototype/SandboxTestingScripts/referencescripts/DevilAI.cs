using Revo.Methods;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static Revo.Methods.ObjectInteraction;

public class DevilAI : MonoBehaviour, IMonsterAI, IPlayerDetectionAI
{
    public int Damage { get; set; }
    public bool ShowHealthBar { get; set; }
    public GameObject[] castObjects;
    public GameObject[] screamObjects;
    public float dodgeAmount = 1.0f;
    public float dodgeDuration = 0.5f;
    private Vector3 dodgeMovement;
    private PlayerStats playerStats;
    private GameObject player;
    private Transform spawn;
    private Animator animator;
    private bool chasingPlayer;
    [HideInInspector] public bool isAlive;
    private MonsterStats monsterStats;
    private float timer = 0f;
    private readonly float interval = 3f;
    private bool isHealing;
    private NavMeshAgent meshAgent;
    private bool hasDestination;
    private int playerdamagereduction = 0;

    private void OnEnable()
    {
        isAlive = true;
        animator = GetComponent<Animator>();
        monsterStats = GetComponent<MonsterStats>();
        chasingPlayer = false;
        meshAgent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (spawn == null)
        {
            spawn = transform.parent;
        }
        if (chasingPlayer && isAlive)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("locomotion"))
            {
                meshAgent.SetDestination(player.transform.position);
                if (meshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    hasDestination = false;
                    monsterStats.HealthRegen();
                }
                else if (!hasDestination)
                {
                    hasDestination = true;
                    monsterStats.StopHealthRegen();
                }
            }
            if (hasDestination)
            {
                meshAgent.SetDestination(player.transform.position);
                if (meshAgent.remainingDistance >= meshAgent.stoppingDistance)
                {
                    animator.SetFloat("locomotion", 1f);
                }
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    animator.SetFloat("locomotion", 0f);
                    Vector3 targetDirection = player.transform.position - transform.position;
                    targetDirection.y = 0;

                    if (targetDirection.sqrMagnitude > 0 * 0)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, meshAgent.angularSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }

    public IEnumerator PlayerDetected(GameObject target)
    {
        if (isAlive && !chasingPlayer)
        {
            player = target;
            animator.Play("Base Layer.scream");
            if (ShowHealthBar)
            {
                monsterStats.ActivateHealthBar();
            }
            yield return new WaitForSeconds(0.5f);
            chasingPlayer = true;
        }
        yield break;
    }
    public void AttackZoneDetection(AttackZone attackZone)
    {
        if (isAlive && !isHealing)
        {
            switch (attackZone)
            {
                case AttackZone.Front:
                    if (CanPerformAction())
                    {
                        int[] weights = { 5, 5, 3, 1, 1 };
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
                                        animator.SetTrigger("attack1");
                                        break;
                                    case 1:
                                        animator.SetTrigger("attack2");
                                        break;
                                    case 2:
                                        animator.SetTrigger("attack3");
                                        break;
                                    case 3:
                                        animator.SetTrigger("attack4");
                                        break;
                                    case 4:
                                        animator.SetTrigger("taunt");
                                        break;
                                }
                                break;
                            }
                        }
                    }
                    break;
                case AttackZone.Back:
                    animator.SetTrigger("attack4");
                    break;
                case AttackZone.Left:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack4"))
                    {
                        animator.SetTrigger("jumpRight");
                        dodgeMovement = transform.right * dodgeAmount;
                        StartCoroutine(CharacterControllerDodging.PerformDodgeMovement(meshAgent, dodgeMovement, dodgeDuration, transform));
                    }
                    break;
                case AttackZone.Right:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack4"))
                    {
                        animator.SetTrigger("jumpLeft");
                        dodgeMovement = -transform.right * dodgeAmount;
                        StartCoroutine(CharacterControllerDodging.PerformDodgeMovement(meshAgent, dodgeMovement, dodgeDuration, transform));
                    }
                    break;
                case AttackZone.Center:
                    animator.SetTrigger("dodge");
                    dodgeMovement = -transform.forward * dodgeAmount;
                    StartCoroutine(CharacterControllerDodging.PerformDodgeMovement(meshAgent, dodgeMovement, dodgeDuration, transform));
                    break;
            }
        }
    }
    private bool CanPerformAction()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = currentState.IsName("attack1") || currentState.IsName("attack2") || currentState.IsName("attack3") || currentState.IsName("attack4");
        bool isJumping = currentState.IsName("jumpRight") || currentState.IsName("jumpLeft");
        bool isDodging = currentState.IsName("dodge");
        bool isTuanting = currentState.IsName("tuant");

        return !isAttacking && !isJumping && !isDodging && !isTuanting;
    }


    public void PlayerLost()
    {
        if (isAlive)
        {
            animator.SetFloat("locomotion", 0);
            chasingPlayer = false;
            hasDestination = false;
            meshAgent.SetDestination(transform.position);
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
                animator.SetTrigger("gotHit");

                timer = 0f;
            }
            if (isranged)
            {
                if (!chasingPlayer)
                {
                    animator.SetFloat("locomotion", 1);
                    if (ShowHealthBar)
                    {
                        monsterStats.ActivateHealthBar();
                    }
                    StartCoroutine(PlayerDetected(PlayerDetection.FindChildWithTag(target.transform, "MainCamera")));
                }
            }
        }
    }

    public void StartHealing()
    {
        animator.SetTrigger("castStart");
        isHealing = true;
    }
    public void StopHealing()
    {
        animator.SetTrigger("castEnd");
        isHealing = false;
    }


    public void StartCast()
    {
        for (int i = 0; i < castObjects.Length; i++)
        {
            if (castObjects[i].GetComponent<ParticleSystem>())
            {
                ParticleSystem ps = castObjects[i].GetComponent<ParticleSystem>();
                var em = ps.emission;
                em.enabled = true;
            }
            else if (castObjects[i].GetComponent<Light>())
            {
                castObjects[i].GetComponent<Light>().enabled = true;
            }
        }
    }

    public void StopCast()
    {
        for (int i = 0; i < castObjects.Length; i++)
        {
            if (castObjects[i].GetComponent<ParticleSystem>())
            {
                ParticleSystem ps = castObjects[i].GetComponent<ParticleSystem>();
                var em = ps.emission;
                em.enabled = false;
            }
            else if (castObjects[i].GetComponent<Light>())
            {
                castObjects[i].GetComponent<Light>().enabled = false;
            }
        }
    }

    public void StartScream()
    {
        for (int i = 0; i < screamObjects.Length; i++)
        {
            if (screamObjects[i].GetComponent<ParticleSystem>())
            {
                ParticleSystem ps = screamObjects[i].GetComponent<ParticleSystem>();
                var em = ps.emission;
                em.enabled = true;
            }
        }
    }

    public void StopScream()
    {
        for (int i = 0; i < screamObjects.Length; i++)
        {
            if (screamObjects[i].GetComponent<ParticleSystem>())
            {
                ParticleSystem ps = screamObjects[i].GetComponent<ParticleSystem>();
                var em = ps.emission;
                em.enabled = false;
            }
        }
    }
    public void Death()
    {
        meshAgent.isStopped = true;
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
        animator.SetFloat("locomotion", 0f);
        animator.SetTrigger("death");
    }
}
