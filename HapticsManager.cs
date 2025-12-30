using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager Instance;

    InputDevice leftHand;
    InputDevice rightHand;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDevices();
    }

    void InitializeDevices()
    {
        var leftDevices = new List<InputDevice>();
        var rightDevices = new List<InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightDevices);

        if (leftDevices.Count > 0)
            leftHand = leftDevices[0];

        if (rightDevices.Count > 0)
            rightHand = rightDevices[0];
    }

    // Call this if controllers reconnect
    void Update()
    {
        if (!leftHand.isValid || !rightHand.isValid)
            InitializeDevices();
    }

    // -------- BASIC HAPTICS --------
    public void Vibrate(float intensity, float duration)
    {
        VibrateLeft(intensity, duration);
        VibrateRight(intensity, duration);
    }

    public void VibrateLeft(float intensity, float duration)
    {
        if (leftHand.isValid)
            leftHand.SendHapticImpulse(0, intensity, duration);
    }

    public void VibrateRight(float intensity, float duration)
    {
        if (rightHand.isValid)
            rightHand.SendHapticImpulse(0, intensity, duration);
    }

    public void StopAll()
    {
        Vibrate(0f, 0f);
    }
}
// To use this in any script just give the following command
//For example  HapticsManager.Instance.Vibrate(0.05f, 0.1f);
