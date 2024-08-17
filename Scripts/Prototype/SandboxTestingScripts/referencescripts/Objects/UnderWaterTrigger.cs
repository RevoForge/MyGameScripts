using UnityEngine;

public class UnderWaterTrigger : MonoBehaviour
{
    private MeshRenderer m_MeshRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UnderWater"))
        {
            m_MeshRenderer = other.GetComponent<MeshRenderer>();
            m_MeshRenderer.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("UnderWater"))
        {
            m_MeshRenderer = other.GetComponent<MeshRenderer>();
            m_MeshRenderer.enabled = false;
        }
    }
}
