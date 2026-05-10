using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BreathingGame : MonoBehaviour
{
    public GameObject breathingGameGroup;
    public TextMeshProUGUI breathingText;
    public RectTransform breathingCircle;

    private bool isRunning = false;

    public void StartBreathing()
    {
        breathingGameGroup.SetActive(true);
        if (!isRunning)
            StartCoroutine(BreathingRoutine());
    }

    public void StopBreathing()
    {
        breathingGameGroup.SetActive(false);
        isRunning = false;
        StopAllCoroutines();
    }

    IEnumerator BreathingRoutine()
    {
        isRunning = true;
        int cycles = 3;

        for (int i = 0; i < cycles; i++)
        {
            // 들이쉬기 (4초)
            breathingText.text = "들이쉬세요...";
            yield return ScaleCircle(50f, 120f, 4f);

            // 참기 (2초)
            breathingText.text = "참으세요...";
            yield return new WaitForSeconds(2f);

            // 내쉬기 (4초)
            breathingText.text = "내쉬세요...";
            yield return ScaleCircle(120f, 50f, 4f);
        }

        breathingText.text = "잘하셨어요";
        yield return new WaitForSeconds(2f);
        StopBreathing();
    }

    IEnumerator ScaleCircle(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float size = Mathf.Lerp(from, to, elapsed / duration);
            breathingCircle.sizeDelta = new Vector2(size, size);
            yield return null;
        }
    }
}