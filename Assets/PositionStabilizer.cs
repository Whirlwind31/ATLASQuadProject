using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Class used to adjust rotation of GameObject
/// </summary>
public class PositionStabilizer : MonoBehaviour
{
    private Quaternion rotation;
    private Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotation = transform.rotation;
        position = transform.position;

        if (rotation.x != 0 || rotation.y !=0)
        {
            rotation.x = 0;
            rotation.z = 0;
            transform.rotation = rotation;
        }

        if (position.y != 0)
        {
            position.y = 0;
            // transform.position = position;
        }
    }
}
