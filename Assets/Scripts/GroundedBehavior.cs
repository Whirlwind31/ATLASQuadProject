using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedBehavior : MonoBehaviour
{
    public float adjustDelay = 2.5f;

    private bool isGrounded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!isGrounded)
            {
                isGrounded = true;
                StartCoroutine(ResetPositionAndRotation(adjustDelay));
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }

    IEnumerator ResetPositionAndRotation(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isGrounded)
        {
            Vector3 newRotation = transform.rotation.eulerAngles;
            newRotation.x = 0;
            newRotation.z = 0;
            transform.rotation = Quaternion.Euler(newRotation);

            Vector3 newPosition = transform.position;
            newPosition.y = 0;
            transform.position = newPosition;

            isGrounded = false;
        }
    }
}
