using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OCDExclamationClick : MonoBehaviour
{
    [Header("--- 게임 UI ---")]
    public GameObject whiteboardCanvas;

    [Header("--- 숨길 자기 자신 ---")]
    public GameObject myExclamationCanvas;

    // 정적 상태
    public static int clearedCount = 0;
    public static int totalElements = 4;
    public static bool gameStarted = false;

    [Header("--- 다음 씬 ---")]
    public string nextSceneName = "room03";

    void Start()
    {
        Button btn = GetComponentInChildren<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClick);
            Debug.Log(gameObject.name + ": Button 찾음!");
        }
    }

    public void OnClick()
    {
        Debug.Log(gameObject.name + " ❗ 클릭됨!");

        // 게임 아직 시작 안 했으면 시작
        if (!gameStarted)
        {
            gameStarted = true;

            // WhiteboardCanvas 켜기
            if (whiteboardCanvas != null)
            {
                whiteboardCanvas.SetActive(true);
            }

            // MiniGameController 시작
            MeetingRoomManager.Instance?.StartMiniGame();
        }

        // 이 ❗ 숨기기
        if (myExclamationCanvas != null)
        {
            myExclamationCanvas.SetActive(false);
        }

        clearedCount++;
        Debug.Log("클릭 카운트: " + clearedCount);
    }
}