using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class ObsessiveElement : MonoBehaviour
{
    [Header("--- ���� ---")]
    public string elementName = "�߶Ծ��� ����";

    [Header("--- ��¦�� ---")]
    public float blinkSpeed = 3f;
    public Color normalColor = Color.white;
    public Color alertColor = new Color(1f, 0.3f, 0.3f);

    private Renderer rend;
    private Material mat;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    private bool isObsessing = false;

    // URP / Built-in ����
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
            mat = rend.material; // �ν��Ͻ� ����

        SetupInteractable();
    }

    void SetupInteractable()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        interactable.selectEntered.AddListener(OnTouched);
    }

    // MiniGameController���� ȣ��
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

    void OnTouched(SelectEnterEventArgs args)
    {
        // ���� ��� ��ġ = ���� ī��Ʈ
        MeetingRoomManager.Instance?.AddResist();

        // ��Ʈ�ѷ� ����
        var baseInteractor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;
        if (baseInteractor != null)
        {
            var controller = baseInteractor
                .GetComponentInParent<ActionBasedController>();
            controller?.SendHapticImpulse(0.4f, 0.15f);
        }
    }
}