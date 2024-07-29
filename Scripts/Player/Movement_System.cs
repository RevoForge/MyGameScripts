using UnityEngine;
using Valve.VR;

public class Movement_System : MonoBehaviour
{
    // Must have SteamVR Input System Set with the listed Action Names for this to work or add your own names in the getstatedown strings
    public GameObject smooth;
    public GameObject snap;
    public bool useButtonTurnChange;
    [Header("Unnecessary if useButtonTurnChange is true")]
    public GameObject ui_canvas;
    public GameObject mainMenu;
    public GameObject statsMenu;
    public GameObject inventoryMenu;
    public GameObject laserPointer;

    void Update()
    {
        if (!useButtonTurnChange)
        {
            if (SteamVR_Input.GetStateDown("Movement_UI", SteamVR_Input_Sources.Any))
            {
                if (ui_canvas.activeSelf)
                {
                    ui_canvas.SetActive(false);
                    laserPointer.SetActive(false);
                }
                else
                {
                    ui_canvas.SetActive(true);
                    inventoryMenu.SetActive(true);
                    statsMenu.SetActive(false);
                    mainMenu.SetActive(false);
                    laserPointer.SetActive(true);
                }
            }
            if (SteamVR_Input.GetStateDown("UILaser", SteamVR_Input_Sources.Any))
            {
                if (laserPointer.activeSelf)
                {
                    laserPointer.SetActive(false);
                }
                else
                {
                    laserPointer.SetActive(true);
                }
            }
        }
        else
        {
            if (SteamVR_Input.GetStateDown("ChangeTurn", SteamVR_Input_Sources.Any))
            {
                if (snap.activeSelf) { snap.SetActive(false); smooth.SetActive(true); }
                else { snap.SetActive(true); smooth.SetActive(false); }
            }
        }

    }

    public void Setturnchange(bool value)
    {
        smooth.SetActive(value);
        snap.SetActive(!value);
    }
}
