using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PointSlot : MonoBehaviour
{
    public FloorZone floorZone;

    private ClothItem currentItem;
    private BoxCollider triggerCollider;

    public ClothItem CurrentItem => currentItem;

    private void Awake()
    {
        triggerCollider = GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
    }

    private void Start()
    {
        RegisterStartingItem();
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentItem != null)
            return;

        ClothItem cloth = other.GetComponent<ClothItem>();
        if (cloth == null)
            cloth = other.GetComponentInParent<ClothItem>();

        if (cloth == null)
            return;

        if (cloth.IsHeld)
            return;

        if (cloth.currentPointSlot != null && cloth.currentPointSlot != this)
            return;

        AssignItem(cloth);
    }

    private void AssignItem(ClothItem cloth)
    {
        currentItem = cloth;
        cloth.SnapToPoint(this);

        if (floorZone != null)
            floorZone.NotifyChanged();
    }

    public void RemoveCurrentItem()
    {
        currentItem = null;

        if (floorZone != null)
            floorZone.NotifyChanged();
    }

    public void RegisterStartingItem()
    {
        Vector3 worldCenter = transform.TransformPoint(triggerCollider.center);
        Vector3 halfExtents = Vector3.Scale(triggerCollider.size, transform.lossyScale) * 0.5f;

        Collider[] hits = Physics.OverlapBox(worldCenter, halfExtents * 0.9f, transform.rotation);

        foreach (Collider hit in hits)
        {
            ClothItem cloth = hit.GetComponent<ClothItem>();
            if (cloth == null)
                cloth = hit.GetComponentInParent<ClothItem>();

            if (cloth == null)
                continue;

            if (cloth.IsHeld)
                continue;

            AssignItem(cloth);
            return;
        }
    }
}