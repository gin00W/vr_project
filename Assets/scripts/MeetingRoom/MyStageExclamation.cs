using UnityEngine;

public class MyStageExclamation : MonoBehaviour
{
    [Header("My Stage References")]
    public GameObject myGameRoot;        // WhiteboardCanvas
    public GameObject myExclamationRoot; // 느낌표 UI 부모 (OCDExclamationCanvas 등)

    public void StartMyGame()
    {
        // 내 게임 켜기
        if (myGameRoot != null)
            myGameRoot.SetActive(true);

        // 느낌표 숨기기
        if (myExclamationRoot != null)
            myExclamationRoot.SetActive(false);
    }
}