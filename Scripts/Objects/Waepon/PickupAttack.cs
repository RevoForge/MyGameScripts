using System.Collections;
using UnityEngine;
using Valve.VR;
using static Revo.Methods.ObjectInteraction;

public class PickupAttackMonster : MonoBehaviour
{
    private GameObject skillObject;
    private GameObject player;
    private PlayerStats playerstats;
    private AudioSource swordAudio;
    public int baseWeaponDamage = 10;
    private bool canDamage = false;
    private SteamVR_Input_Sources right;
    private SteamVR_Input_Sources left;
    private SteamVR_Input_Sources hand;
    [HideInInspector] public bool isRight;
    private bool canDamageSkill;
    public DamageType selectedDamageType;
    [HideInInspector] public bool isHeld;
    public bool pickupRotation;
    public Vector3 rotationAmount;
    private PickupObjectScript objectScript;

    private void OnEnable()
    {
        right = SteamVR_Input_Sources.RightHand;
        left = SteamVR_Input_Sources.LeftHand;
        skillObject = transform.Find("SkillEffect").gameObject;
        objectScript = GetComponent<PickupObjectScript>();
        isHeld = false;
        canDamageSkill = true;
    }
    public void PlayerPickup(GameObject playerobject)
    {
        player = playerobject;
        playerstats = player.GetComponentInParent<PlayerStats>();
        swordAudio = GetComponent<AudioSource>();
        canDamage = true;
        hand = isRight ? right : left;
    }
    void Update()
    {
        if (isHeld && canDamageSkill && SteamVR_Input.GetState("InteractUI", hand))
        {
            if (skillObject != null) { skillObject.SetActive(true); }
            canDamageSkill = false;
            baseWeaponDamage *= 3;
            StartCoroutine(ResetSkillTrigger());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mob") && canDamage && (objectScript.objectVelocityL.sqrMagnitude >= 0.15f || objectScript.objectVelocityR.sqrMagnitude >= 0.15f))
        {
            MonsterStats monsterStats = other.GetComponentInParent<MonsterStats>();
            if (monsterStats != null)
            {
                playerstats.AttackMonster(monsterStats, baseWeaponDamage, false, (int)selectedDamageType);
            }
            swordAudio.Play();
            canDamage = false;
            StartCoroutine(ResetTrigger());
        }
    }
    IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        canDamage = true;
    }
    public IEnumerator ResetSkillTrigger()
    {
        yield return new WaitForSeconds(3f);
        baseWeaponDamage /= 3;
        if (skillObject != null) { skillObject.SetActive(false); }
        yield return new WaitForSeconds(30f);
        canDamageSkill = true;
    }

}
