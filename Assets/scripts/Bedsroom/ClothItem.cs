using UnityEngine;

public class ClothItem : MonoBehaviour
{
    public enum ClothColor
    {
        None,
        White,
        Navy,
        Red
    }

    public enum ClothType
    {
        None,
        Shirt
    }

    [Header("Auto Detected Info")]
    public ClothColor clothColor = ClothColor.None;
    public ClothType clothType = ClothType.None;

    private void OnValidate()
    {
        string objName = gameObject.name.ToLower();

        // 색 판별
        if (objName.Contains("white"))
            clothColor = ClothColor.White;
        else if (objName.Contains("navy"))
            clothColor = ClothColor.Navy;
        else if (objName.Contains("red"))
            clothColor = ClothColor.Red;
        else
            clothColor = ClothColor.None;

        // 종류 판별
        if (objName.Contains("shirt"))
            clothType = ClothType.Shirt;
        else
            clothType = ClothType.None;
    }
}