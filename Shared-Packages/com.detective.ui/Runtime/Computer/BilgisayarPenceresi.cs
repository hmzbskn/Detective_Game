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

    [Header("Varsayılan Pencere Boyutu")]
    [Tooltip("Pencere ilk açıldığında ortalanıp bu boyuta getirilir. Sonraki açılışlarda oyuncunun sürükleyip boyutlandırdığı konum/boyut korunur.")]
    [SerializeField] private Vector2 varsayilanBoyut = new Vector2(900, 600);

    public PencereDurumu Durum { get; private set; } = PencereDurumu.Kapali;
    public string PencereAdi => pencereAdi;

    private bool ilkKonumAyarlandi;

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

        if (!ilkKonumAyarlandi)
        {
            VarsayilanKonumaYerlestir();
            ilkKonumAyarlandi = true;
        }

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

    private void VarsayilanKonumaYerlestir()
    {
        if (pencereRect == null)
            return;

        pencereRect.anchorMin = new Vector2(0.5f, 0.5f);
        pencereRect.anchorMax = new Vector2(0.5f, 0.5f);
        pencereRect.pivot = new Vector2(0.5f, 0.5f);

        pencereRect.sizeDelta = varsayilanBoyut;
        pencereRect.anchoredPosition = Vector2.zero;
        pencereRect.localScale = Vector3.one;
    }
}