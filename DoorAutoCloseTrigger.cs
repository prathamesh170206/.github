using UnityEngine;

public class DoorAutoCloseTrigger : MonoBehaviour
{
    [Header("References")]
    public VRDoorAutoClose door;
    public Transform doorPivot;     // same pivot used by the door
    public Transform xrCamera;      // Main Camera (XR)

    [Header("Tolerance Settings")]
    public float forwardDistance = 0.35f;   // how far past the door
    public float sideTolerance = 0.75f;     // sideways forgiveness

    [Header("Direction")]
    public bool invertForward = false;       // toggle if needed

    private bool triggered = false;

    void Update()
    {
        if (triggered || door == null || doorPivot == null || xrCamera == null)
            return;

        // Vector from door hinge to camera
        Vector3 toCamera = xrCamera.position - doorPivot.position;
        toCamera.y = 0f;

        // Door forward defines "outside"
        Vector3 doorForward = invertForward ? -doorPivot.forward : doorPivot.forward;
        doorForward.y = 0f;
        doorForward.Normalize();

        // How far the player is past the door plane
        float forwardDot = Vector3.Dot(toCamera, doorForward);

        // How far sideways the player is (error margin)
        float sidewaysDist = Vector3.Cross(doorForward, toCamera).magnitude;

        // ✅ TOLERANT trigger condition
        if (forwardDot > forwardDistance &&
            sidewaysDist < sideTolerance)
        {
            door.StartAutoClose();
            triggered = true;
        }
    }
}
