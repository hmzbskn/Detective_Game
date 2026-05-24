using UnityEngine;
using TMPro;

public class EtkilesimPanelController : MonoBehaviour
{
    public static EtkilesimPanelController Instance { get; private set; }

    private static int panelKilitSayisi = 0;
    public static bool PanelKilitliMi => panelKilitSayisi > 0;

    [Header("Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI objeAdiText;
    [SerializeField] private TextMeshProUGUI etkilesimText;

    private void Awake()
    {
        Instance = this;
        Kapat();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Goster(string objeAdi, string etkilesimMetni)
    {
        if (PanelKilitliMi)
        {
            Kapat();
            return;
        }

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

    public static void PaneliKilitle()
    {
        panelKilitSayisi++;

        EtkilesimPanelController controller = Instance;

        if (controller == null)
            controller = FindFirstObjectByType<EtkilesimPanelController>();

        if (controller != null)
            controller.Kapat();
    }

    public static void PanelKilitiniKaldir()
    {
        panelKilitSayisi--;

        if (panelKilitSayisi < 0)
            panelKilitSayisi = 0;
    }

    public static void PanelKilidiniSifirla()
    {
        panelKilitSayisi = 0;

        EtkilesimPanelController controller = Instance;

        if (controller == null)
            controller = FindFirstObjectByType<EtkilesimPanelController>();

        if (controller != null)
            controller.Kapat();
    }
}