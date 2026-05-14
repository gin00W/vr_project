using UnityEngine;
using System.Collections;

public class AtmosphereController : MonoBehaviour
{
    public static AtmosphereController Instance;

    [Header("--- 조명 ---")]
    public Light[] roomLights;
    public Light directionalLight;

    [Header("--- 불안 상태 색상 ---")]
    public Color anxiousColor = new Color(0.8f, 0.3f, 0.3f);
    public float anxiousIntensity = 0.4f;

    [Header("--- 편안 상태 색상 ---")]
    public Color calmColor = new Color(1f, 0.95f, 0.8f);
    public float calmIntensity = 1.5f;

    [Header("--- 기본 상태 ---")]
    public Color normalColor = Color.white;
    public float normalIntensity = 1f;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 안전 처리 - 아무것도 안 함
    }

    public void SetAnxiousMode()
    {
        // 안전 처리
    }

    public void SetCalmMode()
    {
        // 안전 처리
    }

    public void SetFailMode()
    {
        // 안전 처리
    }
}