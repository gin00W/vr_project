using UnityEngine;
using System.Collections;

public class AtmosphereController : MonoBehaviour
{
    public static AtmosphereController Instance;

    [Header("--- 조명 ---")]
    public Light[] roomLights;
    public Light directionalLight;

    [Header("--- 불안 상태 색상 ---")]
    public Color anxiousColor =
        new Color(0.8f, 0.3f, 0.3f);      // 붉은색
    public float anxiousIntensity = 0.4f;

    [Header("--- 편안 상태 색상 ---")]
    public Color calmColor =
        new Color(1f, 0.95f, 0.8f);        // 따뜻한 흰색
    public float calmIntensity = 1.5f;

    [Header("--- 기본 상태 ---")]
    public Color normalColor = Color.white;
    public float normalIntensity = 1f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        SetLights(normalColor, normalIntensity);
    }

    // 강박 상태 (게임 시작)
    public void SetAnxiousMode()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionLights(
            anxiousColor, anxiousIntensity, 2f));
    }

    // 클리어 상태
    public void SetCalmMode()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionLights(
            calmColor, calmIntensity, 1.5f));
    }

    // 실패 상태
    public void SetFailMode()
    {
        StopAllCoroutines();
        StartCoroutine(FailFlicker());
    }

    // 조명 깜빡임 (실패 시)
    IEnumerator FailFlicker()
    {
        float elapsed = 0f;
        while (elapsed < 1.5f)
        {
            elapsed += Time.deltaTime;
            float flicker = Mathf.Sin(
                Time.time * 15f) * 0.5f + 0.5f;
            SetLights(anxiousColor,
                Mathf.Lerp(0.1f, anxiousIntensity,
                    flicker));
            yield return null;
        }

        // 깜빡임 후 어둡게
        yield return StartCoroutine(TransitionLights(
            anxiousColor, 0.3f, 1f));
    }

    IEnumerator TransitionLights(
        Color targetColor, float targetIntensity,
        float duration)
    {
        if (roomLights.Length == 0) yield break;

        Color startColor = roomLights[0].color;
        float startIntensity = roomLights[0].intensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // ease out
            t = 1f - Mathf.Pow(1f - t, 2f);

            Color current = Color.Lerp(
                startColor, targetColor, t);
            float intensity = Mathf.Lerp(
                startIntensity, targetIntensity, t);

            SetLights(current, intensity);
            yield return null;
        }
    }

    void SetLights(Color color, float intensity)
    {
        foreach (var l in roomLights)
        {
            if (l == null) continue;
            l.color = color;
            l.intensity = intensity;
        }

        if (directionalLight != null)
            directionalLight.intensity = intensity * 0.5f;
    }
}