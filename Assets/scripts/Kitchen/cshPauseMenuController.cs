using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;

    private void Awake()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OpenPausePopup);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
    }

    private void OnDestroy()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveListener(OpenPausePopup);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(ResumeGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }

        Time.timeScale = 1f;
    }

    public void OpenPausePopup()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(true);
        }

        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }

        isPaused = false;
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}