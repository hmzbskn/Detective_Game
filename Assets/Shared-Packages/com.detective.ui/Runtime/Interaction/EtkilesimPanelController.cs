using UnityEngine;
using TMPro;

public class EtkilesimPanelController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI objeAdiText;
    [SerializeField] private TextMeshProUGUI etkilesimText;

    private void Awake()
    {
        Kapat();
    }

    public void Goster(string objeAdi, string etkilesimMetni)
    {
        if (objeAdiText != null)
            objeAdiText.text = objeAdi;

        if (etkilesimText != null)
            etkilesimText.text = "[E] " + etkilesimMetni;

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    public void Kapat()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }
}