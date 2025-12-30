using System;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Clock/Clock Time Controller (Real Time)")]
public class ClockTimeController : MonoBehaviour
{
    [Header("Hand Transforms (assign in Inspector)")]
    //Here I have assigned the below variables with type transform instead of GameObject as this helps me to skip a step.
    //Tranform is used when I need to rotate something,move or scale smthg,change heirarchy of smthg.
    // U rarely use GameObject unless u want to enable/disable it.
    public ClockTimeController clock;

    public Transform hourHand;
    public Transform minuteHand;
    public Transform secondHand;
    public GameClock gameClock;




    [Header("Rotation / Behavior")]
    public bool useWorldSpaceHands = false;    // true -> apply world rotation (ignores parent transform)
    // false -> rotate in local space such that it is affected by parent transform.
    public Vector3 rotationAxis = Vector3.right; // axis around which hands rotate (default Z)(In this case X)
    public float rotationOffsetDegrees = 0f;   // tweak if 12 o'clock is not aligned
    [Tooltip("0 = snap immediately. >0 = smooth interpolation (larger = faster smoothing).")]
    public float smoothSpeed = 0f;

    public float GetMinuteHandAngle()
    {
        return minuteHand.localEulerAngles.x; // or y/z depending on axis
    }


    void Reset()
    {
        // sensible defaults when adding the component
        rotationAxis = Vector3.right;
        rotationOffsetDegrees = 0f;
        smoothSpeed = 0f;
    }

    void Start()
    {
        // Auto-find children if not assigned (common names)
        if (hourHand == null) hourHand = transform.Find("Clock_Analog_A_Hour");
        if (minuteHand == null) minuteHand = transform.Find("Clock_Analog_A_Minute");
        if (secondHand == null) secondHand = transform.Find("Clock_Analog_A_Second");
        //The above 3 statements are only neseccary if the three variables are not assigned in the inspector.
        //Here since I have already assigned so the above statements are not needed.

        // case-insensitive fallback
        if (hourHand == null || minuteHand == null || secondHand == null)
        {
            foreach (Transform t in transform)
            {
                string n = t.name.ToLowerInvariant();
                if (hourHand == null && n.Contains("hour")) hourHand = t;
                if (minuteHand == null && n.Contains("min")) minuteHand = t;
                if (secondHand == null && n.Contains("sec")) secondHand = t;
            }
        // The above statements are if both those things dont work. This might happen if while manually assigning we use the wrong case.   
        // This step makes them case insensitive. 
        }
    }

    void Update()
    {
       // DateTime now = DateTime.Now; // uses system timezone (IST on your machine)
        //I have commented the above part as that is not needed for gametime calculation.
        // precise hours/minutes/seconds including fractional parts for smooth sweep
        // These calculations are needed only if the given clock is an analog clock.
        //float hour = (now.Hour % 12) + (now.Minute / 60f) + (now.Second / 3600f) + (now.Millisecond / 3600000f);
       // float minute = now.Minute + (now.Second / 60f) + (now.Millisecond / 60000f);
        //float second = now.Second + (now.Millisecond / 1000f);

        //The earlier calculations can be used when u want the clock to be synced with the real time.
        //Use the ones below to sync it with game time.


        float hour = gameClock.Hours;
        float minute = gameClock.Minutes;
        float second = gameClock.Seconds;


        //This step helps it align with the correct time
        float hourAngle = hour * 30f;    // 360/12
        float minuteAngle = minute * 6f; // 360/60
        float secondAngle = second * 6f; // 360/60

        // final angle (apply offset so 12 o'clock aligns correctly)
        float finalHour = rotationOffsetDegrees + hourAngle;
        float finalMinute = rotationOffsetDegrees + minuteAngle;
        float finalSecond = rotationOffsetDegrees + secondAngle;

        float angle = clock.GetMinuteHandAngle();
        float diff = Mathf.Abs(Mathf.DeltaAngle(angle, 90f));

        if (diff <= 8f)
        {
            Debug.Log("✔ Correct Alignment");
            // SUCCESS
            gameClock.Pause();

            // 2. Unlock something
           // door.SetActive(false);   // or animator trigger

            // 3. Prevent repeat checks
            //puzzleSolved = true;
        }   
        else
        {
            // FAILURE
            Debug.Log("✖ Wrong Alignment");

            // 1. Resume time so player retries
            gameClock.Resume();

            // 2. Optional: small penalty
            gameClock.SpeedUp();

            // 3. Feedback
            //errorSound.Play();

        }



        // compute quaternions around the chosen axis
        if (useWorldSpaceHands)
        {
            // world axis must be normalized
            Vector3 worldAxis = rotationAxis.normalized;
            Quaternion hourQuat = Quaternion.AngleAxis(finalHour, worldAxis);
            Quaternion minuteQuat = Quaternion.AngleAxis(finalMinute, worldAxis);
            Quaternion secondQuat = Quaternion.AngleAxis(finalSecond, worldAxis);

            ApplyRotationWorld(hourHand, hourQuat);
            ApplyRotationWorld(minuteHand, minuteQuat);
            ApplyRotationWorld(secondHand, secondQuat);
        }
        else
        {
            // local axis: convert axis to local space of the hand's parent (if needed)
            Quaternion hourQuat = Quaternion.AngleAxis(finalHour, rotationAxis);
            Quaternion minuteQuat = Quaternion.AngleAxis(finalMinute, rotationAxis);
            Quaternion secondQuat = Quaternion.AngleAxis(finalSecond, rotationAxis);

            ApplyRotationLocal(hourHand, hourQuat);
            ApplyRotationLocal(minuteHand, minuteQuat);
            ApplyRotationLocal(secondHand, secondQuat);
        }
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            gameClock.Pause();
        }

        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            gameClock.Resume();
        }

    }

    void ApplyRotationLocal(Transform hand, Quaternion target)
    {
        if (hand == null) return;
        if (smoothSpeed > 0f)
            hand.localRotation = Quaternion.Slerp(hand.localRotation, target, Time.deltaTime * smoothSpeed);
        else
            hand.localRotation = target;
    }

    void ApplyRotationWorld(Transform hand, Quaternion targetWorld)
    {
        if (hand == null) return;
        if (smoothSpeed > 0f)
            hand.rotation = Quaternion.Slerp(hand.rotation, targetWorld, Time.deltaTime * smoothSpeed);
        else
            hand.rotation = targetWorld;
    }

    // Optional helpers you can call from UI
    public void SetRotationOffset(float degrees) => rotationOffsetDegrees = degrees;
    public void SetSmoothSpeed(float s) => smoothSpeed = Mathf.Max(0f, s);
    public void UseWorldSpace(bool world) => useWorldSpaceHands = world;
}
