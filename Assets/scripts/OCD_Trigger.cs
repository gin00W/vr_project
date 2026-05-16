using UnityEngine;

public class OCD_Trigger : MonoBehaviour
{
    public GameObject ocdPanel;
    public string message = "불편하지만 위험하지 않아요";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera") || other.CompareTag("Player"))
        {
            if (ocdPanel != null)
            {
                ocdPanel.SetActive(true);
                var text = ocdPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                    text.text = message;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera") || other.CompareTag("Player"))
        {
            if (ocdPanel != null)
                ocdPanel.SetActive(false);
        }
    }
}