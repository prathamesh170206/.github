using UnityEngine;

public class MirrorMindActivator : MonoBehaviour
{
    void Start()
    {
        // Only activate if this object is in Layer3
        if (gameObject.layer != LayerMask.NameToLayer("Layer3"))
            return;

        foreach (var m in FindObjectsByType<MirroredMovement>(FindObjectsSortMode.None))
            m.mirrorEnabled = true;

        foreach (var r in FindObjectsByType<MirroredRotation>(FindObjectsSortMode.None))
            r.mirrorEnabled = true;
    }
}
