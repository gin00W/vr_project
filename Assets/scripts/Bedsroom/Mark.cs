using UnityEngine;

using UnityEngine.SceneManagement;

public class Mark : MonoBehaviour
{
    [Header("References")]
    public StageManager stageManager;

    [Header("World UI")]
    public GameObject exclamationUIRoot;

    [Header("Start Screen UI")]
    public GameObject introPanelRoot;

    [Header("Clear Screen UI")]
    public GameObject clearPanelRoot;

    [Header("Ending Scene")]
    public string endingSceneName = "Ending sence";

    [Header("Optional - Disable clothes before start")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable[] clothInteractables;

    private bool hasStarted = false;
    private bool hasCleared = false;

    private void Start()
    {
        ShowExclamation();
        HideIntroPanel();
        HideClearPanel();
        SetClothesInteractable(false);
    }

    private void OnEnable()
    {
        if (stageManager != null)
        {
            stageManager.OnStageCleared += ShowClearPanel;
        }
    }

    private void OnDisable()
    {
        if (stageManager != null)
        {
            stageManager.OnStageCleared -= ShowClearPanel;
        }
    }

    public void ShowExclamation()
    {
        if (exclamationUIRoot != null)
            exclamationUIRoot.SetActive(true);
    }

    public void HideExclamation()
    {
        if (exclamationUIRoot != null)
            exclamationUIRoot.SetActive(false);
    }

    public void ShowIntroPanel()
    {
        if (hasStarted)
            return;

        HideExclamation();

        if (introPanelRoot != null)
            introPanelRoot.SetActive(true);
    }

    public void HideIntroPanel()
    {
        if (introPanelRoot != null)
            introPanelRoot.SetActive(false);
    }

    public void ConfirmStartMiniGame()
    {
        if (hasStarted)
            return;

        hasStarted = true;

        HideIntroPanel();
        SetClothesInteractable(true);

        if (stageManager != null)
            stageManager.StartMiniGame();
    }

    public void CancelIntroPanel()
    {
        if (hasStarted)
            return;

        HideIntroPanel();
        ShowExclamation();
    }

    public void ShowClearPanel()
    {
        if (hasCleared)
            return;

        hasCleared = true;

        if (clearPanelRoot != null)
            clearPanelRoot.SetActive(true);
    }

    public void HideClearPanel()
    {
        if (clearPanelRoot != null)
            clearPanelRoot.SetActive(false);
    }

    public void OnNextButtonClicked()
    {
        if (string.IsNullOrEmpty(endingSceneName))
        {
            Debug.LogWarning("endingSceneName이 비어 있습니다.");
            return;
        }

        SceneManager.LoadScene(endingSceneName);
    }

    private void SetClothesInteractable(bool value)
    {
        if (clothInteractables == null)
            return;

        for (int i = 0; i < clothInteractables.Length; i++)
        {
            if (clothInteractables[i] != null)
                clothInteractables[i].enabled = value;
        }
    }
}