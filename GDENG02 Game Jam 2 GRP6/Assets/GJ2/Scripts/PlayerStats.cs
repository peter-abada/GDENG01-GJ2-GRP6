using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    [Header("SFX")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioSource sfxAudioSource;

    //[SerializeField] private HealthManagerMG5 healthManager;

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

        //if (healthManager != null)
        //{
        //    healthManager.UpdateUI();
        //}
        EventBroadcaster.Instance.PostEvent(EventNames.GJ2_Events.ON_HEALTH_CHANGED);
        if (sfxAudioSource != null && hitSound != null)
        {
            sfxAudioSource.PlayOneShot(hitSound);
        }

        if (currentHealth <= 0)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }
}
