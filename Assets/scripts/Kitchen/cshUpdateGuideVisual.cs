using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public sealed class FloatingWarningIndicator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform targetObject;

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Floating Motion")]
    [SerializeField] private float floatAmplitude = 0.015f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Interaction")]
    [SerializeField] private XRSimpleInteractable interactable;

    [Header("Warning Color")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private float colorPulseSpeed = 2f;

    private bool hasInteracted;

    private Material targetMaterial;
    private Color originalColor;

    private Vector3 initialPosition;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetRenderer != null)
        {
            targetMaterial = targetRenderer.material;
            originalColor = targetMaterial.color;
        }

        if (interactable == null && targetObject != null)
        {
            interactable = targetObject.GetComponent<XRSimpleInteractable>();
        }
    }

    private void Start()
    {
        // 씬에 배치된 현재 위치 저장
        initialPosition = transform.position;
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnInteracted);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnInteracted);
        }
    }

    private void Update()
    {
        if (hasInteracted)
        {
            return;
        }

        UpdateFloatingMotion();
        UpdateLookAtCamera();
        UpdateWarningColor();
    }

    private void UpdateFloatingMotion()
    {
        float floatOffset =
            Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        Vector3 targetPosition = initialPosition;
        targetPosition.y += floatOffset;

        transform.position = targetPosition;
    }

    private void UpdateLookAtCamera()
    {
        if (targetCamera == null)
        {
            return;
        }

        Vector3 toCamera =
            transform.position - targetCamera.transform.position;

        if (toCamera.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        transform.rotation =
            Quaternion.LookRotation(toCamera.normalized, Vector3.up);
    }

    private void UpdateWarningColor()
    {
        if (targetMaterial == null)
        {
            return;
        }

        float t =
            (Mathf.Sin(Time.time * colorPulseSpeed) + 1f) * 0.5f;

        Color currentColor =
            Color.Lerp(originalColor, warningColor, t);

        targetMaterial.color = currentColor;
    }

    private void OnInteracted(SelectEnterEventArgs args)
    {
        hasInteracted = true;

        // 원래 색상 복원
        if (targetMaterial != null)
        {
            targetMaterial.color = originalColor;
        }

        // 느낌표 비활성화
        gameObject.SetActive(false);
    }
}