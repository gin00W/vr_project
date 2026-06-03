using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class ExclamationInteractable : MonoBehaviour
{
    [Header("References")]
    public Mark miniGameStarter;
    public Renderer targetRenderer;
    public Transform visualRoot;

    [Header("Hover Effect")]
    public float hoverScaleMultiplier = 1.15f;

    [ColorUsage(true, true)]
    public Color normalEmission = Color.black;

    [ColorUsage(true, true)]
    public Color hoverEmission = Color.white * 2f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    private Material runtimeMaterial;
    private Vector3 startScale;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        if (visualRoot == null)
            visualRoot = transform;

        startScale = visualRoot.localScale;

        if (targetRenderer != null)
        {
            runtimeMaterial = targetRenderer.material;

            if (runtimeMaterial.HasProperty("_EmissionColor"))
                runtimeMaterial.EnableKeyword("_EMISSION");
        }

        SetHoverVisual(false);
    }

    private void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        SetHoverVisual(true);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        SetHoverVisual(false);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        SetHoverVisual(false);

        if (miniGameStarter != null)
            miniGameStarter.ShowIntroPanel();
    }

    private void SetHoverVisual(bool isHovering)
    {
        if (visualRoot != null)
        {
            visualRoot.localScale = isHovering
                ? startScale * hoverScaleMultiplier
                : startScale;
        }

        if (runtimeMaterial != null && runtimeMaterial.HasProperty("_EmissionColor"))
        {
            runtimeMaterial.SetColor(
                "_EmissionColor",
                isHovering ? hoverEmission : normalEmission
            );
        }
    }
}