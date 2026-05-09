using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public sealed class GasValveSkillCheckController : MonoBehaviour
{
    [Header("XR")]
    [SerializeField] private XRSimpleInteractable interactable;
    [SerializeField] private InputActionProperty vrRotateHoldAction;

    [Header("PC Test")]
    [SerializeField] private bool enableMouseTestInEditor = true;

    [Header("Valve")]
    [SerializeField] private Transform valveTarget;
    [SerializeField] private Vector3 localRotationAxis = Vector3.forward;
    [SerializeField] private float successRotateLeftDegrees = 10f;
    [SerializeField] private float minValveAngleDegrees = 0f;
    [SerializeField] private float failValveAngleDegrees = 90f;

    [Header("VR Rotation")]
    [SerializeField] private float vrRotationScale = 1f;
    [SerializeField] private bool invertVrClockwise = true;

    [Header("Mouse Rotation")]
    [SerializeField] private float mouseRotationScale = 1f;
    [SerializeField] private bool invertMouseClockwise = false;

    [Header("Guide Image")]
    [SerializeField] private GameObject guideRoot;

    [Header("Skill Check UI")]
    [SerializeField] private GameObject skillCheckRoot;
    [SerializeField] private CanvasGroup skillCheckCanvasGroup;
    [SerializeField] private RectTransform redMarkerImage;
    [SerializeField] private RectTransform blueZoneImage;

    [Header("World Placement")]
    [SerializeField] private Vector3 uiWorldOffset = new Vector3(0f, 0.25f, 0f);
    [SerializeField] private Camera targetCamera;

    [Header("Arc Motion")]
    [SerializeField] private float arcRadius = 180f;
    [SerializeField] private float arcStartAngle = 160f;
    [SerializeField] private float arcEndAngle = 20f;
    [SerializeField] private float blueZoneHalfWidthDegrees = 8f;
    [SerializeField] private float redMarkerMoveDegreesPerSecond = 120f;
    [SerializeField] private float rotationToTriggerSkillCheckDegrees = 25f;

    [Header("Result")]
    [SerializeField] private float fadeOutSeconds = 1f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip resultClip;

    [Header("Radio After Skill Check")]
    [SerializeField] private RadioAudioFeedbackController radioFeedback;
    [SerializeField] private AudioClip room1Voice1;
    [SerializeField]
    [TextArea]
    private string room1Subtitle1 =
        "가스 검침은 국가적으로 시행되고 있으니 안심하세요.";
    [SerializeField] private AudioClip room1Voice2;
    [SerializeField]
    [TextArea]
    private string room1Subtitle2 =
        "가스 밸브를 안잠궜다고 불이 나진 않아요.";
    [SerializeField] private AudioClip room1Voice3;
    [SerializeField]
    [TextArea]
    private string room1Subtitle3 =
        "밸브는 가스가 실제로 누수되고 있는 경우에만 위험합니다.";

    [Header("Stage Fail Popup")]
    [SerializeField] private GameObject stageFailPopupRoot;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private string mainMenuSceneName = "start";

    private bool isSkillCheckActive;
    private bool isFading;
    private bool hasStageFailed;
    private bool hasInteractedOnce;

    private bool isVrSelected;
    private Transform activeInteractorTransform;
    private Vector3 previousControllerUp;
    private Vector3 previousControllerForward;
    private bool hasPreviousControllerPose;

    private bool isMouseDraggingObject;
    private float previousMouseAngle;
    private bool hasPreviousMouseAngle;

    private float accumulatedRotationDegrees;
    private float currentValveAngleDegrees;
    private float currentRedAngle;
    private float targetBlueZoneAngle;
    private float redMoveTimer;
    private float fadeTimer;

    private float ArcMin => Mathf.Min(arcStartAngle, arcEndAngle);
    private float ArcMax => Mathf.Max(arcStartAngle, arcEndAngle);
    private float ArcSpan => ArcMax - ArcMin;

    private void Reset()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        valveTarget = transform;
        audioSource = GetComponent<AudioSource>();
        localRotationAxis = Vector3.forward;
    }

    private void Awake()
    {
        if (interactable == null)
        {
            interactable = GetComponent<XRSimpleInteractable>();
        }

        if (valveTarget == null)
        {
            valveTarget = transform;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (stageFailPopupRoot != null)
        {
            stageFailPopupRoot.SetActive(false);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryStage);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        currentValveAngleDegrees = 0f;
        hasInteractedOnce = false;
        SetGuideVisible(true);
        HideSkillCheckImmediate();
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExited);
        }

        if (vrRotateHoldAction.action != null)
        {
            vrRotateHoldAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }

        if (vrRotateHoldAction.action != null)
        {
            vrRotateHoldAction.action.Disable();
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(RetryStage);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }

        Time.timeScale = 1f;
    }

    private void Update()
    {
        UpdateGuideVisual();
        UpdateSkillCheckWorldTransform();

        if (hasStageFailed)
        {
            return;
        }

        HandleVrInput();
        HandleMouseTestInput();

        if (isFading)
        {
            UpdateFadeOut();
            return;
        }

        if (isSkillCheckActive)
        {
            UpdateRedMarkerMotion();
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (hasStageFailed)
        {
            return;
        }

        MarkFirstInteraction();

        isVrSelected = true;
        activeInteractorTransform = args.interactorObject?.transform;
        hasPreviousControllerPose = false;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        isVrSelected = false;
        activeInteractorTransform = null;
        hasPreviousControllerPose = false;
    }

    private void HandleVrInput()
    {
        if (vrRotateHoldAction.action == null)
        {
            return;
        }

        if (isSkillCheckActive)
        {
            if (vrRotateHoldAction.action.WasPressedThisFrame())
            {
                ResolveSkillCheck();
            }

            return;
        }

        if (isFading || !isVrSelected || activeInteractorTransform == null)
        {
            hasPreviousControllerPose = false;
            return;
        }

        if (!vrRotateHoldAction.action.IsPressed())
        {
            hasPreviousControllerPose = false;
            return;
        }

        Vector3 currentUp = activeInteractorTransform.up;
        Vector3 currentForward = activeInteractorTransform.forward;

        if (!hasPreviousControllerPose)
        {
            previousControllerUp = currentUp;
            previousControllerForward = currentForward;
            hasPreviousControllerPose = true;
            return;
        }

        float signedRollDelta = Vector3.SignedAngle(previousControllerUp, currentUp, currentForward);
        float clockwiseDelta = invertVrClockwise ? -signedRollDelta : signedRollDelta;
        float appliedDelta = clockwiseDelta * vrRotationScale;

        if (Mathf.Abs(appliedDelta) > 0f)
        {
            RotateValveBy(appliedDelta);
        }

        previousControllerUp = currentUp;
        previousControllerForward = currentForward;
    }

    private void HandleMouseTestInput()
    {
        if (!enableMouseTestInEditor)
        {
            return;
        }

        Mouse mouse = Mouse.current;
        if (mouse == null)
        {
            return;
        }

        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return;
        }

        if (isSkillCheckActive)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                ResolveSkillCheck();
            }

            return;
        }

        if (isFading)
        {
            return;
        }

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                bool clickedThisObject = hit.transform == transform || hit.transform.IsChildOf(transform);

                if (clickedThisObject)
                {
                    MarkFirstInteraction();

                    isMouseDraggingObject = true;
                    previousMouseAngle = GetMouseAngleAroundValve(cam, mouse.position.ReadValue());
                    hasPreviousMouseAngle = true;
                }
            }
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            isMouseDraggingObject = false;
            hasPreviousMouseAngle = false;
        }

        if (!isMouseDraggingObject || !mouse.leftButton.isPressed)
        {
            return;
        }

        float currentMouseAngle = GetMouseAngleAroundValve(cam, mouse.position.ReadValue());

        if (!hasPreviousMouseAngle)
        {
            previousMouseAngle = currentMouseAngle;
            hasPreviousMouseAngle = true;
            return;
        }

        float signedDelta = Mathf.DeltaAngle(previousMouseAngle, currentMouseAngle);
        float clockwiseDelta = invertMouseClockwise ? signedDelta : -signedDelta;
        float appliedDelta = clockwiseDelta * mouseRotationScale;

        if (Mathf.Abs(appliedDelta) > 0f)
        {
            RotateValveBy(appliedDelta);
        }

        previousMouseAngle = currentMouseAngle;
    }

    private void MarkFirstInteraction()
    {
        if (hasInteractedOnce)
        {
            return;
        }

        hasInteractedOnce = true;
        SetGuideVisible(false);
    }

    private void SetGuideVisible(bool visible)
    {
        if (guideRoot != null)
        {
            guideRoot.SetActive(visible);
        }
    }

    private void UpdateGuideVisual()
    {
        if (guideRoot == null || hasInteractedOnce)
        {
            return;
        }

        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return;
        }

        const float baseHeight = 0.12f;
        const float forwardOffset = 0.02f;
        const float floatAmplitude = 0.015f;
        const float floatSpeed = 2f;

        float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        Vector3 basePosition =
            transform.position +
            Vector3.up * (baseHeight + floatOffset) +
            transform.forward * forwardOffset;

        guideRoot.transform.position = basePosition;

        Vector3 toCamera = guideRoot.transform.position - cam.transform.position;
        if (toCamera.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        guideRoot.transform.rotation = Quaternion.LookRotation(toCamera.normalized, Vector3.up);
    }

    private float GetMouseAngleAroundValve(Camera cam, Vector2 mouseScreenPosition)
    {
        Vector3 pivotWorldPosition = valveTarget != null ? valveTarget.position : transform.position;
        Vector3 pivotScreenPosition = cam.WorldToScreenPoint(pivotWorldPosition);
        Vector2 direction = mouseScreenPosition - new Vector2(pivotScreenPosition.x, pivotScreenPosition.y);
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private void RotateValveBy(float deltaDegrees)
    {
        float targetAngle = currentValveAngleDegrees + deltaDegrees;
        float clampedAngle = Mathf.Max(minValveAngleDegrees, targetAngle);
        float actualDelta = clampedAngle - currentValveAngleDegrees;

        if (Mathf.Approximately(actualDelta, 0f))
        {
            return;
        }

        valveTarget.Rotate(localRotationAxis.normalized, actualDelta, Space.Self);

        currentValveAngleDegrees = clampedAngle;
        accumulatedRotationDegrees += Mathf.Abs(actualDelta);

        if (Mathf.Abs(currentValveAngleDegrees) >= failValveAngleDegrees)
        {
            ShowStageFailPopup();
            return;
        }

        if (accumulatedRotationDegrees >= rotationToTriggerSkillCheckDegrees)
        {
            BeginSkillCheck();
        }
    }

    private void RotateValveLeftImmediate(float degrees)
    {
        float safeDegrees = Mathf.Min(degrees, currentValveAngleDegrees);
        if (safeDegrees <= 0f)
        {
            return;
        }

        valveTarget.Rotate(localRotationAxis.normalized, -safeDegrees, Space.Self);
        currentValveAngleDegrees -= safeDegrees;
    }

    private void BeginSkillCheck()
    {
        if (hasStageFailed)
        {
            return;
        }

        isSkillCheckActive = true;
        isFading = false;
        fadeTimer = 0f;
        redMoveTimer = 0f;

        isMouseDraggingObject = false;
        hasPreviousMouseAngle = false;
        hasPreviousControllerPose = false;

        if (skillCheckRoot != null)
        {
            skillCheckRoot.SetActive(true);
        }

        if (skillCheckCanvasGroup != null)
        {
            skillCheckCanvasGroup.alpha = 1f;
            skillCheckCanvasGroup.interactable = false;
            skillCheckCanvasGroup.blocksRaycasts = false;
        }

        targetBlueZoneAngle = Random.Range(
            ArcMin + blueZoneHalfWidthDegrees,
            ArcMax - blueZoneHalfWidthDegrees
        );

        currentRedAngle = ArcMin;

        UpdateBlueZoneVisual();
        UpdateRedMarkerVisual();
    }

    private void ResolveSkillCheck()
    {
        bool isSuccess =
            Mathf.Abs(Mathf.DeltaAngle(currentRedAngle, targetBlueZoneAngle)) <= blueZoneHalfWidthDegrees;

        if (isSuccess)
        {
            RotateValveLeftImmediate(successRotateLeftDegrees);
        }

        PlayResultSound();
        PlayRandomRadioFeedback();
        accumulatedRotationDegrees = 0f;
        StartFadeOut();
    }

    private void PlayRandomRadioFeedback()
    {
        if (radioFeedback == null)
        {
            return;
        }

        int index = Random.Range(0, 3);
        AudioClip selectedClip;
        string selectedSubtitle;

        switch (index)
        {
            case 0:
                selectedClip = room1Voice1;
                selectedSubtitle = room1Subtitle1;
                break;
            case 1:
                selectedClip = room1Voice2;
                selectedSubtitle = room1Subtitle2;
                break;
            default:
                selectedClip = room1Voice3;
                selectedSubtitle = room1Subtitle3;
                break;
        }

        radioFeedback.PlayRadio(selectedClip, selectedSubtitle);
    }

    private void StartFadeOut()
    {
        isSkillCheckActive = false;
        isFading = true;
        fadeTimer = 0f;
    }

    private void UpdateFadeOut()
    {
        if (skillCheckCanvasGroup == null)
        {
            HideSkillCheckImmediate();
            isFading = false;
            return;
        }

        fadeTimer += Time.deltaTime;
        float t = Mathf.Clamp01(fadeTimer / Mathf.Max(0.0001f, fadeOutSeconds));
        skillCheckCanvasGroup.alpha = 1f - t;

        if (t >= 1f)
        {
            HideSkillCheckImmediate();
            isFading = false;
        }
    }

    private void HideSkillCheckImmediate()
    {
        if (skillCheckCanvasGroup != null)
        {
            skillCheckCanvasGroup.alpha = 0f;
            skillCheckCanvasGroup.interactable = false;
            skillCheckCanvasGroup.blocksRaycasts = false;
        }

        if (skillCheckRoot != null)
        {
            skillCheckRoot.SetActive(false);
        }

        isSkillCheckActive = false;
        isFading = false;
    }

    private void UpdateRedMarkerMotion()
    {
        redMoveTimer += Time.deltaTime * redMarkerMoveDegreesPerSecond;
        float travel = Mathf.PingPong(redMoveTimer, ArcSpan);
        currentRedAngle = ArcMin + travel;
        UpdateRedMarkerVisual();
    }

    private void UpdateRedMarkerVisual()
    {
        if (redMarkerImage == null)
        {
            return;
        }

        redMarkerImage.anchoredPosition = GetArcPosition(currentRedAngle);
        redMarkerImage.localRotation = Quaternion.Euler(0f, 0f, currentRedAngle - 90f);
    }

    private void UpdateBlueZoneVisual()
    {
        if (blueZoneImage == null)
        {
            return;
        }

        blueZoneImage.anchoredPosition = GetArcPosition(targetBlueZoneAngle);
        blueZoneImage.localRotation = Quaternion.Euler(0f, 0f, targetBlueZoneAngle - 90f);
    }

    private Vector2 GetArcPosition(float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians) * arcRadius;
        float y = Mathf.Sin(radians) * arcRadius;
        return new Vector2(x, y);
    }

    private void UpdateSkillCheckWorldTransform()
    {
        if (skillCheckRoot == null)
        {
            return;
        }

        skillCheckRoot.transform.position = transform.position + uiWorldOffset;
    }

    private void PlayResultSound()
    {
        if (audioSource == null || resultClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(resultClip);
    }

    private void ShowStageFailPopup()
    {
        if (hasStageFailed)
        {
            return;
        }

        hasStageFailed = true;
        isSkillCheckActive = false;
        isFading = false;
        isVrSelected = false;
        activeInteractorTransform = null;
        isMouseDraggingObject = false;
        hasPreviousMouseAngle = false;
        hasPreviousControllerPose = false;

        HideSkillCheckImmediate();
        SetGuideVisible(false);

        if (stageFailPopupRoot != null)
        {
            stageFailPopupRoot.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void RetryStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}