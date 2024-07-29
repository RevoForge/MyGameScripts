using UnityEngine;

public class mushroomParticleControl : MonoBehaviour
{
    private Transform castSpot;
    public GameObject castPrefab;

    private void Start()
    {
        Transform childTransform = transform.Find("CastSource");
        if (childTransform != null)
        {
            castSpot = childTransform;
        }
    }

    public void CastMagic()
    {
        var newSpell = Instantiate(castPrefab, castSpot.position, castSpot.rotation);
        newSpell.transform.SetParent(transform);
        Destroy(newSpell, 5.0f);
    }

}