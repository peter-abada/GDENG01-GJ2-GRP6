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

    [SerializeField] private AudioClip rockShotClip;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 30f;

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

        if (rockShotClip != null)
        {
            AudioSource sfx = rock.AddComponent<AudioSource>();
            sfx.clip = rockShotClip;
            sfx.spatialBlend = 1f; // fully 3D
            sfx.minDistance = minDistance;
            sfx.maxDistance = maxDistance;
            sfx.rolloffMode = AudioRolloffMode.Linear; // or Logarithmic
            sfx.Play();

            Destroy(sfx, rockShotClip.length + 0.1f); // cleanup after playing
        }
    }
}