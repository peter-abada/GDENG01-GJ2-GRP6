using UnityEngine;
using UnityEngine.UI;

public class HealthManagerMG5 : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    private PlayerStats player;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Start()
    {
        if (playerObject != null)
        {
            player = playerObject.GetComponent<PlayerStats>();
        }

        if (hearts.Length == 0 || fullHeart == null || emptyHeart == null || player == null)
        {
            Debug.LogError("HealthManagerMG5: Missing references.");
            return;
        }

        EventBroadcaster.Instance.AddObserver(EventNames.GJ2_Events.ON_HEALTH_CHANGED, UpdateUI);
        UpdateUI(); // initialize at start
    }

    void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveObserver(EventNames.GJ2_Events.ON_HEALTH_CHANGED);
    }

    void UpdateUI()
    {
        int currentHealth = Mathf.Clamp(player.currentHealth, 0, hearts.Length);

        if (player == null)
        {
            Debug.LogError("HealthManagerMG5: Player reference is null in UpdateUI!");
            return;
        }

        
        Debug.Log("Updating UI with health: " + currentHealth);
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
        }
    }
}
