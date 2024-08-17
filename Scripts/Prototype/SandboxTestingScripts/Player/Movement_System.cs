using UnityEngine;
using Valve.VR;

public class Movement_System : MonoBehaviour
{
    // Must have SteamVR Input System Set with the listed Action Names for this to work or add your own names in the getstatedown strings
    public GameObject smooth;
    public GameObject snap;
    public bool useButtonTurnChange;

    [Header("Skill Prototype UI References")]
    public bool useSkillPrototypeUI;
    public GameObject skillPrototypeUI;
    public RRSkillSystem skillPrototypeSystem;
    [Header("Unnecessary if useButtonTurnChange is true")]
    public GameObject ui_canvas;
    public GameObject mainMenu;
    public GameObject statsMenu;
    public GameObject inventoryMenu;
    public GameObject laserPointer;

    private SettingsLoader settingsLoader;

    private void Start()
    {
        settingsLoader = FindObjectOfType<SettingsLoader>().GetComponent<SettingsLoader>();
    }

    void Update()
    {
        if (useSkillPrototypeUI)
        {
            if (SteamVR_Input.GetStateDown("Movement_UI", SteamVR_Input_Sources.Any))
            {
                if (skillPrototypeUI.activeSelf)
                {
                    skillPrototypeUI.SetActive(false);
                    laserPointer.SetActive(false);
                    skillPrototypeSystem.usingSkillUI = false;
                }
                else
                {
                    skillPrototypeUI.SetActive(true);
                    laserPointer.SetActive(true);
                    skillPrototypeSystem.usingSkillUI = true;
                }
            }
        }
        else
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
                if (SteamVR_Input.GetStateDown("changeTurn", SteamVR_Input_Sources.Any))
                {
                    if (snap.activeSelf)
                    {
                        snap.SetActive(false);
                        smooth.SetActive(true);
                        settingsLoader.currentSettings.MovementType = true;
                    }
                    else
                    {
                        snap.SetActive(true);
                        smooth.SetActive(false);
                        settingsLoader.currentSettings.MovementType = false;
                    }
                }
            }
        }


    }

    public void Setturnchange(bool value)
    {
        settingsLoader.currentSettings.MovementType = value;
        smooth.SetActive(value);
        snap.SetActive(!value);
    }
}
