using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWithPaper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Hide object until paper is rotated rightside up.
        // Exclusively used with children of the Secret Paper family.

        GameObject paper = GameObject.FindWithTag("EasterEggPaper");
        float z = paper.transform.eulerAngles.z;

        if (z <= 25 || z >= 335)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
