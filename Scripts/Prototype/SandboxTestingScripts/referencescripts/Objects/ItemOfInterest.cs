using UnityEngine;
using UnityEngine.UI;
using static Revo.Methods.LootSystem;

public class ItemOfInterest : MonoBehaviour
{
    private PlayerStats playerStats;
    private AudioSource itemAudio;
    private Button button;
    public LootEntry[] lootEntries;
    private Transform lootTransform;

    void OnEnable()
    {
        itemAudio = GetComponent<AudioSource>();
        button = GetComponent<Button>();
        lootTransform = transform.Find("LootDrop");
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameObject.GetComponent<SphereCollider>().enabled && other.CompareTag("Player"))
        {
            playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null && !playerStats.hasSeenItemOI)
            {
                playerStats.hasSeenItemOI = true;
                itemAudio.Play();
                gameObject.GetComponent<SphereCollider>().enabled = false;
            }
            if (playerStats != null && playerStats.hasSeenItemOI)
                gameObject.GetComponent<SphereCollider>().enabled = false;
        }
        if (!gameObject.GetComponent<SphereCollider>().enabled && other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            button.onClick.Invoke();
        }
    }

    public void AddStatPoint()
    {
        playerStats.IncreaseAvaliblePoints();
        ButtonUsed();
    }
    public void GiveLoot()
    {
        GameObject selectedObject = DropLoot(lootEntries);

        // Instantiate the selected object
        if (selectedObject != null)
        {
            Instantiate(selectedObject, lootTransform.position, lootTransform.rotation);
        }
        ButtonUsed();
    }

    private void ButtonUsed()
    {
        button.gameObject.SetActive(false);
    }
}
