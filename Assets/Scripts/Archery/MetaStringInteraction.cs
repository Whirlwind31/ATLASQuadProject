using System;
using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(Grabbable))]
public class MetaStringInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform stringStartPoint;

    [SerializeField]
    private Transform stringEndPoint;

    [Header("Interaction")]
    [SerializeField]
    private Grabbable grabbable;

    [Tooltip("Drag your LeftHandAnchor's 'Hand Grab Interactor' GameObject here")]
    [SerializeField]
    private GrabInteractor leftHandGrabInteractor;

    [Tooltip("Drag your RightHandAnchor's 'Hand Grab Interactor' GameObject here")]
    [SerializeField]
    private GrabInteractor rightHandGrabInteractor;

    private int _leftHandId = -1;
    private int _rightHandId = -1;

    private Transform interactorTransform = null;
    private bool isGrabbed = false;
    private int _interactorId = -1;

    private float previousPullAmount = 0f;
    private const float kEpsilon = 1e-4f;

    // This property holds the result of our calculation.
    public float PullAmount { get; private set; } = 0.0f;

    // These properties are helpful for the bow script to get the string points.
    public Vector3 StringStartPoint => stringStartPoint != null ? stringStartPoint.position : transform.position;
    public Vector3 StringEndPoint => stringEndPoint != null ? stringEndPoint.position : transform.position;

    // Optionally expose the transforms if needed by other systems.
    public Transform StringStartTransform => stringStartPoint;
    public Transform StringEndTransform => stringEndPoint;

    public event Action<float> PullAmountChanged;

    public event Action<float> OnStringReleased;

    void Awake()
    {
        // Ensure the Grabbable reference is set.
        if (grabbable == null)
        {
            grabbable = GetComponent<Grabbable>();
        }

        if (leftHandGrabInteractor != null)
        {
            _leftHandId = leftHandGrabInteractor.Identifier;
        }
        if (rightHandGrabInteractor != null)
        {
            _rightHandId = rightHandGrabInteractor.Identifier;
        }

    }

    // Editor-time quick validation of fields
    private void OnValidate()
    {
        if (grabbable == null)
        {
            grabbable = GetComponent<Grabbable>();
        }

#if UNITY_EDITOR
        if (stringStartPoint == null || stringEndPoint == null)
        {
            UnityEngine.Debug.LogWarning($"{nameof(MetaStringInteraction)} on '{name}' has null string endpoints. Assign Transforms for proper behavior.", this);
        }
#endif
    }

    // Subscribe to the Grabbable events when the object is enabled.
    private void OnEnable()
    {
        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised += HandlePointerEvent;
        }
        else
        {
            Debug.LogWarning($"{nameof(MetaStringInteraction)}: Grabbable is null in OnEnable on '{name}'.", this);
        }
    }

    // Unsubscribe when the object is disabled to prevent errors.
    private void OnDisable()
    {
        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised -= HandlePointerEvent;
        }

        // Ensure state is reset when component is disabled.
        isGrabbed = false;
        interactorTransform = null;
        SetPullAmount(0f);
    }

    private void HandlePointerEvent(PointerEvent pointerEvent)
    {
        int eventId = pointerEvent.Identifier;

        switch (pointerEvent.Type)
        {
            case PointerEventType.Select:
                // Check if the event ID matches one of our known hands
                if (eventId == _leftHandId)
                {
                    isGrabbed = true;
                    _interactorId = eventId;
                    // Store the TRANSFORM from the interactor
                    interactorTransform = leftHandGrabInteractor.transform;
                }
                else if (eventId == _rightHandId)
                {
                    isGrabbed = true;
                    _interactorId = eventId;
                    // Store the TRANSFORM from the interactor
                    interactorTransform = rightHandGrabInteractor.transform;
                }
                break;

            case PointerEventType.Unselect:
                // Check if the pointer unselecting is the same one we are tracking
                if (eventId == _interactorId)
                {
                    isGrabbed = false;
                    interactorTransform = null;
                    _interactorId = -1;

                    // --- ADD THIS LINE ---
                    // Fire the release event with the last known pull amount before resetting
                    OnStringReleased?.Invoke(PullAmount);

                    SetPullAmount(0f);
                }
                break;

            default:
                break;
        }
    }

    void Update()
    {
        // If grabbed and we have a live hand transform, sample its current position each frame.
        if (isGrabbed && interactorTransform != null)
        {
            Vector3 pullPosition = interactorTransform.position;

            if (stringStartPoint != null && stringEndPoint != null)
            {
                float newPull = CalculatePull(pullPosition);
                SetPullAmount(newPull);
            }
        }
        else if (PullAmount != 0f)
        {
            SetPullAmount(0f);
        }
    }

    // Editor visualization for the string and current pull.
    private void OnDrawGizmosSelected()
    {
        if (stringStartPoint == null || stringEndPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(stringStartPoint.position, stringEndPoint.position);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(stringStartPoint.position, 0.01f);
        Gizmos.DrawSphere(stringEndPoint.position, 0.01f);

        if (isGrabbed && interactorTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(interactorTransform.position, 0.015f);

            Vector3 projected = stringStartPoint.position + Vector3.Project(interactorTransform.position - stringStartPoint.position, (stringEndPoint.position - stringStartPoint.position).normalized);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(projected, 0.01f);
        }
    }

    // Centralized setter to raise change events only when value meaningfully changes.
    private void SetPullAmount(float value)
    {
        if (Mathf.Abs(previousPullAmount - value) > kEpsilon)
        {
            previousPullAmount = value;
            PullAmount = value;
            PullAmountChanged?.Invoke(PullAmount);
        }
        else
        {
            PullAmount = value;
        }
    }
    private float CalculatePull(Vector3 pullPosition)
    {
        if (stringStartPoint == null || stringEndPoint == null) return 0f;

        Vector3 pullDirection = pullPosition - stringStartPoint.position;
        Vector3 targetDirection = stringEndPoint.position - stringStartPoint.position;
        float maxLength = targetDirection.magnitude;

        if (maxLength <= Mathf.Epsilon) return 0f;

        targetDirection.Normalize();

        float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;
        return Mathf.Clamp01(pullValue);
    }
}
