using UnityEngine;
using Valve.VR;

public class SmoothTurn : MonoBehaviour
{
    public GameObject cameraHolder;
    public GameObject vrCamera;
    private float rotationSpeed = 100f; // Default rotation speed

    private void Update()
    {
        Vector2 turnValue = SteamVR_Input.GetVector2Action("SmoothTurn").GetAxis(SteamVR_Input_Sources.Any);
        float turnAmount = turnValue.x;
        float scaledRotationSpeed = rotationSpeed * Time.deltaTime;
        Vector3 pivot = vrCamera.transform.position;
        // Rotate around the pivot point
        cameraHolder.transform.RotateAround(pivot, Vector3.up, turnAmount * scaledRotationSpeed);
    }
    public void SetRotationSpeed(float value)
    {
        rotationSpeed = value;
    }
}
