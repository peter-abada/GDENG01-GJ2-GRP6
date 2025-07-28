using UnityEngine;

public class FlickerScript : MonoBehaviour
{
    private Light fireLight;
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private float flickerSpeed = 1f;

    private void Start()
    {
        fireLight = GetComponent<Light>();
    }

    private void Update()
    {
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f));
    }
}
