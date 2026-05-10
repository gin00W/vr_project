using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MeetingUIManager : MonoBehaviour
{
    [Header("--- 텍스트 ---")]
    public TextMeshProUGUI sequenceText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI toastText;
    public TextMeshProUGUI resultText;

    [Header("--- 버튼 (5개 미리 배치) ---")]
    public Button[] wordButtons;

    // 시퀀스 표시 업데이트
    public void UpdateSequenceDisplay(List<string> words, int currentIdx)
    {
        string display = "";

        for (int i = 0; i < words.Count; i++)
        {
            if (i < currentIdx)
            {
                // 완료된 단어 - 회색 취소선
                display += $"<color=#666666><s>{words[i]}</s></color>  ";
            }
            else if (i == currentIdx)
            {
                // 현재 선택해야 할 단어 - 노란색 강조
                display += $"<color=#FFE44D><b>▶ {words[i]}</b></color>  ";
            }
            else
            {
                // 아직 안 한 단어 - 흰색
                display += $"<color=#CCCCCC>{words[i]}</color>  ";
            }
        }

        if (sequenceText != null)
            sequenceText.text = display;
    }

    // 버튼 셔플 세팅
    public void SetupWordButtons(List<string> words, MiniGameController controller)
    {
        // 셔플
        List<string> shuffled = new List<string>(words);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        for (int i = 0; i < wordButtons.Length; i++)
        {
            if (i < shuffled.Count)
            {
                wordButtons[i].gameObject.SetActive(true);

                // 텍스트 세팅
                var tmp = wordButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null) tmp.text = shuffled[i];

                // 클릭 이벤트
                string capturedWord = shuffled[i];
                wordButtons[i].onClick.RemoveAllListeners();
                wordButtons[i].onClick.AddListener(
                    () => controller.OnWordSelected(capturedWord)
                );
            }
            else
            {
                wordButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // 타이머 표시
    public void UpdateTimer(float remain, float total)
    {
        if (timerText == null) return;

        timerText.text = $"⏱ {Mathf.Max(0, remain):F0}초";

        // 15초 이하면 빨간색 깜빡임
        if (remain < 15f)
        {
            float blink = Mathf.Sin(Time.time * 6f) * 0.5f + 0.5f;
            timerText.color = Color.Lerp(Color.red, Color.white, blink);
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    // 토스트 메시지
    public void ShowToast(string msg)
    {
        StopCoroutine(nameof(ToastRoutine));
        StartCoroutine(ToastRoutine(msg));
    }

    IEnumerator ToastRoutine(string msg)
    {
        if (toastText == null) yield break;

        toastText.gameObject.SetActive(true);
        toastText.text = msg;

        Color c = toastText.color;
        c.a = 1f;
        toastText.color = c;

        yield return new WaitForSeconds(1.5f);

        // 페이드 아웃
        float elapsed = 0f;
        while (elapsed < 0.4f)
        {
            elapsed += Time.deltaTime;
            c.a = 1f - (elapsed / 0.4f);
            toastText.color = c;
            yield return null;
        }

        toastText.gameObject.SetActive(false);
    }

    // 클리어 화면
    public void ShowClearUI(int resistCount, float clearTime)
    {
        if (resultText == null) return;

        resultText.gameObject.SetActive(true);
        resultText.text =
            "<color=#4CAF50><b>✓ 미션 완료!</b></color>\n\n" +
            $"클리어 시간: <b>{clearTime:F1}초</b>\n" +
            $"충동 억제: <b>{resistCount}회</b>\n\n" +
            "<size=70%><color=#AAAAAA>문을 열고 나가세요</color></size>";
    }

    // 실패 화면
    public void ShowFailUI()
    {
        if (resultText == null) return;

        resultText.gameObject.SetActive(true);
        resultText.text =
            "<color=#F44336><b>시간 초과!</b></color>\n\n" +
            "다시 시도합니다...";
    }
}