using UnityEngine;
using System.Collections;

public class GroundedBehavior : MonoBehaviour
{
    public float adjustDelay = 2.5f;
    public float spawnDelay = 2.0f;
    public GameObject[] objectsToSpawn;
    public float spawnHeight = 16.0f;

    private bool isGrounded = false;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Enter with: " + collision.gameObject.name);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("OnCollisionEnter triggered with ground!");
            if (!isGrounded)
            {
                isGrounded = true;
                Debug.Log("Grounded: " + isGrounded);
                StartCoroutine(ResetAndSpawnAfterDelay(adjustDelay, spawnDelay));
            }
        }
    }

    IEnumerator ResetAndSpawnAfterDelay(float adjustTime, float spawnTime)
    {
        Debug.Log("Coroutine started: ResetAndSpawnAfterDelay");
        yield return new WaitForSeconds(adjustTime);

        Debug.Log("Adjust Time Wait Over");
        if (isGrounded)
        {
            Debug.Log("Position and rotation adjusted!");
            Vector3 newRotation = transform.rotation.eulerAngles;
            newRotation.x = 0;
            newRotation.z = 0;
            transform.rotation = Quaternion.Euler(newRotation);

            Vector3 newPosition = transform.position;
            newPosition.y = 0;
            transform.position = newPosition;

            yield return new WaitForSeconds(spawnTime);

            Debug.Log("Spawn Time Wait Over");
            foreach (GameObject obj in objectsToSpawn)
            {
                Debug.Log("Spawning object: " + obj.name);
                Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + spawnHeight, transform.position.z);
                Instantiate(obj, spawnPosition, Quaternion.identity);
            }

            isGrounded = false;
        }
        else
        {
            Debug.Log("Object is not grounded, skipping position and rotation adjustment.");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Collision Exit with: " + collision.gameObject.name);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("OnCollisionExit triggered with ground!");
            isGrounded = false;
            Debug.Log("Grounded: " + isGrounded);
        }
    }
}


