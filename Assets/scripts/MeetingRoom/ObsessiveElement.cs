using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ObsessiveElement : MonoBehaviour
{
    [Header("--- 설정 ---")]
    public string elementName = "강박 요소";

    [Header("--- 반짝임 ---")]
    public float blinkSpeed = 3f;
    public Color normalColor = Color.white;
    public Color alertColor = new Color(1f, 0.3f, 0.3f);

    private Renderer rend;
    private Material mat;
    private XRSimpleInteractable interactable;
    private bool isObsessing = false;

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            mat = rend.material;

        SetupInteractable();

        // 시작 시 깜빡임
        SetObsessing(true);
    }

    void SetupInteractable()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();

        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        interactable.selectEntered.AddListener(OnTouched);
    }

    public void SetObsessing(bool active)
    {
        isObsessing = active;
        StopAllCoroutines();

        if (active)
            StartCoroutine(BlinkLoop());
        else
            ApplyColor(normalColor);
    }

    IEnumerator BlinkLoop()
    {
        while (isObsessing)
        {
            float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f;
            ApplyColor(Color.Lerp(normalColor, alertColor, t));
            yield return null;
        }
    }

    void ApplyColor(Color color)
    {
        if (mat == null) return;

        if (mat.HasProperty(BaseColorID))
            mat.SetColor(BaseColorID, color);
        else if (mat.HasProperty(ColorID))
            mat.SetColor(ColorID, color);
    }

    // VR 컨트롤러로 잡았을 때 (강박 행동 = 참기)
    void OnTouched(SelectEnterEventArgs args)
    {
        DataCollector.Instance?.LogObsessionTouch();
        MeetingRoomManager.Instance?.AddResist();

        var baseInteractor = args.interactorObject as XRBaseInteractor;
        if (baseInteractor != null)
        {
            var controller = baseInteractor
                .GetComponentInParent<ActionBasedController>();
            controller?.SendHapticImpulse(0.4f, 0.15f);
        }
    }
}