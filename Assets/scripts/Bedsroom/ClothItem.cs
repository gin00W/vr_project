using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class ClothItem : MonoBehaviour
{
    public enum ClothColor
    {
        None,
        White,
        Navy,
        Red
    }

    [Header("Auto Detected Info")]
    public ClothColor clothColor = ClothColor.None;

    [HideInInspector] public PointSlot currentPointSlot;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    public bool IsHeld
    {
        get
        {
            return grabInteractable != null && grabInteractable.isSelected;
        }
    }

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnValidate()
    {
        string objName = gameObject.name.ToLower();

        if (objName.Contains("white"))
            clothColor = ClothColor.White;
        else if (objName.Contains("navy"))
            clothColor = ClothColor.Navy;
        else if (objName.Contains("red"))
            clothColor = ClothColor.Red;
        else
            clothColor = ClothColor.None;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (currentPointSlot != null)
        {
            currentPointSlot.RemoveCurrentItem();
            currentPointSlot = null;
        }

        transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void SnapToPoint(PointSlot pointSlot)
    {
        currentPointSlot = pointSlot;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        transform.SetParent(pointSlot.transform);
        transform.position = pointSlot.transform.position;
        transform.rotation = pointSlot.transform.rotation;
    }
}