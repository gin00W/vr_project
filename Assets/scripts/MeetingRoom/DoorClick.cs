using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorClick : MonoBehaviour
{
    [Header("--- 다음 씬 ---")]
    public string nextSceneName = "room03";

    [Header("--- 강박 클리어 조건 ---")]
    public bool requireAllOCDCleared = true;

    [Header("--- 안내 메시지 (선택) ---")]
    public GameObject warningText;  // "강박 먼저 클리어" 같은 메시지

    void OnMouseDown()
    {
        Debug.Log("문 클릭됨!");

        if (requireAllOCDCleared)
        {
            // 강박 모두 클리어 됐는지 확인
            if (OCDExclamationClick.clearedCount >= OCDExclamationClick.totalElements)
            {
                Debug.Log("모든 강박 클리어! room03으로 이동!");

                // 클리어 카운트 리셋 (다음 방을 위해)
                OCDExclamationClick.clearedCount = 0;

                // 다음 씬 로드
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                int remaining = OCDExclamationClick.totalElements - OCDExclamationClick.clearedCount;
                Debug.Log("강박을 먼저 클리어하세요! 남은 개수: " + remaining);

                // 경고 메시지 표시 (선택)
                if (warningText != null)
                {
                    warningText.SetActive(true);
                    Invoke("HideWarning", 2f);
                }
            }
        }
        else
        {
            // 조건 없이 바로 이동
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void HideWarning()
    {
        if (warningText != null)
            warningText.SetActive(false);
    }
}