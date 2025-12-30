using UnityEngine;

public class TunnelHum : MonoBehaviour
{
    public float intensity = 0.04f;
    public float interval = 0.1f;

    void Start()
    {
        InvokeRepeating(nameof(Hum), 0f, interval);
    }

    void Hum()
    {
        if (HapticsManager.Instance != null)
            HapticsManager.Instance.Vibrate(intensity, 0.05f);
    }

    void OnDestroy()
    {
        CancelInvoke();
    }
}
