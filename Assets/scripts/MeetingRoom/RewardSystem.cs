using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RewardSystem : MonoBehaviour
{
    public static RewardSystem Instance;

    [Header("--- 성찰 메시지 ---")]
    private string[] reflectionMessages =
    {
        "불편함을 느꼈지만\n그것이 당신을 지배하지 못했습니다",
        "오늘 당신은 충동보다\n더 강했습니다",
        "완벽하지 않아도 괜찮습니다\n그 자체로 충분합니다",
        "불안은 당신의 일부이지만\n당신의 전부가 아닙니다",
        "오늘의 작은 전진이\n내일의 큰 변화를 만듭니다"
    };

    [Header("--- 치료사 코멘트 ---")]
    private string[] therapistComments =
    {
        "강박적 충동을 인식하고\n행동으로 옮기지 않은 것\n매우 잘 하셨습니다",
        "주의를 전환하는 능력이\n점점 향상되고 있어요",
        "호흡을 통해 스스로를\n조절할 수 있음을 경험했습니다",
        "오늘 경험한 것을\n일상에서도 적용해보세요"
    };

    [Header("--- UI 연결 ---")]
    public GameObject reportPanel;
    public TextMeshProUGUI reflectionText;
    public TextMeshProUGUI therapistText;
    public TextMeshProUGUI impulseBarText;
    public TextMeshProUGUI focusBarText;
    public TextMeshProUGUI responseTimeText;
    public TextMeshProUGUI growthText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (reportPanel != null)
            reportPanel.SetActive(false);
    }

    public void ShowReward(SessionData data)
    {
        StartCoroutine(RewardSequence(data));
    }

    IEnumerator RewardSequence(SessionData data)
    {
        yield return new WaitForSeconds(1.5f);

        // 1. 성찰 메시지
        string reflection = reflectionMessages[
            Random.Range(0, reflectionMessages.Length)];

        if (reflectionText != null)
        {
            reflectionText.text = reflection;
            yield return StartCoroutine(
                FadeInText(reflectionText, 1.5f));
        }

        yield return new WaitForSeconds(2f);

        // 2. 리포트 패널
        if (reportPanel != null)
        {
            reportPanel.SetActive(true);
            UpdateReportUI(data);
        }

        yield return new WaitForSeconds(2f);

        // 3. 치료사 코멘트
        string comment = therapistComments[
            Random.Range(0, therapistComments.Length)];

        if (therapistText != null)
        {
            therapistText.text =
                $"💬 {comment}";
            yield return StartCoroutine(
                FadeInText(therapistText, 1f));
        }
    }

    void UpdateReportUI(SessionData data)
    {
        // 충동억제율 바
        if (impulseBarText != null)
            impulseBarText.text =
                $"충동억제   " +
                $"{GetBar(data.impulseControlRate)} " +
                $"{data.impulseControlRate:F0}%";

        // 집중력 바
        if (focusBarText != null)
            focusBarText.text =
                $"집중력     " +
                $"{GetBar(data.focusScore)} " +
                $"{data.focusScore:F0}%";

        // 반응속도
        if (responseTimeText != null)
            responseTimeText.text =
                $"반응속도   " +
                $"{data.avgResponseTime:F1}초";

        // 성장 메시지
        if (growthText != null)
            growthText.text =
                DataCollector.Instance?.GetGrowthMessage();
    }

    // 프로그레스 바 텍스트
    string GetBar(float value)
    {
        int filled = Mathf.RoundToInt(value / 20f);
        string bar = "";
        for (int i = 0; i < 5; i++)
            bar += i < filled ? "█" : "░";
        return bar;
    }

    // 텍스트 페이드인
    IEnumerator FadeInText(TextMeshProUGUI tmp,
        float duration)
    {
        Color c = tmp.color;
        c.a = 0f;
        tmp.color = c;
        tmp.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = elapsed / duration;
            tmp.color = c;
            yield return null;
        }

        c.a = 1f;
        tmp.color = c;
    }
}