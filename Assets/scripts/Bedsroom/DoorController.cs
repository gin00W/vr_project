using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Renderer doorRenderer;
    public GameObject clearEffect;

    [ColorUsage(true, true)]
    public Color offEmission = Color.black;

    [ColorUsage(true, true)]
    public Color onEmission = Color.white * 2f;

    private Material doorMaterial;

    private void Awake()
    {
        if (doorRenderer != null)
        {
            doorMaterial = doorRenderer.material;
            doorMaterial.EnableKeyword("_EMISSION");
        }

        SetDoorState(false);
    }

    public void SetDoorState(bool isOpen)
    {
        if (doorMaterial != null)
            doorMaterial.SetColor("_EmissionColor", isOpen ? onEmission : offEmission);

        if (clearEffect != null)
            clearEffect.SetActive(isOpen);
    }
}