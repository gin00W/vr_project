using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SessionData
{
    public int obsessionTouchCount;    // 강박 터치 횟수
    public int wrongAnswerCount;       // 오답 횟수
    public float totalPlayTime;        // 총 플레이 시간
    public float avgResponseTime;      // 평균 반응속도
    public bool isClear;               // 클리어 여부
    public float impulseControlRate;   // 충동억제율
    public float focusScore;           // 집중력 점수
}

public class DataCollector : MonoBehaviour
{
    public static DataCollector Instance;

    [HideInInspector] public SessionData session;

    private float sessionStartTime;
    private List<float> responseTimes = new List<float>();
    private float lastWordSelectedTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        session = new SessionData();
        sessionStartTime = Time.time;
        lastWordSelectedTime = Time.time;
    }

    // 강박요소 터치 기록
    public void LogObsessionTouch()
    {
        session.obsessionTouchCount++;
    }

    // 단어 선택 반응속도 기록
    public void LogWordSelected(bool isCorrect)
    {
        float responseTime =
            Time.time - lastWordSelectedTime;
        lastWordSelectedTime = Time.time;

        if (isCorrect)
        {
            responseTimes.Add(responseTime);
            // 평균 반응속도 계산
            float total = 0f;
            foreach (var t in responseTimes)
                total += t;
            session.avgResponseTime =
                total / responseTimes.Count;
        }
        else
        {
            session.wrongAnswerCount++;
        }
    }

    // 세션 종료 및 분석
    public void EndSession(bool isClear)
    {
        session.isClear = isClear;
        session.totalPlayTime =
            Time.time - sessionStartTime;

        // 충동억제율 계산
        session.impulseControlRate = Mathf.Max(0,
            100f - session.obsessionTouchCount * 10f);

        // 집중력 점수 계산
        session.focusScore = Mathf.Max(0,
            100f - session.wrongAnswerCount * 15f);

        SaveSession();
    }

    void SaveSession()
    {
        // 누적 세션 수
        int total =
            PlayerPrefs.GetInt("TotalSessions", 0) + 1;
        PlayerPrefs.SetInt("TotalSessions", total);

        // 이전 충동억제율 저장
        PlayerPrefs.SetFloat("PrevImpulseRate",
            session.impulseControlRate);

        // 최고 집중력 점수 갱신
        float bestFocus =
            PlayerPrefs.GetFloat("BestFocusScore", 0f);
        if (session.focusScore > bestFocus)
            PlayerPrefs.SetFloat("BestFocusScore",
                session.focusScore);

        PlayerPrefs.Save();
    }

    // 성장 메시지 생성
    public string GetGrowthMessage()
    {
        float prevImpulse =
            PlayerPrefs.GetFloat("PrevImpulseRate", 0f);
        int totalSessions =
            PlayerPrefs.GetInt("TotalSessions", 0);

        if (totalSessions <= 1)
            return "첫 번째 세션을 완료했습니다 🌱";

        if (session.impulseControlRate > prevImpulse)
            return $"지난 세션보다\n충동억제율이 " +
                   $"{session.impulseControlRate - prevImpulse:F0}%" +
                   $" 향상됐어요 📈";

        if (session.impulseControlRate == 100f)
            return "완벽한 충동 억제!\n당신은 오늘 정말 강했습니다 💪";

        return "꾸준히 하다 보면\n반드시 나아집니다 🌿";
    }
}