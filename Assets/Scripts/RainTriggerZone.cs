using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RainTriggerZone : MonoBehaviour
{
    public GameObject[] objectsToRain;
    public float rainHeight = 40.0f;
    public int rainCount = 180;
    public float rainAreaSize = 120.0f;
    public LayerMask triggerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if ((triggerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("raaaaaaain");
            StartCoroutine(RainObjects());
        }
    }

    IEnumerator RainObjects()
    {
        for (int i = 0; i < rainCount; i++)
        {
            foreach (GameObject obj in objectsToRain)
            {
                Vector3 spawnPosition = new Vector3(
                    transform.position.x + Random.Range(-rainAreaSize / 2, rainAreaSize / 2),
                    transform.position.y + rainHeight,
                    transform.position.z + Random.Range(-rainAreaSize / 2, rainAreaSize / 2)
                );
                Instantiate(obj, spawnPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.1f); // Adjust the interval between spawns as needed
        }
    }
}
