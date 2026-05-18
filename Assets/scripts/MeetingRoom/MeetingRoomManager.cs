using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MeetingRoomManager : MonoBehaviour
{
    public static MeetingRoomManager Instance;

    [Header("--- 씬 이동 ---")]
    public string nextSceneName = "room03";

    [Header("--- 연결 ---")]
    public MiniGameController miniGame;
    public DoorInteraction door;
    public LightController lightCtrl;
    public MeetingUIManager uiManager;

    [Header("--- 게임 UI ---")]
    public GameObject whiteboardCanvas;

    [HideInInspector] public int resistCount = 0;
    [HideInInspector] public float clearTime = 0f;
    private float startTime;
    private bool gameStarted = false;

    public enum State { Intro, Waiting, Playing, Clear, Fail }
    [HideInInspector] public State currentState;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        startTime = Time.time;
        currentState = State.Waiting;

        // UI 끄기
        if (whiteboardCanvas != null)
            whiteboardCanvas.SetActive(false);
    }

    public void StartMiniGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        Debug.Log("미니게임 시작!");

        currentState = State.Playing;
        startTime = Time.time;  // ⭐ 타이머 리셋

        if (whiteboardCanvas != null)
            whiteboardCanvas.SetActive(true);

        miniGame.StartMiniGame();

        AtmosphereController.Instance?.SetAnxiousMode();
    }

    public void OnMiniGameClear()
    {
        if (currentState != State.Playing) return;

        currentState = State.Clear;
        clearTime = Time.time - startTime;

        Debug.Log("미니게임 클리어! 시간: " + clearTime);

        DataCollector.Instance?.EndSession(true);
        AtmosphereController.Instance?.SetCalmMode();

        // 결과 화면 표시
        if (uiManager != null)
            uiManager.ShowClearUI(resistCount, clearTime);

        // 보상 시스템 (있으면)
        if (DataCollector.Instance != null && RewardSystem.Instance != null)
            RewardSystem.Instance.ShowReward(DataCollector.Instance.session);

        // ⭐ 결과 화면 보여주고 3초 후 다음 씬
        Invoke(nameof(GoNextScene), 3f);
    }

    public void OnMiniGameFail()
    {
        if (currentState != State.Playing) return;

        currentState = State.Fail;
        Debug.Log("미니게임 실패!");

        DataCollector.Instance?.EndSession(false);
        AtmosphereController.Instance?.SetFailMode();

        if (uiManager != null)
            uiManager.ShowFailUI();

        // ⭐ 3초 후 다시 시도
        Invoke(nameof(RetryGame), 3f);
    }

    // ⭐ 다시 시도
    void RetryGame()
    {
        Debug.Log("다시 시도!");

        // 상태 초기화
        gameStarted = false;
        currentState = State.Waiting;
        resistCount = 0;
        startTime = Time.time;

        // OCD 카운트 리셋
        OCDExclamationClick.clearedCount = 0;
        OCDExclamationClick.gameStarted = false;

        // 다시 게임 시작
        StartMiniGame();
    }

    public void AddResist()
    {
        resistCount++;
        if (uiManager != null)
            uiManager.ShowToast($"참았어요! 💪 ({resistCount}회)");
    }

    public void GoNextScene()
    {
        Debug.Log("다음 씬으로: " + nextSceneName);
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1f);

        // 정적 변수 초기화
        OCDExclamationClick.clearedCount = 0;
        OCDExclamationClick.gameStarted = false;

        SceneManager.LoadScene(nextSceneName);
    }

    void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}