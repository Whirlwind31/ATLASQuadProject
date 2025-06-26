using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperText : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogue;

    // Start is called before the first frame update
    void Start() 
    {
        dialogue.text =
        @"Hello!
        What brings you here?
        I assume you're not used to walking through trees?";
    }

    // Update is called once per frame
    void Update()
    {}

    /* DEPRECATED Update() method. Replaced with HideWithPaper script.
     * 
    // Hide the text until the paper is rotated rightside-up.

    //GameObject parent = transform.parent.gameObject;

    //float x = parent.transform.eulerAngles.x;
    //float z = parent.transform.eulerAngles.z;
    ////if (z >= 155 && z <= 210 && x >= 0 && x <= 60)
    //if (z <= 25 || z >= 335)
    //{
    //    dialogue.enabled = true;
    //    dialogue.GetComponent<MeshRenderer>().enabled = true;
    //} 
    //else
    //{
    //    dialogue.enabled = false;
    //    dialogue.GetComponent<MeshRenderer>().enabled = false;
    //}
    *
    */
}
