using UnityEngine;
using TMPro;

public class BilgisayarPenceresi : MonoBehaviour
{
    public enum PencereDurumu
    {
        Kapali,
        Acik,
        Kucultulmus
    }

    [Header("Referanslar")]
    [SerializeField] private RectTransform pencereRect;
    [SerializeField] private TaskbarYoneticisi taskbarYoneticisi;
    [SerializeField] private TMP_Text pencereBaslikText;

    [Header("Pencere Bilgisi")]
    [SerializeField] private string pencereAdi = "Uygulama";

    public PencereDurumu Durum { get; private set; } = PencereDurumu.Kapali;
    public string PencereAdi => pencereAdi;

    private void Awake()
    {
        if (pencereRect == null)
            pencereRect = GetComponent<RectTransform>();

        BasligiGuncelle();
    }

    private void OnValidate()
    {
        BasligiGuncelle();
    }

    private void BasligiGuncelle()
    {
        if (pencereBaslikText != null)
            pencereBaslikText.text = pencereAdi;
    }

    public void Ac()
    {
        gameObject.SetActive(true);
        TamEkranYerlestir();
        ONeGetir();

        if (Durum == PencereDurumu.Kapali)
            taskbarYoneticisi?.PencereAcildi(this);

        Durum = PencereDurumu.Acik;
    }

    public void Kapat()
    {
        gameObject.SetActive(false);
        Durum = PencereDurumu.Kapali;
        taskbarYoneticisi?.PencereKapandi(this);
    }

    public void Kucult()
    {
        gameObject.SetActive(false);
        Durum = PencereDurumu.Kucultulmus;
    }

    public void ONeGetir()
    {
        transform.SetAsLastSibling();
    }

    private void TamEkranYerlestir()
    {
        if (pencereRect == null)
            return;

        pencereRect.anchorMin = Vector2.zero;
        pencereRect.anchorMax = Vector2.one;
        pencereRect.pivot = new Vector2(0.5f, 0.5f);

        pencereRect.offsetMin = Vector2.zero;
        pencereRect.offsetMax = Vector2.zero;
        pencereRect.anchoredPosition = Vector2.zero;
        pencereRect.localScale = Vector3.one;
    }
}