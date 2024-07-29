using UnityEngine;

public class ComponentVisibility : MonoBehaviour
{
    private Light lightComponent;

    private void Start()
    {
        lightComponent = GetComponent<Light>();
    }

    private void OnBecameVisible()
    {


        if (lightComponent != null)
        {
            lightComponent.enabled = true;

        }
    }

    private void OnBecameInvisible()
    {


        if (lightComponent != null)
        {
            lightComponent.enabled = false;

        }
    }
}
