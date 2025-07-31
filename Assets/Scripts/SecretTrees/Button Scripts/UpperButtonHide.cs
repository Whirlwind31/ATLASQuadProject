using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperButtonHide : MonoBehaviour
{
    [SerializeField] private GameObject paper;
    private PageNo scr;

    private void Awake()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        scr = paper.GetComponent<PageNo>();
    }

    // Disables B Button Indicators if you cannot "turn back" one page.
    void Update()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = (scr.pageNumber > scr.minPageNo);
    }
}