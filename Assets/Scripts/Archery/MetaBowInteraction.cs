using System; // Needed for Action
using UnityEngine;

// Note: We don't need 'Oculus.Interaction' in this script
// because it only talks to our other scripts.

public class BowController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The MetaStringInteraction script on your string")]
    [SerializeField]
    private MetaStringInteraction stringInteraction;

    [Tooltip("The empty GameObject where the arrow snaps to")]
    [SerializeField]
    private Transform nockPoint;

    [Header("Bow Visuals")]
    [Tooltip("Top part of the bow that will bend")]
    [SerializeField]
    private Transform topLimb;

    [Tooltip("Bottom part of the bow that will bend")]
    [SerializeField]
    private Transform bottomLimb;

    [Tooltip("How much the limbs should bend at full pull")]
    [SerializeField]
    private float maxLimbBend = 30f; // Max rotation in degrees

    [Tooltip("How far back the nock point moves at full pull")]
    [SerializeField]
    private float maxPullDistance = 0.5f; // In meters

    // Private State
    private Arrow currentArrow = null;
    private bool isArrowNocked = false;
    private Vector3 nockRestLocalPosition;

    private void Start()
    {
        if (stringInteraction == null)
        {
            Debug.LogError("BowController: String Interaction is not set!", this);
            return;
        }

        // Store the nock's starting position
        if (nockPoint != null)
        {
            nockRestLocalPosition = nockPoint.localPosition;
        }

        // Subscribe to the string's events
        stringInteraction.PullAmountChanged += UpdateBowTension;
        stringInteraction.OnStringReleased += FireArrow;
    }

    private void OnDestroy()
    {
        // Always unsubscribe from events
        if (stringInteraction != null)
        {
            stringInteraction.PullAmountChanged -= UpdateBowTension;
            stringInteraction.OnStringReleased -= FireArrow;
        }
    }

    // This is called by the NockSocket script (see Step 3)
    public void NockArrow(Arrow arrow)
    {
        if (isArrowNocked) return; // Already have an arrow

        Debug.Log("Arrow Nocked!");
        currentArrow = arrow;
        isArrowNocked = true;

        // Tell the arrow it's nocked (this will disable its physics/grab)
        currentArrow.Nock(nockPoint);
    }

    // Called every frame the string's pull amount changes
    private void UpdateBowTension(float pullAmount)
    {
        // 1. Bend the limbs
        if (topLimb != null)
        {
            // Rotates top limb forward (around X-axis)
            topLimb.localRotation = Quaternion.Euler(pullAmount * maxLimbBend, 0, 0);
        }
        if (bottomLimb != null)
        {
            // Rotates bottom limb forward (around X-axis)
            bottomLimb.localRotation = Quaternion.Euler(pullAmount * -maxLimbBend, 0, 0);
        }

        // 2. Move the nock point (and the attached arrow)
        if (nockPoint != null)
        {
            // Moves nock point back (along Z-axis)
            nockPoint.localPosition = nockRestLocalPosition + Vector3.back * pullAmount * maxPullDistance;
        }
    }

    // Called once when the string is released
    private void FireArrow(float finalPullAmount)
    {
        if (!isArrowNocked || currentArrow == null)
        {
            // No arrow to fire, just reset bow
            UpdateBowTension(0f);
            return;
        }

        Debug.Log("Firing arrow with power: " + finalPullAmount);

        // Tell the arrow to fly
        // We use nockPoint.forward because that's the direction the arrow is pointing
        currentArrow.Fire(nockPoint.forward, finalPullAmount);

        // Reset state
        isArrowNocked = false;
        currentArrow = null;

        // Reset bow visuals
        UpdateBowTension(0f);
    }
}