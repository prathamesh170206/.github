using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

[RequireComponent(typeof(XRGrabInteractable))]
public class DisableTurningOnGrab : MonoBehaviour
{
    XRGrabInteractable grab;

    SnapTurnProvider snapTurn;
    ContinuousTurnProvider continuousTurn;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        snapTurn = FindObjectOfType<SnapTurnProvider>(true);
        continuousTurn = FindObjectOfType<ContinuousTurnProvider>(true);

        grab.selectEntered.AddListener(_ => SetTurning(false));
        grab.selectExited.AddListener(_ => SetTurning(true));
    }

    void SetTurning(bool enableTurning)
    {
        if (snapTurn != null)
            snapTurn.enabled = enableTurning;

        if (continuousTurn != null)
            continuousTurn.enabled = enableTurning;
    }
}
    