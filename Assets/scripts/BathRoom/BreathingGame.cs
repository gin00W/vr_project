using UnityEngine;
using TMPro;
using System.Collections;

public class BreathingGame : MonoBehaviour
{
    public GameObject breathingGameGroup;
    public TextMeshProUGUI breathingText;
    public RectTransform breathingCircle;

    public GameObject exclamationObject;

    private bool isRunning = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (breathingGameGroup != null)
            breathingGameGroup.SetActive(false);
    }

    public void StartBreathing()
    {
        Debug.Log("느낌표 선택됨 - 심호흡 미니게임 시작");

        if (isRunning)
            return;

        if (exclamationObject != null)
            exclamationObject.SetActive(false);

        if (breathingGameGroup != null)
            breathingGameGroup.SetActive(true);
        else
        {
            Debug.LogError("BreathingGameGroup이 연결되지 않았습니다.");
            return;
        }

        if (breathingText == null)
        {
            Debug.LogError("BreathingText가 연결되지 않았습니다.");
            return;
        }

        if (breathingCircle == null)
        {
            Debug.LogError("BreathingCircle이 연결되지 않았습니다.");
            return;
        }

        StartCoroutine(BreathingRoutine());
    }

    IEnumerator BreathingRoutine()
    {
        isRunning = true;

        int cycles = 3;

        for (int i = 0; i < cycles; i++)
        {
            breathingText.text = "들이쉬세요...";
            yield return ScaleCircle(50f, 120f, 4f);

            breathingText.text = "참으세요...";
            yield return new WaitForSeconds(2f);

            breathingText.text = "내쉬세요...";
            yield return ScaleCircle(120f, 50f, 4f);
        }

        breathingText.text = "잘하셨어요";
        yield return new WaitForSeconds(2f);

        StopBreathing();
    }

    public void StopBreathing()
    {
        isRunning = false;

        if (breathingGameGroup != null)
            breathingGameGroup.SetActive(false);
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

        breathingCircle.sizeDelta = new Vector2(to, to);
    }
}