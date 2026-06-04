using System.Collections;
using UnityEngine;

public sealed class HighlightPulse : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;

    [Header("Pulse")]
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private float pulseSpeed = 2f;

    private Material materialInstance;
    private Color originalColor;

    private bool isStopped;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        materialInstance = targetRenderer.material;
        originalColor = materialInstance.color;
    }

    private void OnEnable()
    {
        StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        while (!isStopped)
        {
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);

            materialInstance.color =
                Color.Lerp(
                    originalColor,
                    highlightColor,
                    t);

            yield return null;
        }
    }

    /// <summary>
    /// 최초 상호작용 시 호출
    /// </summary>
    public void StopHighlight()
    {
        if (isStopped)
            return;

        isStopped = true;

        materialInstance.color = originalColor;
    }
}