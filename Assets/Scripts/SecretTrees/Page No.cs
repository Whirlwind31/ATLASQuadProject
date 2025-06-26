using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageNo : MonoBehaviour
{
    public int pageNumber = 1;

    // Start is called before the first frame update
    void Start()
    {
        pageNumber = 1;
    }

    //void Update()
    //{
    //    // Detect "A" button press on the right controller
    //    if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
    //    {
    //        Debug.Log("Right controller A button pressed!");
    //    }

    //    // Detect trigger press on the left controller
    //    if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) > 0.5f)
    //    {
    //        Debug.Log("Left controller trigger pulled halfway!");
    //    }
    //}

    void OnButtonClicked()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log("Clicked button: " + buttonName);
    }
}
