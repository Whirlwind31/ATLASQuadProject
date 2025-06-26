using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Collider c = gameObject.GetComponent<MeshCollider>();
        c.enabled = true;
    }

    // Update is called once per frame
    void Update() { }

    public void disableCollision()
    {
        Collider c = gameObject.GetComponent<MeshCollider>();
        c.enabled = false;
    }
}
