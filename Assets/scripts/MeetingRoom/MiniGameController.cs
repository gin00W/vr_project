using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniGameController : MonoBehaviour
{
    [Header("--- 단어 시퀀스 ---")]
    public List<string> sequence = new List<string>
    { "호흡", "관찰", "수용", "집중", "완료" };

    [Header("--- 타이머 ---")]
    public float timeLimit = 60f;

    [Header("--- 강박 요소 연결 ---")]
    public ObsessiveElement[] obsessiveElements;

    [Header("--- UI 연결 ---")]
    public MeetingUIManager uiManager;

    private int currentIndex = 0;
    private float remainTime;
    private bool isPlaying = false;

    // 반응속도 측정용
    private float lastAnswerTime;

    public void StartMiniGame()
    {
        currentIndex = 0;
        remainTime = timeLimit;
        isPlaying = true;
        lastAnswerTime = Time.time;

        foreach (var e in obsessiveElements)
            e.SetObsessing(true);

        uiManager.UpdateSequenceDisplay(
            sequence, currentIndex);
        uiManager.SetupWordButtons(sequence, this);

        StartCoroutine(TimerLoop());
    }

    public void OnWordSelected(string word)
    {
        if (!isPlaying) return;

        if (word == sequence[currentIndex])
        {
            // 정답 - 반응속도 기록
            DataCollector.Instance?.LogWordSelected(true);

            currentIndex++;
            uiManager.ShowToast("✓ 정답!");

            if (currentIndex >= sequence.Count)
            {
                isPlaying = false;
                StopAllCoroutines();
                StopAllObsessions();
                MeetingRoomManager.Instance?
                    .OnMiniGameClear();
            }
            else
            {
                uiManager.UpdateSequenceDisplay(
                    sequence, currentIndex);
                uiManager.SetupWordButtons(sequence, this);
            }
        }
        else
        {
            // 오답 기록
            DataCollector.Instance?.LogWordSelected(false);

            remainTime -= 8f;
            uiManager.ShowToast("❌ 틀렸어요! -8초");
            StartCoroutine(CameraShake());
        }
    }

    IEnumerator TimerLoop()
    {
        while (isPlaying && remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            uiManager.UpdateTimer(remainTime, timeLimit);
            yield return null;
        }

        if (isPlaying)
        {
            isPlaying = false;
            StopAllObsessions();
            MeetingRoomManager.Instance?.OnMiniGameFail();
        }
    }

    void StopAllObsessions()
    {
        foreach (var e in obsessiveElements)
            e.SetObsessing(false);
    }

    IEnumerator CameraShake()
    {
        Transform cam = Camera.main?.transform;
        if (cam == null) yield break;

        Vector3 origin = cam.localPosition;
        float elapsed = 0f;

        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            cam.localPosition = origin +
                Random.insideUnitSphere * 0.015f;
            yield return null;
        }

        cam.localPosition = origin;
    }
}