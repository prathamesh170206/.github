using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

[RequireComponent(typeof(XRGrabInteractable))]
public class VRDoorAutoClose : MonoBehaviour
{
    [Header("Hinge Setup")]
    public Transform pivot;

    [Header("Door Limits")]
    public float minAngle = 0f;
    public float maxAngle = 90f;

    [Header("Tuning")]
    public float smoothSpeed = 10f;
    public float angleDeadZone = 0.3f;

    [Header("Assist Open")]
    public float assistThreshold = 45f;
    public float assistOpenSpeed = 120f;

    [Header("Auto Close")]
    public float closeDelay = 0.3f;
    public float closeSpeed = 90f;

    private XRGrabInteractable grab;
    private Transform grabbingHand;

    private bool isGrabbing = false;
    private bool assistOpening = false;
    private bool isClosing = false;

    private float doorAngleAtGrab;
    private float handAngleAtGrab;
    private float currentAngle;

    private Quaternion closedRotation;
    private Coroutine closeRoutine;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        closedRotation = transform.rotation;
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    // -------------------- GRAB LOGIC (UNCHANGED) --------------------

    void OnGrab(SelectEnterEventArgs args)
    {
        if (pivot == null || isGrabbing)
            return;

        isGrabbing = true;
        assistOpening = false;
        isClosing = false;

        if (closeRoutine != null)
            StopCoroutine(closeRoutine);

        grabbingHand = args.interactorObject.transform;

        doorAngleAtGrab = GetDoorAngle();

        Vector3 handDir = grabbingHand.position - pivot.position;
        handDir.y = 0f;

        handAngleAtGrab = Mathf.Atan2(handDir.x, handDir.z) * Mathf.Rad2Deg;
        currentAngle = doorAngleAtGrab;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbing = false;
        grabbingHand = null;
        doorAngleAtGrab = currentAngle;

        // Assist open if already open enough
        if (currentAngle >= assistThreshold)
            assistOpening = true;
    }

    void Update()
    {
        // -------- ORIGINAL OPENING --------
        if (isGrabbing && grabbingHand != null && pivot != null)
        {
            Vector3 handDir = grabbingHand.position - pivot.position;
            handDir.y = 0f;

            if (handDir.sqrMagnitude < 0.0001f)
                return;

            float handAngleNow = Mathf.Atan2(handDir.x, handDir.z) * Mathf.Rad2Deg;
            float delta = Mathf.DeltaAngle(handAngleAtGrab, handAngleNow);

            float targetAngle = Mathf.Clamp(
                doorAngleAtGrab + delta,
                minAngle,
                maxAngle
            );

            if (Mathf.Abs(targetAngle - currentAngle) > angleDeadZone)
            {
                currentAngle = Mathf.Lerp(
                    currentAngle,
                    targetAngle,
                    Time.deltaTime * smoothSpeed
                );

                ApplyDoorRotation(currentAngle);
            }

            return;
        }

        // -------- ASSIST OPEN --------
        if (assistOpening)
        {
            currentAngle = Mathf.MoveTowards(
                currentAngle,
                maxAngle,
                assistOpenSpeed * Time.deltaTime
            );

            ApplyDoorRotation(currentAngle);

            if (Mathf.Abs(currentAngle - maxAngle) < 0.1f)
            {
                currentAngle = maxAngle;
                ApplyDoorRotation(currentAngle);
                assistOpening = false;
            }

            return;
        }

        // -------- SIMPLE AUTO CLOSE --------
        if (isClosing)
        {
            currentAngle = Mathf.MoveTowards(
                currentAngle,
                minAngle,
                closeSpeed * Time.deltaTime
            );

            ApplyDoorRotation(currentAngle);

            if (Mathf.Abs(currentAngle - minAngle) < 0.1f)
            {
                currentAngle = minAngle;
                ApplyDoorRotation(currentAngle);
                isClosing = false;
            }
        }
    }

    // -------------------- AUTO CLOSE API --------------------

    public void StartAutoClose()
    {
        if (isGrabbing || isClosing)
            return;

        if (closeRoutine != null)
            StopCoroutine(closeRoutine);

        closeRoutine = StartCoroutine(AutoCloseAfterDelay());
    }

    IEnumerator AutoCloseAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);
        isClosing = true;
    }

    // -------------------- HELPERS (UNCHANGED) --------------------

    float GetDoorAngle()
    {
        Vector3 dir = transform.position - pivot.position;
        dir.y = 0f;

        return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    }

    void ApplyDoorRotation(float angle)
    {
        transform.rotation = closedRotation;

        transform.RotateAround(
            pivot.position,
            Vector3.up,
            angle
        );
    }
}
