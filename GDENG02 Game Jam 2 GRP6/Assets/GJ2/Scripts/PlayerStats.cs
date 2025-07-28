using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        EventBroadcaster.Instance.AddObserver(EventNames.GJ2_Events.ON_HIT, TakeDamage);
    }

    void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveObserver(EventNames.GJ2_Events.ON_HIT);
    }

    public void TakeDamage()
    {
        int amount = 1;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Notify UI or other systems
        EventBroadcaster.Instance.PostEvent(EventNames.GJ2_Events.ON_HEALTH_CHANGED); 
    }
}
