using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapBlock : MonoBehaviour
{
    [Tooltip("How close two snap points need to be to snap together.")]
    public float snapDistance = 0.005f;

    [Tooltip("Optional: add extra damping to stabilize after landing.")]
    public float linearDamping = 2.5f;
    public float angularDamping = 2.5f;

    private bool hasSnapped = false;
    private FixedJoint joint;

    // Per-group piece count, keyed by that group's middle Rigidbody
    private static Dictionary<Rigidbody, int> groupPieceCounts = new Dictionary<Rigidbody, int>();


    private void OnTriggerStay(Collider other)
    {
        if (hasSnapped) return;
        if (!other.CompareTag("SnapPoint")) return;

        Transform targetSnap = other.transform;
        float distance = Vector3.Distance(transform.position, targetSnap.position);
        if (distance > snapDistance) return;

        Rigidbody myRb = GetComponent<Rigidbody>();
        if (myRb == null) return;

        // ---------------------------
        // FIND ROOT (MIDDLE) IN SAME LETTER GROUP
        // ---------------------------

        string letterKey = ExtractLetterKey(gameObject.name);
        Rigidbody rootRb = FindRootMiddle(letterKey, myRb, targetSnap);

        if (rootRb == null)
        {
            Debug.LogWarning($"[SnapBlock] {gameObject.name} could not find a valid 'middle' for {letterKey}");
            return;
        }

        // ---------------------------
        // PREVENT SELF-JOINTS
        // ---------------------------
        if (rootRb == myRb) return;

        // ---------------------------
        // DISABLE XR GRABBING
        // ---------------------------
        DisableGrabInteractions();
        hasSnapped = true;

        // ---------------------------
        // ALIGN PERFECTLY BEFORE ATTACHMENT
        // ---------------------------
        transform.position = targetSnap.position;
        transform.rotation = targetSnap.rotation;

        if (transform.parent != null && transform.parent.name.ToLower().Contains("hand"))
        {
            transform.SetParent(null, true);
            Debug.Log($"[SnapBlock] {gameObject.name} detached from XR hand before snapping.");
        }

        // Parent under the middle for correct assembly
        if (rootRb != null)
            transform.SetParent(rootRb.transform, true);

        // Disable "Collider Extension" helpers
        foreach (Collider c in GetComponentsInChildren<Collider>(true))
        {
            string n = c.name.ToLower();
            c.enabled = !n.Contains("collider extension");
        }

        // ---------------------------
        // CREATE FIXED JOINT
        // ---------------------------
        TryTemporarilyUnlockXR(myRb);

        // Delay actual joint creation to avoid XR physics conflict
        StartCoroutine(DelayedJointAttach(myRb, rootRb));


        // ---------------------------
        // STABILIZE PHYSICS
        // ---------------------------
        myRb.isKinematic = false;
        myRb.useGravity = true;
        myRb.detectCollisions = true;
        myRb.mass = 1f;
        myRb.drag = linearDamping;
        myRb.angularDrag = angularDamping;
        myRb.solverIterations = 20;
        myRb.solverVelocityIterations = 20;
        myRb.maxDepenetrationVelocity = 1.5f;
        myRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // ---------------------------
        // TRACK ATTACHED PARTS PER MIDDLE
        // ---------------------------
        int count = 0;
        if (groupPieceCounts.TryGetValue(rootRb, out count))
            count++;
        else
            count = 1;

        groupPieceCounts[rootRb] = count;
        Debug.Log($"[SnapBlock] {rootRb.name} now has {count} parts attached.");

        // When this middle has all three pieces (top/middle/bottom)
        if (count >= 3)
        {
            groupPieceCounts[rootRb] = 0;
            MergeAssembly(rootRb);
        }

        Physics.SyncTransforms();
    }

    // Temporarily unlock XR control when attaching a grabbed piece
    private void TryTemporarilyUnlockXR(Rigidbody rb)
    {
        var locker = rb.GetComponent("RigidbodyKinematicLocker") as Behaviour;
        if (locker != null && locker.enabled)
        {
            locker.enabled = false;
            StartCoroutine(RestoreXRLocker(locker, 0.2f)); // re-enable after 0.2 s
            Debug.Log($"[SnapBlock] Temporarily disabled XR locker on {rb.name} for safe snapping.");
        }
    }

    private IEnumerator RestoreXRLocker(Behaviour locker, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (locker != null)
        {
            locker.enabled = true;
            Debug.Log("[SnapBlock] XR locker restored.");
        }
    }

    private IEnumerator DelayedJointAttach(Rigidbody myRb, Rigidbody rootRb)
    {
        // Wait one fixed frame so XR release & physics sync
        yield return new WaitForFixedUpdate();

        // Recheck validity before creating the joint
        if (myRb == null || rootRb == null) yield break;
        if (myRb == rootRb) yield break;

        // Create the joint cleanly after release
        FixedJoint joint = myRb.GetComponent<FixedJoint>();
        if (joint == null)
            joint = myRb.gameObject.AddComponent<FixedJoint>();

        joint.connectedBody = rootRb;
        joint.breakForce = Mathf.Infinity;
        joint.breakTorque = Mathf.Infinity;

        Debug.Log($"[SnapBlock] Delayed joint successfully attached between {myRb.name} and {rootRb.name}.");
    }

    // ---------------------------
    // FIND THE CORRECT "MIDDLE" BLOCK
    // ---------------------------
    private Rigidbody FindRootMiddle(string letterKey, Rigidbody self, Transform targetSnap)
    {
        Rigidbody rootRb = null;

        // 1. Search nearby colliders for a "middle" of same letter
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.25f);
        foreach (Collider hit in hits)
        {
            if (hit.attachedRigidbody == null) continue;
            if (hit.attachedRigidbody == self) continue;

            string n = hit.attachedRigidbody.name.ToLower();
            if (n.Contains("middle") && n.Contains(letterKey))
            {
                rootRb = hit.attachedRigidbody;
                break;
            }
        }

        // 2. Hierarchy fallback
        if (rootRb == null)
        {
            Transform t = transform.parent;
            while (t != null && rootRb == null)
            {
                if (t.name.ToLower().Contains("middle") && t.name.ToLower().Contains(letterKey))
                    rootRb = t.GetComponent<Rigidbody>();
                t = t.parent;
            }
        }

        // 3. Global fallback search
        if (rootRb == null)
        {
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj == gameObject) continue;
                string n = obj.name.ToLower();
                if (n.Contains("middle") && n.Contains(letterKey))
                {
                    Rigidbody cand = obj.GetComponent<Rigidbody>();
                    if (cand != null && cand != self)
                    {
                        rootRb = cand;
                        break;
                    }
                }
            }
        }

        return rootRb;
    }


    // ---------------------------
    // EXTRACT LETTER IDENTIFIER
    // ---------------------------
    private string ExtractLetterKey(string name)
    {
        // e.g. "logo-i-middle[2]" ? "i"
        string lower = name.ToLower();
        int firstDash = lower.IndexOf("-");
        int secondDash = lower.IndexOf("-", firstDash + 1);
        if (firstDash >= 0 && secondDash > firstDash)
            return lower.Substring(firstDash + 1, secondDash - firstDash - 1);
        return "";
    }


    // ---------------------------
    // DISABLE XR GRABBING LOGIC
    // ---------------------------
    private void DisableGrabInteractions()
    {
        var grabScripts = GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var script in grabScripts)
        {
            string n = script.GetType().Name.ToLower();
            if (n.Contains("handgrabinteraction") || n.Contains("grabinteractable") || n.Contains("grabbable"))
            {
                if (script is Behaviour b)
                {
                    b.enabled = false;
                    Debug.Log($"[SnapBlock] Disabled grab interaction on {gameObject.name} ({script.GetType().Name})");
                }
            }
        }
    }


    // ---------------------------
    // MERGE FINALIZED GROUP INTO ONE BODY
    // ---------------------------
    private void MergeAssembly(Rigidbody rootRb)
    {
        Debug.Log("[SnapBlock] Unifying assembled parts...");

        FixedJoint[] joints = rootRb.GetComponentsInChildren<FixedJoint>(true);
        foreach (FixedJoint j in joints)
        {
            if (j != null && j.connectedBody == rootRb)
            {
                Rigidbody childRb = j.GetComponent<Rigidbody>();
                if (childRb != null && childRb != rootRb)
                {
                    StartCoroutine(SafeDestroyRigid(childRb, j));
                }
            }
        }

        rootRb.mass = 3f;
        rootRb.drag = linearDamping;
        rootRb.angularDrag = angularDamping;
        rootRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rootRb.maxDepenetrationVelocity = 1.5f;

        Debug.Log("[SnapBlock] Assembly unified successfully.");
    }

    private IEnumerator SafeDestroyRigid(Rigidbody childRb, FixedJoint joint)
    {
        // Give Unity one full physics frame to stabilize XR and collider refs
        yield return new WaitForFixedUpdate();

        // Verify objects still exist before destroying
        if (joint != null)
            Destroy(joint);

        if (childRb != null)
        {
            childRb.isKinematic = true;
            childRb.useGravity = false;
            yield return null; // one more frame
            Destroy(childRb);
        }
    }



    // ---------------------------
    // COLLISION DAMPENER
    // ---------------------------
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == null && collision.relativeVelocity.magnitude > 5f)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
