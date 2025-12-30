using UnityEngine;

public class MirroredMovement : MonoBehaviour
{
    [Header("References")]
    public Transform xrRig;          // XR Origin or Camera Offset
    public Transform mirrorPlane;    // Center of mirror (normal defines mirror direction)

    [Header("Control")]
    public bool mirrorEnabled = false;

    void LateUpdate()
    {
        if (!mirrorEnabled || xrRig == null || mirrorPlane == null)
            return;

        // Position of player relative to mirror plane
        Vector3 relativePos = xrRig.position - mirrorPlane.position;

        // Reflect across mirror plane normal
        Vector3 mirroredPos = Vector3.Reflect(relativePos, mirrorPlane.forward);

        // Apply mirrored position
        transform.position = mirrorPlane.position + mirroredPos;
    }
}
