using UnityEngine;
using Oculus.Interaction; // For Grabbable

[RequireComponent(typeof(Grabbable), typeof(Rigidbody), typeof(Collider))]
public class Arrow : MonoBehaviour
{
    [Tooltip("How much to multiply the 0-1 pull value by for force")]
    [SerializeField]
    private float firePowerMultiplier = 20f;

    private Rigidbody rb;
    private Grabbable grabbable;
    private Collider col;

    public bool IsHeldByHand { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<Grabbable>();
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        // Start with physics off
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnEnable()
    {
        grabbable.WhenPointerEventRaised += HandlePointerEvent;
    }

    private void OnDisable()
    {
        grabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent pointerEvent)
    {
        if (pointerEvent.Type == PointerEventType.Select)
        {
            IsHeldByHand = true;

            // When we pick up the arrow, re-enable physics
            // so it doesn't just float in the air if we drop it
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        else if (pointerEvent.Type == PointerEventType.Unselect)
        {
            IsHeldByHand = false;
        }
    }

    // Called by BowController when arrow is nocked
    public void Nock(Transform nockPoint)
    {
        // We've been nocked, so we are no longer "held"
        IsHeldByHand = false;

        // Disable the grabbable so the player can't accidentally grab it
        grabbable.enabled = false;

        // Turn off physics
        rb.isKinematic = true;
        rb.useGravity = false;

        // Parent to the nock point and snap into position
        transform.SetParent(nockPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Called by BowController when string is released
    public void Fire(Vector3 fireDirection, float pullValue)
    {
        // Unparent from the bow
        transform.SetParent(null);

        // Turn physics back on
        rb.isKinematic = false;
        rb.useGravity = true;

        // Apply force
        float fireForce = pullValue * firePowerMultiplier;
        rb.AddForce(fireDirection * fireForce, ForceMode.Impulse);

        // Make it point in the direction it's flying
        transform.rotation = Quaternion.LookRotation(rb.velocity);

        // Disable the collider for a split second to avoid hitting the bow
        StartCoroutine(BrieflyDisableCollider());
    }

    // Stick in surfaces
    private void OnCollisionEnter(Collision collision)
    {
        // Only stick if we are moving fast and haven't been nocked
        if (rb.velocity.magnitude > 0.5f && !isKinematic)
        {
            // Stop moving and stick to the object
            rb.isKinematic = true;
            rb.useGravity = false;
            transform.SetParent(collision.transform);
        }
    }

    private System.Collections.IEnumerator BrieflyDisableCollider()
    {
        col.enabled = false;
        yield return new WaitForSeconds(0.1f);
        col.enabled = true;
    }

    // Helper property
    private bool isKinematic => rb.isKinematic;
}