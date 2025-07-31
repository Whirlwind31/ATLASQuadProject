using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private Vector3 respawnPoint;

    private void Awake()
    {
        respawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -15f)
        {
            // Resets object to 0-rotation. In the case of the paper, you clearly already found that as well.
            transform.SetPositionAndRotation(respawnPoint, Quaternion.identity);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
            
    }
}
