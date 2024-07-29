using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SmoothWalkWhmd : MonoBehaviour
{
    public SteamVR_Action_Vector2 moveAction;

    private Player player;
    public float movementSpeed = 1.5f; // Default movement speed
    private Vector3 previousHmdPosition;
    public float positionChangeThreshold = 0.01f; // Threshold to determine if the HMD has moved

    private void Awake()
    {
        player = Player.instance;
    }

    private void Update()
    {
        if (player != null)
        {
            // Get the player's current position and rotation
            Vector3 currentHmdPosition = player.hmdTransform.position;
            Quaternion hmdRotation = player.hmdTransform.rotation;
            if (previousHmdPosition == null)
            {
                previousHmdPosition = player.hmdTransform.position;
            }
            // Calculate the position change of the HMD
            Vector3 positionChange = currentHmdPosition - previousHmdPosition;

            // Set the Y component of the positionChange vector to zero
            positionChange.y = 0f;

            // Reset the movement flag
            bool isMoving = false;

            // Check if joystick movement is being used (moveAction is not null)
            if (moveAction != null)
            {
                // Get joystick input
                Vector2 moveValue = moveAction.GetAxis(SteamVR_Input_Sources.Any);
                Vector3 moveDirection = new Vector3(moveValue.x, 0f, moveValue.y);

                // Rotate the move direction based on the HMD rotation
                moveDirection = RotateVectorByQuaternion(moveDirection, hmdRotation);

                // Check if joystick movement is significant
                if (moveDirection.magnitude >= 0.1f)
                {
                    // Update the player's position based on joystick input
                    player.transform.position += moveDirection * movementSpeed * Time.deltaTime;
                    isMoving = true; // Set isMoving to true since joystick movement is happening
                    return; // Skip the HMD position check since joystick movement is being used
                }

            }

            // Check if HMD position has changed enough
            if (positionChange.magnitude > positionChangeThreshold)
            {
                // Update the player's position based on the HMD movement relative to the previous position
                player.transform.position += positionChange * (10 * Time.deltaTime);
                isMoving = true; // Set isMoving to true since HMD movement is happening
            }

            // If there is no input or movement, stop the player's movement
            if (!isMoving)
            {
                player.transform.position += Vector3.zero;
            }

            // Update the previous HMD position
            previousHmdPosition = currentHmdPosition;
        }
    }

    public void SetMovementSpeed(float value)
    {
        movementSpeed = value;
    }

    private Vector3 RotateVectorByQuaternion(Vector3 vector, Quaternion quaternion)
    {
        Vector3 rotatedVector = quaternion * vector;
        return rotatedVector;
    }
}
