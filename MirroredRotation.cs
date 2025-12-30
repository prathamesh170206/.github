using UnityEngine;

public class MirroredRotation : MonoBehaviour
{
    public Transform xrRig;
    public bool mirrorEnabled = false;

    void LateUpdate()
    {
        if (!mirrorEnabled || xrRig == null)
            return;

        Vector3 euler = xrRig.eulerAngles;
        transform.rotation = Quaternion.Euler(
            euler.x,
            -euler.y,
            euler.z
        );
    }
}
