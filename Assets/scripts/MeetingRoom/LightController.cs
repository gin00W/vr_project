using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour
{
    [Header("--- 조명 연결 ---")]
    public Light[] roomLights;

    [Header("--- 강도 설정 ---")]
    public float normalIntensity = 1.2f;
    public float obsessionIntensity = 0.5f;
    public float clearIntensity = 1.8f;

    [Header("--- 색상 ---")]
    public Color normalColor = Color.white;
    public Color obsessionColor = new Color(1f, 0.95f, 0.8f);

    public void SetObsessionMode(bool active)
    {
        StopAllCoroutines();

        if (active)
            StartCoroutine(Transition(obsessionIntensity, obsessionColor, 2f));
        else
            StartCoroutine(Transition(clearIntensity, normalColor, 1f));
    }

    IEnumerator Transition(float targetInt, Color targetColor, float duration)
    {
        if (roomLights.Length == 0) yield break;

        float startInt = roomLights[0].intensity;
        Color startColor = roomLights[0].color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            foreach (var l in roomLights)
            {
                if (l == null) continue;
                l.intensity = Mathf.Lerp(startInt, targetInt, t);
                l.color = Color.Lerp(startColor, targetColor, t);
            }

            yield return null;
        }
    }
}