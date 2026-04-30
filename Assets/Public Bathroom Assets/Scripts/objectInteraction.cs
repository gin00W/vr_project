using UnityEngine;
using System.Linq;
using System.Collections;


public class objectInteraction : MonoBehaviour {

    public AudioSource soundEffect;
    public Animator animatorObject;
    //public Material material;
    private Material[] materials;
    public Color highlightColor;

    public string animationName = "Interaction Animation";
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    // Use this for initialization
    void Start () {

        materials = GetComponentsInChildren<Renderer>().SelectMany(r => r.materials).ToArray();
        foreach (var tempMaterial in materials)
        tempMaterial.SetColor("_EmissionColor", Color.black);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseUp()
    {
        if(animatorObject != null  &&  (soundEffect == null || !soundEffect.isPlaying))
        {
            animatorObject.Play(animationName);
        }
        if (soundEffect != null && !soundEffect.isPlaying)
        {
            soundEffect.pitch = Random.Range(minPitch, maxPitch);
            soundEffect.Play();

        }
    }

    void OnMouseEnter()
    {
        foreach(var tempMaterial in materials)
        tempMaterial.SetColor("_EmissionColor", highlightColor);
    }

    void OnMouseExit()
    {
        foreach (var tempMaterial in materials)
            tempMaterial.SetColor("_EmissionColor", Color.black);
    }
}
