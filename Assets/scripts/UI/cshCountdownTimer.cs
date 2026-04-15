using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class CountdownTimer : MonoBehaviour
{
    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTimeSeconds = 90f;

    [Header("Time Out Popup")]
    [SerializeField] private GameObject timeOutPopup;
    [SerializeField] private Button nextStageButton;
    [SerializeField] private string nextStageSceneName;

    private float remainingTime;
    private bool isFinished;

    private void Awake()
    {
        remainingTime = Mathf.Max(0f, startTimeSeconds);

        if (timeOutPopup != null)
        {
            timeOutPopup.SetActive(false);
        }

        if (nextStageButton != null)
        {
            nextStageButton.onClick.AddListener(GoToNextStage);
        }

        UpdateTimerText();
    }

    private void Update()
    {
        if (isFinished)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimerText();
            HandleTimeOut();
            return;
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        int totalSeconds = Mathf.CeilToInt(remainingTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void HandleTimeOut()
    {
        isFinished = true;
        Time.timeScale = 0f;

        if (timeOutPopup != null)
        {
            timeOutPopup.SetActive(true);
        }
    }

    public void GoToNextStage()
    {
        if (string.IsNullOrWhiteSpace(nextStageSceneName))
        {
            Debug.LogError("Next Stage Scene Name이 비어 있습니다.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextStageSceneName);
    }

    private void OnDestroy()
    {
        if (nextStageButton != null)
        {
            nextStageButton.onClick.RemoveListener(GoToNextStage);
        }

        Time.timeScale = 1f;
    }
}