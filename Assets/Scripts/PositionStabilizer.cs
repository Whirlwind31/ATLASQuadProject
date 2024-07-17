using UnityEngine;
using System.Collections;

public class PositionStabilizer : MonoBehaviour
{
    [Tooltip("The time it takes for the object position to reset.")]
    [SerializeField]
    public float delayTime = 2.0f;


    private bool isGrounded = false;

    /// <summary>
    /// Helper method which checks if the object collided with the desired layer.
    /// </summary>
    /// <param name="collision">The collision between GameObject and desired layer.</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("OnCollisionEnter triggered!");
            isGrounded = true;
            StartCoroutine(ResetAfterDelay(delayTime));
        }
    }

    /// <summary>
    /// Waits for a period of time, and adjusts the orientaion of the GameObject
    /// once it is grounded.
    /// </summary>
    /// <param name="delay">Delay time in seconds. May be adjusted through the Unity Inspector.</param>
    IEnumerator ResetAfterDelay(float delay)
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

    /// <summary>
    /// Helper method which checks if the object left the desired layer.
    /// </summary>
    /// <param name="collision">The collision between GameObject and desired layer.</param>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("OnCollisionExit triggered!");
            isGrounded = false;
        }
    }
}

