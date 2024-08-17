using Revo.Methods;
using System.Collections;
using UnityEngine;
using Valve.VR;
using static Revo.Methods.ObjectInteraction;
public class PlayerController : MonoBehaviour
{
    private GameObject vrCamera;
    private Transform vrRig;
    private Transform vrCameraT;
    public SteamVR_Action_Vector2 walk;
    public SteamVR_Action_Boolean jump;
    public SteamVR_Action_Boolean crouch;
    public float speed = 1;
    public float jumpHeight = 3f;
    public float jumpTime = 0.5f;
    private float jumpSpeed;
    private float jumpTimeCounter;
    private Vector3 moveDirection;
    private Vector3 fallVelocity = Vector3.zero;
    private Vector3 startPosition;
    private Vector3 originalPosition;
    private Quaternion startRotation;
    private bool isFalling = false;
    private bool isSeated;
    private bool crouched;
    private Vector3 targetPosition;
    private readonly float transitionDuration = 0.5f;
    private float speedModifier = 1;
    private bool isResettingSpeedModifier = false;
    private CharacterController characterController;

    void OnEnable()
    {
        characterController = GetComponent<CharacterController>();
        vrCamera = PlayerDetection.FindChildWithTag(transform, "MainCamera");
        vrCameraT = vrCamera.transform;
        vrRig = vrCameraT.parent.parent;
        originalPosition = vrRig.position;
        startPosition = vrCameraT.position;
        startRotation = transform.rotation;
        jumpSpeed = 2f * jumpHeight / jumpTime;

        PositionController();
        OpenVR.Chaperone.ResetZeroPose(ETrackingUniverseOrigin.TrackingUniverseStanding);
    }
    private void OnParticleCollision(GameObject other)
    {
        if (ParticleModifiers.TryGetValue(other.name, out SpeedModifier modifier) && !isResettingSpeedModifier)
        {
            speedModifier = modifier.moveSpeedModifier;
            StartCoroutine(ResetSpeedModifier(modifier.timeModifier));
            isResettingSpeedModifier = true;
        }
    }
    private IEnumerator ResetSpeedModifier(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        speedModifier = 1;
        isResettingSpeedModifier = false;
    }
    void Update()
    {
        HandleMovement();
        PositionController();
        PlayerPhysics();
        if (jump.GetState(SteamVR_Input_Sources.Any) && characterController.isGrounded)
        {
            jumpTimeCounter = jumpTime; // Reset the jump time counter
            StartCoroutine(HandleJump());
        }
        if (isSeated)
        {
            SeatedMode();
        }
    }
    private void PlayerPhysics()
    {

        fallVelocity.y += Physics.gravity.y * Time.deltaTime;

        // Move the character with the proper fall speed
        characterController.Move((moveDirection + fallVelocity) * Time.deltaTime);

        if (characterController.isGrounded)
        {
            jumpTimeCounter = 0f;
            isFalling = false;
            fallVelocity = Vector3.zero;
        }

        if (isFalling)
        {
            // Increase the fall speed over time
            float fallMultiplier = 2f; // Adjust the multiplier as needed
            fallVelocity.y += Physics.gravity.y * fallMultiplier * Time.deltaTime;
        }
    }
    private void SeatedMode()
    {
        if (crouch.GetState(SteamVR_Input_Sources.Any))
        {
            if (Mathf.Approximately(vrRig.localPosition.y, 0.7f) && !crouched)
            {
                targetPosition = Vector3.zero;
                crouched = true;
                StartCoroutine(MoveVRRigSmoothly());
            }
        }
        else if (crouched)
        {
            if (Mathf.Approximately(vrRig.localPosition.y, 0f))
            {
                targetPosition = Vector3.up * 0.7f;
                crouched = false;
                StartCoroutine(MoveVRRigSmoothly());
            }
        }
    }

    private IEnumerator MoveVRRigSmoothly()
    {
        Vector3 initialPosition = vrRig.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            vrRig.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        vrRig.localPosition = targetPosition;
    }

    void HandleMovement()
    {
        // Calculate the camera movement adjustment
        Vector3 cameraMovement = vrCameraT.localPosition - startPosition;

        // Calculate the input-based movement
        Vector3 inputMovement = Vector3.zero;

        if (walk.axis.sqrMagnitude > 0.1f * 0.1f)
        {
            Vector3 direction = vrCameraT.transform.TransformDirection(new Vector3(walk.axis.x, 0, walk.axis.y));
            inputMovement = speed * Vector3.ProjectOnPlane(direction, Vector3.up);
        }

        // Combine camera and input movements
        moveDirection = (cameraMovement + inputMovement) * speedModifier;

        // Update the starting position for camera adjustments
        startPosition = vrCameraT.localPosition;
    }
    public void HandleSeated(bool seated)
    {
        isSeated = seated;
        if (isSeated)
        {
            vrRig.localPosition = new Vector3(0, 0.7f, 0);
        }
        else
        {
            vrRig.localPosition = Vector3.zero;
        }
    }

    private IEnumerator HandleJump()
    {
        while (jumpTimeCounter > 0f)
        {
            float jumpDistance = jumpSpeed * Time.deltaTime;
            if (jumpDistance > 0f)
            {
                characterController.Move(Vector3.up * jumpDistance);
                jumpTimeCounter -= Time.deltaTime;
            }
            yield return null;
        }
        isFalling = true;

        yield break;
    }

    public void ResetPositionAndRotation()
    {
        transform.SetPositionAndRotation(originalPosition, startRotation);
    }

    private void PositionController()
    {
        Vector3 playerHeight = isSeated || !crouched ? new Vector3(0, vrCamera.transform.localPosition.y + 0.7f, 0) : vrCamera.transform.localPosition;
        float headHeight = Mathf.Clamp(playerHeight.y, 1, 2);
        characterController.height = headHeight;

        //Cut in half, add skin
        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;

        //Lets move the capsule in local space as well
        newCenter.x = vrCamera.transform.localPosition.x;
        newCenter.z = vrCamera.transform.localPosition.z;

        //Apply
        characterController.center = newCenter;
    }
}
