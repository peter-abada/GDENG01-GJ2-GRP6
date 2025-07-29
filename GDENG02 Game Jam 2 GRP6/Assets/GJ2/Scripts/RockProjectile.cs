using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    public float lifetime = 10f;
    public string playerTag = "Player";
    //[SerializeField] private GameObject player;

    private void Start()
    {
        //Destroy(gameObject, lifetime); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Rock collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag(playerTag))
        {
            //player.GetComponent<PlayerStats>().TakeDamage();
            EventBroadcaster.Instance.PostEvent(EventNames.GJ2_Events.ON_HIT);
        }

        
        Destroy(gameObject);
    }
}