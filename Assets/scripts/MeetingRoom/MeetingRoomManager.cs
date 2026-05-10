using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MeetingRoomManager : MonoBehaviour
{
    public static MeetingRoomManager Instance;

    // 씬 이름 - 인스펙터에서 수정 가능하게
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

    // 게임 상태
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
        // 2초 후 게임 시작 (씬 전환 후 적응 시간)
        Invoke(nameof(StartPlaying), 2f);
    }

    void StartPlaying()
    {
        currentState = State.Playing;
        miniGame.StartMiniGame();
        lightCtrl.SetObsessionMode(true);
    }

    // 미니게임 클리어 시 호출
    public void OnMiniGameClear()
    {
        if (currentState != State.Playing) return;

        currentState = State.Clear;
        clearTime = Time.time - startTime;

        lightCtrl.SetObsessionMode(false);
        door.Unlock();
        uiManager.ShowClearUI(resistCount, clearTime);
    }

    // 미니게임 실패 시 호출
    public void OnMiniGameFail()
    {
        if (currentState != State.Playing) return;

        currentState = State.Fail;
        uiManager.ShowFailUI();

        // 3초 후 씬 리셋
        Invoke(nameof(ResetScene), 3f);
    }

    // 강박 참기 카운트
    public void AddResist()
    {
        resistCount++;
        uiManager.ShowToast($"참았어요! 💪 ({resistCount}회)");
    }

    // 다음 씬으로 이동 (Door에서 호출)
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}