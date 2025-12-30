using UnityEngine;

public class GameClock : MonoBehaviour
{
    public float gameTime;        // seconds since start
    public float timeScale = 1f;  // 0 = paused, 1 = normal, 2 = fast

    void Update()
    {
        gameTime += Time.deltaTime * timeScale;
    }

    // These replace DateTime.Now
    public float Hours =>
        gameTime / 3600f % 12f;

    public float Minutes =>
        gameTime / 60f % 60f;

    public float Seconds =>
        gameTime % 60f;

    // Controls (Layer 2 mechanics)
    public void Pause() => timeScale = 0f;
    public void Resume() => timeScale = 1f;
    public void SpeedUp() => timeScale = 2f;
    public void SlowDown() => timeScale = 0.5f;
    public void ResetClock() => gameTime = 0f;
}
