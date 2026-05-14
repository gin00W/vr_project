using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MeetingRoomManager : MonoBehaviour
{
    public static MeetingRoomManager Instance;

    [Header("--- 씬 이동 ---")]
    public string nextSceneName = "Bathroom";

    [Header("--- 연결 ---")]
    public MiniGameController miniGame;
    public DoorInteraction door;
    public LightController lightCtrl;
    public MeetingUIManager uiManager;

    // 결과 기록
    [HideInInspector] public int resistCount = 0;
    [HideInInspector] public float clearTime = 0f;
    private float startTime;

    public enum State { Intro, Playing, Clear, Fail }
    [HideInInspector] public State currentState;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        startTime = Time.time;
        Invoke(nameof(StartPlaying), 2f);
    }

    void StartPlaying()
    {
        currentState = State.Playing;
        miniGame.StartMiniGame();

        // 불안 모드 조명
        AtmosphereController.Instance?.SetAnxiousMode();
    }

    public void OnMiniGameClear()
    {
        if (currentState != State.Playing) return;

        currentState = State.Clear;
        clearTime = Time.time - startTime;

        // 데이터 수집 종료
        DataCollector.Instance?.EndSession(true);

        // 편안 모드 조명
        AtmosphereController.Instance?.SetCalmMode();

        // 보상 시스템
        if (DataCollector.Instance != null)
            RewardSystem.Instance?.ShowReward(
                DataCollector.Instance.session);

        door.Unlock();
        uiManager.ShowClearUI(resistCount, clearTime);
    }

    public void OnMiniGameFail()
    {
        if (currentState != State.Playing) return;

        currentState = State.Fail;

        // 데이터 수집 종료
        DataCollector.Instance?.EndSession(false);

        // 실패 조명
        AtmosphereController.Instance?.SetFailMode();

        uiManager.ShowFailUI();
        Invoke(nameof(ResetScene), 3f);
    }

    public void AddResist()
    {
        resistCount++;
        uiManager.ShowToast($"참았어요! 💪 ({resistCount}회)");
    }

    public void GoNextScene()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextSceneName);
    }

    void ResetScene()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }
}