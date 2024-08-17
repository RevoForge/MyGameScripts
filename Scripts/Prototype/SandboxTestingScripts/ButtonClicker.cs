using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClicker : MonoBehaviour
{
    public Button tmproButton;

    // Method to be called when the inspector button is clicked
    public void ClickTMPButton()
    {
        if (tmproButton.IsActive())
        {
            EventSystem.current.SetSelectedGameObject(tmproButton.gameObject);
            tmproButton.onClick.Invoke();
        }
    }
}
