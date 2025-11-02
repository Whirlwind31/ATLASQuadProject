using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
[RequireComponent(typeof(XRGrabInteractable))]
public class Forceperspective : MonoBehaviour
{
    [Header("Scale Settings")]
    public float minScale = 0.2f;
    public float maxScale = 5f;
    public float maxRayDistance = 100f;

    private XRGrabInteractable grabInteractable;
    private Transform cameraTransform;
    private Vector3 initialScale;
    private float initialDistance;
    private bool isGrabbed = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        cameraTransform = Camera.main?.transform;

        grabInteractable.firstSelectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (cameraTransform == null) return;

        isGrabbed = true;
        initialScale = transform.localScale;
        initialDistance = Vector3.Distance(cameraTransform.position, transform.position);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            if (hit.transform == transform) return; 
            Vector3 targetPos = cameraTransform.position + cameraTransform.forward * hit.distance;
            transform.position = targetPos;
        }
    }

    void Update()
    {
        if (!isGrabbed || cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            if (hit.transform == transform) return;

            float distance = hit.distance;
            float scaleFactor = distance / initialDistance;
            scaleFactor = Mathf.Clamp(scaleFactor, minScale, maxScale);

            transform.localScale = initialScale * scaleFactor;
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.firstSelectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}
