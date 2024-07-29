using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SmoothTurnWhmd : MonoBehaviour
{
    public SteamVR_Action_Vector2 joystickAction;
    public SteamVR_Action_Boolean hmdAction;

    private Player player;
    private float rotationSpeed = 10f; // Default rotation speed

    private void Awake()
    {
        player = Player.instance;
    }

    private void Update()
    {
        if (joystickAction != null && hmdAction != null && player != null)
        {
            Vector2 joystickValue = joystickAction.GetAxis(SteamVR_Input_Sources.Any);
            bool hmdUsed = hmdAction.GetState(SteamVR_Input_Sources.Any);

            if (joystickValue.magnitude > 0)
            {
                float turnAmount = joystickValue.x;
                RotatePlayer(turnAmount);
            }
            else if (hmdUsed)
            {
                float hmdTurnAmount = CalculateHMDTurnAmount();
                RotatePlayer(hmdTurnAmount);
            }
        }
    }

    private float CalculateHMDTurnAmount()
    {
        Quaternion hmdRotation;
        if (UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out hmdRotation))
        {
            Vector3 hmdEuler = hmdRotation.eulerAngles;
            float hmdTurnAmount = hmdEuler.y;
            return hmdTurnAmount;
        }
        else
        {
            return 0f;
        }
    }

    private void RotatePlayer(float turnAmount)
    {
        float scaledRotationSpeed = rotationSpeed * Time.deltaTime;
        player.transform.Rotate(Vector3.up, turnAmount * scaledRotationSpeed);
    }

    public void SetRotationSpeed(float value)
    {
        rotationSpeed = value;
    }
}
