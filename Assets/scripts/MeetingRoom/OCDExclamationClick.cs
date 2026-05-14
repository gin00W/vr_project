using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OCDExclamationClick : MonoBehaviour
{
    [Header("--- 활성화할 게임 UI ---")]
    public GameObject whiteboardCanvas;

    [Header("--- 숨길 자기 자신 (Canvas) ---")]
    public GameObject myExclamationCanvas;

    public static int clearedCount = 0;
    public static int totalElements = 4;

    [Header("--- 다음 씬 ---")]
    public string nextSceneName = "room03";
    public float waitBeforeNextScene = 3f;

    void Start()
    {
        Button btn = GetComponentInChildren<Button>();

        if (btn != null)
        {
            btn.onClick.AddListener(OnClick);
            Debug.Log(gameObject.name + ": Button 찾음, 이벤트 등록!");
        }
        else
        {
            Debug.LogError(gameObject.name + ": Button 못 찾음!");
        }
    }

    public void OnClick()
    {
        Debug.Log(gameObject.name + " 클릭됨!");

        if (whiteboardCanvas != null)
        {
            whiteboardCanvas.SetActive(true);
        }

        if (myExclamationCanvas != null)
        {
            myExclamationCanvas.SetActive(false);
        }

        clearedCount++;
        Debug.Log("클리어: " + clearedCount + "/" + totalElements);

        if (clearedCount >= totalElements)
        {
            Invoke("LoadNextScene", waitBeforeNextScene);
        }
    }

    void LoadNextScene()
    {
        Debug.Log("다음 씬으로: " + nextSceneName);
        clearedCount = 0;
        SceneManager.LoadScene(nextSceneName);
    }
}