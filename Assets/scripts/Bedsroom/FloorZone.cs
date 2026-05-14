using UnityEngine;

public class FloorZone : MonoBehaviour
{
    public PointSlot leftPoint;
    public PointSlot rightPoint;

    public ClothItem.ClothColor requiredColorA;
    public ClothItem.ClothColor requiredColorB;

    public StageManager stageManager;

    public bool IsSolved()
    {
        if (leftPoint == null || rightPoint == null)
            return false;

        if (leftPoint.CurrentItem == null || rightPoint.CurrentItem == null)
            return false;

        ClothItem.ClothColor leftColor = leftPoint.CurrentItem.clothColor;
        ClothItem.ClothColor rightColor = rightPoint.CurrentItem.clothColor;

        bool case1 = leftColor == requiredColorA && rightColor == requiredColorB;
        bool case2 = leftColor == requiredColorB && rightColor == requiredColorA;

        return case1 || case2;
    }

    public void NotifyChanged()
    {
        if (stageManager != null)
            stageManager.CheckStageClear();
    }
}