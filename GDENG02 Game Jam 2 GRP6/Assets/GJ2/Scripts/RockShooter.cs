using UnityEngine;

public class RockShooter : MonoBehaviour
{
    public GameObject rockPrefab;
    public Transform firePoint; // where the rock spawns from
    public float fireForce = 10f;

    public bool useInterval = true;
    public float fireInterval = 3f;

    public bool useTrigger = false;
    public string targetTag = "Player";

    private float fireTimer;

    void Start()
    {
        fireTimer = fireInterval;
    }

    void Update()
    {
        if (useInterval)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = fireInterval;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (useTrigger && other.CompareTag(targetTag))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject rock = Instantiate(rockPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = rock.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(firePoint.forward * fireForce, ForceMode.Impulse);
        }
    }
}