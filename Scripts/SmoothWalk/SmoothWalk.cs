using UnityEngine;
using Valve.VR;

public class SmoothWalk : MonoBehaviour
{
    public GameObject player;
    public Transform cameraTransform;
    public float movementSpeed = 1.5f; // Default movement speed

    private void Update()
    {
        if (player != null && cameraTransform != null)
        {
            Vector2 moveValue = SteamVR_Input.GetVector2Action("SmoothWalk").GetAxis(SteamVR_Input_Sources.Any);
            Vector3 moveDirection = new(moveValue.x, 0f, moveValue.y);
            moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * moveDirection;

            float scaledMovementSpeed = movementSpeed * Time.deltaTime;
            player.transform.position += moveDirection * scaledMovementSpeed;
        }
    }

    public void SetMovementSpeed(float value)
    {
        movementSpeed = value;
    }
}
