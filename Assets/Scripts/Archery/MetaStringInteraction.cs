using UnityEngine;
using Oculus.Interaction;
using System;

[RequireComponent(typeof(LineRenderer), typeof(Grabbable))]
public class MetaStringInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The stationary point on the Top Limb")]
    [SerializeField] private Transform topLimbPos;

    [Tooltip("The stationary point on the Bottom Limb")]
    [SerializeField] private Transform bottomLimbPos;

    [Tooltip("OPTIONAL: Drag an object here that points the Z-Axis in the correct fire direction. If empty, it uses the Parent.")]
    [SerializeField] private Transform directionReference;

    [Header("Settings")]
    [SerializeField] private float maxPullDistance = 0.5f;

    // --- EVENTS ---
    public event Action<float> PullAmountChanged;
    public event Action<float> OnStringReleased;

    private LineRenderer lineRenderer;
    private Grabbable grabbable;
    private float currentPullValue = 0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        grabbable = GetComponent<Grabbable>();

        lineRenderer.positionCount = 3;
        lineRenderer.useWorldSpace = true;
    }

    private void OnEnable()
    {
        if (grabbable != null) grabbable.WhenPointerEventRaised += HandlePointerEvent;
    }

    private void OnDisable()
    {
        if (grabbable != null) grabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Unselect)
        {
            OnStringReleased?.Invoke(currentPullValue);
        }
    }

    private void LateUpdate()
    {
        if (topLimbPos == null || bottomLimbPos == null) return;

        // 1. Determine which object defines "Forward"
        // If you assigned a Direction Reference, use it. Otherwise, use the Parent.
        Transform referenceFrame = directionReference != null ? directionReference : transform.parent;

        if (referenceFrame == null) return;

        // --- 2. Calculate the "Rail" Center ---
        Vector3 worldRestPos = (topLimbPos.position + bottomLimbPos.position) / 2f;

        // --- 3. Calculate the Constrained Position ---
        // Convert Hand position to the Reference Frame's local space
        Vector3 localHandPos = referenceFrame.InverseTransformPoint(transform.position);
        Vector3 localRestPos = referenceFrame.InverseTransformPoint(worldRestPos);

        // MAGIC LINE: Keep Hand's Z (Depth), Force X and Y to match Center
        Vector3 constrainedLocalPos = new Vector3(localRestPos.x, localRestPos.y, localHandPos.z);

        // Clamp Z: Stop string from going forward through the bow
        // (Assumes Z+ is backward. If Z+ is forward, flip the sign of this check)
        if (constrainedLocalPos.z > localRestPos.z)
        {
            constrainedLocalPos.z = localRestPos.z;
        }

        // Convert back to World Space
        Vector3 constrainedWorldPos = referenceFrame.TransformPoint(constrainedLocalPos);

        // --- 4. FORCE THE OBJECT TO THE RAIL ---
        transform.position = constrainedWorldPos;

        // --- 5. Update Visuals ---
        lineRenderer.SetPosition(0, topLimbPos.position);
        lineRenderer.SetPosition(1, constrainedWorldPos);
        lineRenderer.SetPosition(2, bottomLimbPos.position);

        // --- 6. Calculate Pull Amount ---
        float distance = Vector3.Distance(worldRestPos, constrainedWorldPos);
        currentPullValue = Mathf.Clamp01(distance / maxPullDistance);

        PullAmountChanged?.Invoke(currentPullValue);
    }
}