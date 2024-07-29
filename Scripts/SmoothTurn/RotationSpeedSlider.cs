using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class RotationSpeedSlider : MonoBehaviour
{
    public Slider slider;
    public SmoothTurn smoothTurn;
    public SteamVR_Action_Boolean turnUIAction;
    public GameObject sliderContainer;

    private bool sliderActive = false;

    private void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);

    }

    private void Update()
    {
        if (turnUIAction != null && sliderContainer != null)
        {
            if (turnUIAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                sliderActive = !sliderActive;
                sliderContainer.SetActive(sliderActive);

            }
        }
    }

    private void OnSliderValueChanged(float value)
    {
        smoothTurn.SetRotationSpeed(value);
    }
}

