using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public GameObject mainMenuButton;

    void Start()
    {
        text1.alpha = 0;
        text2.alpha = 0;
        text3.alpha = 0;
        mainMenuButton.SetActive(false);
        StartCoroutine(PlayEnding());
    }

    IEnumerator PlayEnding()
    {
        yield return FadeIn(text1, 2f);
        yield return new WaitForSeconds(1.5f);

        yield return FadeIn(text2, 2f);
        yield return new WaitForSeconds(1.5f);

        yield return FadeIn(text3, 2f);
        yield return new WaitForSeconds(1.5f);

        mainMenuButton.SetActive(true);
    }

    IEnumerator FadeIn(TextMeshProUGUI text, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
        text.alpha = 1;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("start scene");
    }
}