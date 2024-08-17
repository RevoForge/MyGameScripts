using Revo.Methods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using static Revo.Methods.ObjectInteraction;

public class PickupObjectScript : MonoBehaviour
{
    public float WeaponSwingTest = 1f;
    private bool isBeingCarriedR = false;
    private bool isBeingCarriedL = false;
    private Rigidbody rb;
    public bool isWeapon;
    public bool isMagic;
    public float pickupDuration = 100f;
    private PickupAttackMonster weapon;
    private WandAttackMonster magic;
    private GameObject player;
    private GameObject itemHolderR;
    private GameObject itemHolderL;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private readonly int bufferSize = 15;
    private Queue<Vector3> positionsR;
    [HideInInspector] public Vector3 objectVelocityR;
    private Queue<Vector3> positionsL;
    [HideInInspector] public Vector3 objectVelocityL;
    public int thrownMultiplier = 10;
    private Transform rightSlot;
    private Transform leftSlot;
    private bool itemSlotDetectedR;
    private bool itemSlotDetectedL;
    private bool inventorySlotDetected;
    private InventorySlot inventorySlots;
    public WeaponTypes weaponType;
    private string inventoryIcon;


    private void Start()
    {
        positionsR = new Queue<Vector3>(bufferSize);
        positionsL = new Queue<Vector3>(bufferSize);
        inventoryIcon = GetStringForWeaponType(weaponType);
    }

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        weapon = GetComponent<PickupAttackMonster>();
        magic = GetComponent<WandAttackMonster>();
        itemHolderR = GameObject.Find("ItemHolderR");
        itemHolderL = GameObject.Find("ItemHolderL");
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void ResetPositionAndRotation()
    {
        transform.SetPositionAndRotation(startPosition, startRotation);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isBeingCarriedR && !isBeingCarriedL)
        {
            if (other.CompareTag("RightHand") && SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.RightHand))
            {
                isBeingCarriedR = true;
                player = other.gameObject;
                if (magic != null) { magic.isRight = true; }
                if (weapon != null) { weapon.isRight = true; weapon.isHeld = true; }
                ObjectInteraction.PickupObject(player, itemHolderR.transform, transform, rb, isWeapon, isMagic, weapon, magic, this);
            }

            if (other.CompareTag("LeftHand") && SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.LeftHand))
            {
                isBeingCarriedL = true;
                player = other.gameObject;
                if (magic != null) { magic.isRight = false; }
                if (weapon != null) { weapon.isRight = false; weapon.isHeld = true; }
                ObjectInteraction.PickupObject(player, itemHolderL.transform, transform, rb, isWeapon, isMagic, weapon, magic, this);
            }
        }

        if (other.CompareTag("RightSlot") && !itemSlotDetectedR)
        {
            rightSlot = other.transform;
            itemSlotDetectedR = true;
        }
        if (other.CompareTag("LeftSlot") && !itemSlotDetectedL)
        {
            leftSlot = other.transform;
            itemSlotDetectedL = true;
        }
        if (other.CompareTag("InventorySlot") && !inventorySlotDetected)
        {
            inventorySlots = other.transform.GetComponent<InventorySlot>();
            inventorySlotDetected = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RightSlot"))
        {
            itemSlotDetectedR = false;
        }
        if (other.CompareTag("LeftSlot"))
        {
            itemSlotDetectedL = false;
        }
        if (other.CompareTag("InventorySlot"))
        {
            inventorySlotDetected = false;
        }
    }

    private void Update()
    {
        if (isBeingCarriedR)
        {
            objectVelocityR = PositionCalculator.CalculateObjectVelocity(this.transform, positionsR, bufferSize);
        }
        if (isBeingCarriedL)
        {
            objectVelocityL = PositionCalculator.CalculateObjectVelocity(this.transform, positionsL, bufferSize);
        }

        if (isBeingCarriedR && SteamVR_Input.GetStateUp("GrabGrip", SteamVR_Input_Sources.RightHand))
        {
            HandleRelease(true, itemSlotDetectedR);
        }

        if (isBeingCarriedL && SteamVR_Input.GetStateUp("GrabGrip", SteamVR_Input_Sources.LeftHand))
        {
            HandleRelease(false, itemSlotDetectedL);
        }
    }
    private void HandleRelease(bool isRightHand, bool itemSlotDetected)
    {
        if (itemSlotDetected)
        {
            transform.SetParent(isRightHand ? rightSlot : leftSlot);
            transform.localPosition = Vector3.zero;
        }
        else if (inventorySlotDetected && !inventorySlots.itemStored && weaponType != WeaponTypes.None)
        {
            inventorySlots.ItemDeposited(inventoryIcon, CleanPrefabName(gameObject.name));
            Destroy(gameObject);
        }
        else
        {
            if (weapon != null) { weapon.isHeld = false; }
            ObjectInteraction.ReleaseObject(isMagic, rb, magic, transform, isRightHand ? objectVelocityR : objectVelocityL, thrownMultiplier);
        }

        if (isRightHand) isBeingCarriedR = false;
        else isBeingCarriedL = false;
    }
    private string CleanPrefabName(string name)
    {
        // Remove the "(Clone)" suffix
        if (name.EndsWith("(Clone)"))
        {
            name = name.Substring(0, name.Length - 7); // Remove the "(Clone)" part
        }

        return name;
    }
    public void StartMoveToHand(Vector3 initialPosition, Quaternion initialRotation)
    {
        Quaternion EulerRotationAmount = Quaternion.identity;
        if (weapon != null && weapon.pickupRotation)
        {
            Vector3 rotationAmount = weapon.rotationAmount;
            EulerRotationAmount = Quaternion.Euler(rotationAmount.x, rotationAmount.y, rotationAmount.z);
        }
        else if (magic != null && magic.pickupRotation)
        {
            Vector3 rotationAmount = magic.rotationAmount;
            EulerRotationAmount = Quaternion.Euler(rotationAmount.x, rotationAmount.y, rotationAmount.z);
        }
        StartCoroutine(MoveToHandSmoothly(initialPosition, initialRotation, EulerRotationAmount));
    }

    public IEnumerator MoveToHandSmoothly(Vector3 initialPosition, Quaternion initialRotation, Quaternion rotationOverride)
    {
        float elapsedTime = 0f;

        while (elapsedTime < pickupDuration && isBeingCarriedL || isBeingCarriedR)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / pickupDuration;

            transform.SetLocalPositionAndRotation(Vector3.Lerp(initialPosition, Vector3.zero, t), Quaternion.Lerp(initialRotation, rotationOverride, t));

            yield return null;
        }
    }

    public void PlayerDeath()
    {
        if (isBeingCarriedR || isBeingCarriedL)
        {
            isBeingCarriedR = false;
            isBeingCarriedL = false;
            ObjectInteraction.ReleaseObject(isMagic, rb, magic, transform, isBeingCarriedR ? objectVelocityR : objectVelocityL, thrownMultiplier);

            if (weapon != null)
            {
                StartCoroutine(weapon.ResetSkillTrigger());
            }
        }
    }
}
