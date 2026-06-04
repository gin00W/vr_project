using UnityEngine;

public class MyStageExclamation : MonoBehaviour
{
    [Header("My Stage References")]
    public GameObject myGameRoot;        // WhiteboardCanvas
    public GameObject myExclamationRoot; // 느낌표 UI 부모

    public void StartMyGame()
    {
        // 1) 실제 미니게임 로직 시작
        if (MeetingRoomManager.Instance != null)
        {
            MeetingRoomManager.Instance.StartMiniGame();
        }
        else
        {
            Debug.LogWarning("MeetingRoomManager.Instance 가 없습니다.");
        }

        // 2) (안전용) 내 게임 루트(UI) 켜기
        if (myGameRoot != null)
            myGameRoot.SetActive(true);

        // 3) 느낌표 숨기기
        if (myExclamationRoot != null)
            myExclamationRoot.SetActive(false);
    }
}