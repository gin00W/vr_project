using UnityEngine;
using TMPro;
using System.Collections;

public class BreathingGame : MonoBehaviour
{
    public GameObject breathingButton;      // 시작 버튼
    public GameObject breathingGameGroup;   // 심호흡 UI 전체
    public TextMeshProUGUI breathingText;
    public RectTransform breathingCircle;

    public GameObject exclamationObject;    // 느낌표 오브젝트

    private bool isRunning = false;

    void Start()
    {
        if (breathingButton != null)
            breathingButton.SetActive(false);

        if (breathingGameGroup != null)
            breathingGameGroup.SetActive(false);
    }

    // 느낌표를 눌렀을 때 실행될 함수
    public void ShowBreathingButton()
    {
        Debug.Log("느낌표 선택됨: 심호흡 시작 버튼 표시");

        if (exclamationObject != null)
            exclamationObject.SetActive(false);

        if (breathingButton != null)
            breathingButton.SetActive(true);
        else
            Debug.LogError("breathingButton이 연결되지 않았습니다.");
    }

    // BreathingButton을 눌렀을 때 실행될 함수
    public void StartBreathing()
    {
        Debug.Log("심호흡 미니게임 시작");

        if (isRunning)
            return;

        if (breathingButton != null)
            breathingButton.SetActive(false);

        if (breathingGameGroup != null)
            breathingGameGroup.SetActive(true);
        else
            Debug.LogError("breathingGameGroup이 연결되지 않았습니다.");

        StartCoroutine(BreathingRoutine());
    }

    public void StopBreathing()
    {
        isRunning = false;

        if (breathingGameGroup != null)
            breathingGameGroup.SetActive(false);
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