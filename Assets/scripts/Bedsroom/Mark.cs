using UnityEngine;

using TMPro;

public class Mark : MonoBehaviour
{
    [Header("References")]
    public StageManager stageManager;

    [Header("World UI")]
    public GameObject exclamationUIRoot;

    [Header("Start Screen UI")]
    public GameObject introPanelRoot;
    public TextMeshProUGUI introText;

    [TextArea(2, 5)]
    public string startMessage = "옷을 다시 배치해 각 층의 조건을 완성하세요.";

    [Header("Clear Screen UI")]
    public GameObject clearPanelRoot;
    public TextMeshProUGUI clearText;

    [TextArea(2, 5)]
    public string clearMessage = "잘했어요! 옷장의 조건을 모두 완성했습니다.";

    [Header("Optional - Disable clothes before start")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable[] clothInteractables;

    private bool hasStarted = false;

    private void Start()
    {
        ShowExclamation();
        HideIntroPanel();
        HideClearPanel();
        SetClothesInteractable(false);
        ApplyMessage();
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

    private void ApplyMessage()
    {
        if (introText != null)
            introText.text = startMessage;

        if (clearText != null)
            clearText.text = clearMessage;
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

        ApplyMessage();
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
        ApplyMessage();

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
        HideClearPanel();
        Debug.Log("다음으로 버튼이 눌렸습니다. 다음 스테이지 이동 기능은 아직 미구현 상태입니다.");
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