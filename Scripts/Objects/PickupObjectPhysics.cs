using Revo.Methods;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PickupObjectsPhysics : MonoBehaviour
{
    private bool isBeingCarriedR = false;
    private bool isBeingCarriedL = false;
    private Transform originalParent;
    private Rigidbody rb;
    private HingeJoint handJoint;
    public bool isWeapon;
    private PickupAttackMonster weapon;
    private GameObject player;
    private GameObject itemHolderR;
    private GameObject itemHolderL;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private readonly int bufferSize = 15;
    private Queue<Vector3> positionsR;
    private Vector3 objectVelocityR;
    private Queue<Vector3> positionsL;
    private Vector3 objectVelocityL;
    public int thrownMultiplier = 10;

    private void Start()
    {
        positionsR = new Queue<Vector3>(bufferSize);
        positionsL = new Queue<Vector3>(bufferSize);
    }

    private void OnEnable()
    {
        handJoint = GetComponent<HingeJoint>();
        rb = GetComponent<Rigidbody>();
        weapon = GetComponent<PickupAttackMonster>();
        itemHolderR = GameObject.Find("ItemHolderR");
        itemHolderL = GameObject.Find("ItemHolderL");
        originalParent = transform.parent;

        startPosition = originalParent.position;
        startRotation = originalParent.rotation;

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("RightHand") && !isBeingCarriedR && !isBeingCarriedL)
        {
            if (SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.RightHand))
            {
                isBeingCarriedR = true;
                player = other.gameObject;
                if (weapon != null) { weapon.isRight = true; weapon.isHeld = true; }
                ObjectInteraction.PickupPhysicsObject(player, itemHolderR.transform, transform, handJoint, rb, isWeapon, weapon);
            }
        }
        if (other.CompareTag("LeftHand") && !isBeingCarriedL && !isBeingCarriedR)
        {
            if (SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.LeftHand))
            {
                isBeingCarriedL = true;
                player = other.gameObject;
                if (weapon != null) { weapon.isRight = false; weapon.isHeld = true; }
                ObjectInteraction.PickupPhysicsObject(player, itemHolderL.transform, transform, handJoint, rb, isWeapon, weapon);
            }
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
            isBeingCarriedR = false;
            if (weapon != null) { weapon.isHeld = false; }
            ObjectInteraction.ReleasePhysicsObject(originalParent, itemHolderR, transform, handJoint, rb, thrownMultiplier, objectVelocityR);
        }
        if (isBeingCarriedL && SteamVR_Input.GetStateUp("GrabGrip", SteamVR_Input_Sources.LeftHand))
        {
            isBeingCarriedL = false;
            if (weapon != null) { weapon.isHeld = false; }
            ObjectInteraction.ReleasePhysicsObject(originalParent, itemHolderL, transform, handJoint, rb, thrownMultiplier, objectVelocityL);
        }

    }

    public void ResetPositionAndRotation()
    {
        rb.isKinematic = true;
        originalParent.SetPositionAndRotation(startPosition, startRotation);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        rb.isKinematic = false;
    }

    public void PlayerDeath()
    {
        if (isBeingCarriedL)
        {
            isBeingCarriedL = false;
            ObjectInteraction.ReleasePhysicsObject(originalParent, itemHolderR, transform, handJoint, rb, thrownMultiplier, objectVelocityR);
        }
        if (isBeingCarriedR)
        {
            isBeingCarriedR = false;
            ObjectInteraction.ReleasePhysicsObject(originalParent, itemHolderL, transform, handJoint, rb, thrownMultiplier, objectVelocityL);
        }

    }
}
