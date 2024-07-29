
using System.Collections;
using UnityEngine;
using Valve.VR;

public class RevoSnap : MonoBehaviour
{
    public Transform cameraHolder;
    public Transform cameraTransform;
    public float snapAngle = 90.0f;
    public bool showTurnAnimation = true;
    public AudioSource snapTurnSource;
    public AudioClip rotateSound;
    public GameObject rotateRightFX;
    public GameObject rotateLeftFX;
    public bool fadeScreen = true;
    public float fadeTime = 0.1f;
    public Color screenFadeColor = Color.black;
    public float distanceFromFace = 1.3f;
    public Vector3 additionalOffset = new(0, -0.3f, 0);
    private bool canRotate = true;
    private bool hasRotated = false;
    public float canTurnEverySeconds = 0.4f;

    private void Start()
    {
        AllOff();
    }

    private void AllOff()
    {
        if (rotateLeftFX != null)
            rotateLeftFX.SetActive(false);

        if (rotateRightFX != null)
            rotateRightFX.SetActive(false);
    }

    private void Update()
    {

        if (canRotate)
        {
            if (Time.time < canTurnEverySeconds)
                return;

            if (SteamVR_Input.GetStateDown("SnapTurnLeft", SteamVR_Input_Sources.Any) && canRotate)
            {
                RotatecameraHolder(-snapAngle);
                hasRotated = true;
            }
            if (SteamVR_Input.GetStateDown("SnapTurnRight", SteamVR_Input_Sources.Any) && canRotate)
            {
                RotatecameraHolder(snapAngle);
                hasRotated = true;
            }
        }
    }

    private Coroutine rotateCoroutine;
    public void RotatecameraHolder(float angle)
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            AllOff();
        }
        rotateCoroutine = StartCoroutine(DoRotatecameraHolder(angle));
    }

    private IEnumerator DoRotatecameraHolder(float angle)
    {
        canRotate = false;
        snapTurnSource.panStereo = angle / 90;
        snapTurnSource.PlayOneShot(rotateSound);

        if (fadeScreen)
        {
            SteamVR_Fade.Start(Color.clear, 0);

            Color tColor = screenFadeColor;
            tColor = tColor.linear * 0.6f;
            SteamVR_Fade.Start(tColor, fadeTime);
        }

        yield return new WaitForSeconds(fadeTime);
        GameObject fx = angle > 0 ? rotateRightFX : rotateLeftFX;

        if (fx != null)
        {
            fx.SetActive(true);
            UpdateOrientation(angle);
        }

        if (fadeScreen)
        {
            SteamVR_Fade.Start(Color.clear, fadeTime);
        }

        float startTime = Time.time;
        float endTime = startTime + canTurnEverySeconds;

        while (Time.time <= endTime)
        {
            yield return null;
            UpdateOrientation(angle);
        };

        if (fx != null)
        {
            fx.SetActive(false);
        }

        canRotate = true;
    }

    private void UpdateOrientation(float angle)
    {
        // Position fx in front of the face
        Vector3 targetPosition = cameraHolder.transform.position + (cameraHolder.transform.forward * distanceFromFace);
        targetPosition.y = cameraTransform.transform.position.y; // Match the Y position of the cameraHolder
        transform.position = targetPosition;

        // Set the rotation of fx to face the cameraHolder
        transform.rotation = Quaternion.LookRotation(cameraHolder.transform.position - transform.position, Vector3.up);
        transform.Translate(additionalOffset, Space.Self);
        transform.rotation = Quaternion.LookRotation(cameraHolder.transform.position - transform.position, Vector3.up);
        if (cameraHolder != null && hasRotated)
        {
            cameraHolder.transform.RotateAround(cameraTransform.position, Vector3.up, angle);
            hasRotated = false;
        }
    }
}
