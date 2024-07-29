using UnityEngine;
using Valve.VR;

public class SteamVRStarter : MonoBehaviour
{
    public GameObject errorObject;
    public GameObject buttons;

    private void Start()
    {
        if (SteamVR.instance == null)
        {
            InitializeSteamVR();
        }
    }

    private void InitializeSteamVR()
    {
        SteamVR.Initialize();
        StartCoroutine(CheckSteamVRInitialization());
    }

    private System.Collections.IEnumerator CheckSteamVRInitialization()
    {
        yield return new WaitForSeconds(10f);
        if (SteamVR.instance == null)
        {
            errorObject.SetActive(true);
            buttons.SetActive(false);
            yield return new WaitForSeconds(5f);
            Application.Quit();
        }
    }
}
