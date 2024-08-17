using System.Collections;
using UnityEngine;
using Valve.VR;
using static Revo.Methods.ObjectInteraction;

public class WandAttackMonster : MonoBehaviour
{
    public GameObject spellPrefab;
    public GameObject castPrefab;
    public GameObject projectilePrefab;
    public DamageType selectedDamageType;
    public float projectileCastTime = 1;
    public float movingSpeed = 1f;
    public float spellLifeTime = 1f;
    public int damage;
    public int coolDown = 1;
    private Transform castSpot;
    private GameObject player;
    private PlayerStats mainAI;
    private bool canCast;
    private bool canCastSkill;
    private SteamVR_Input_Sources right;
    private SteamVR_Input_Sources left;
    private SteamVR_Input_Sources hand;
    [HideInInspector] public bool isRight;
    private GameObject skillObject;
    public bool pickupRotation;
    public Vector3 rotationAmount;
    private GameObject createdProjectile;
    public bool oldSystem = true;


    private void OnEnable()
    {
        castSpot = transform.Find("CastSpot");
        right = SteamVR_Input_Sources.RightHand;
        left = SteamVR_Input_Sources.LeftHand;
        skillObject = transform.Find("SkillEffect").gameObject;
    }

    public void PlayerPickup(GameObject playerobject)
    {
        player = playerobject;
        mainAI = player.GetComponentInParent<PlayerStats>();
        hand = isRight ? right : left;
        canCast = true;
        canCastSkill = true;
        if (pickupRotation)
        {
            Quaternion EulerRotationAmount = Quaternion.Euler(rotationAmount.x, rotationAmount.y, rotationAmount.z);
            gameObject.transform.localRotation = EulerRotationAmount;
        }
    }

    public void PlayerDrop()
    {
        mainAI = null;
    }

    void Update()
    {
        if (mainAI != null)
        {
            if (canCast && SteamVR_Input.GetStateDown("InteractUI", hand) && !SteamVR_Input.GetState("Skill", SteamVR_Input_Sources.Any))
            {
                canCast = false;
                if (oldSystem)
                {
                    CastMagic();
                    StartCoroutine(ResetTrigger());
                }
                else
                {
                    CastProjectile();
                }
            }
            if (canCastSkill && SteamVR_Input.GetState("Skill", SteamVR_Input_Sources.Any) && SteamVR_Input.GetStateDown("InteractUI", hand))
            {
                if (skillObject != null) { skillObject.SetActive(true); }
                canCastSkill = false;
                damage *= 3;
                coolDown /= 3;
                StartCoroutine(ResetSkillTrigger());
            }
        }
    }

    private void CastMagic()
    {
        var newSpell = Instantiate(spellPrefab, castSpot.position, castSpot.rotation);
        var magic = newSpell.GetComponent<PlayerMagicMoving>();
        magic.mainAI = mainAI;
        magic.speed = movingSpeed;
        magic.spellTime = spellLifeTime;
        magic.baseDamage = damage;
    }
    private void CastProjectile()
    {
        createdProjectile = Instantiate(castPrefab, castSpot.position, castSpot.rotation,castSpot);
        StartCoroutine(ProjectileCasting());
    }
    private IEnumerator ProjectileCasting()
    {
        yield return new WaitForSeconds(projectileCastTime);
        Instantiate(projectilePrefab, createdProjectile.transform.position, createdProjectile.transform.rotation);
        var magic = projectilePrefab.GetComponent<Projectile>();
        magic.mainAI = mainAI;
        magic.selectedDamageType = selectedDamageType;
        magic.speed = movingSpeed;
        magic.baseDamage = damage;
        Destroy(createdProjectile);
        canCast = true;
    }
    private IEnumerator ResetTrigger() // Old System
    {
        yield return new WaitForSeconds(coolDown);
        canCast = true;
    }
    private IEnumerator ResetSkillTrigger()
    {
        yield return new WaitForSeconds(3f);
        if (skillObject != null) { skillObject.SetActive(false); }
        damage /= 3;
        coolDown *= 3;
        yield return new WaitForSeconds(30f);
        canCastSkill = true;
    }
}
