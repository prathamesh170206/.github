using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public Light flickerLight;
    public float minIntensity = 1f;
    public float maxIntensity = 4f;
    public float flickerSpeed = 0.15f;

    void Update()
    {
        if (flickerLight == null) return;

        flickerLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f));
    }
}
