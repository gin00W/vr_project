using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public GameObject panelGuide;
    public GameObject panelWarning;
    public GameObject startButton;
    public GameObject gameTitle;
    public GameObject descriptionText;

    void Start()
    {
        panelGuide.SetActive(false);
        panelWarning.SetActive(false);
    }

    public void OnStartButtonClick()
    {
        startButton.SetActive(false);
        gameTitle.SetActive(false);
        descriptionText.SetActive(false);
        panelGuide.SetActive(true);
    }

    public void OnNextButtonClick()
    {
        panelGuide.SetActive(false);
        panelWarning.SetActive(true);
    }

    public void OnStartGameButtonClick()
    {
        SceneManager.LoadScene("room01");
    }
}