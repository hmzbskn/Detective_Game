using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EnvanterSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI Bileşenleri")]
    public Image ikon;
    public TextMeshProUGUI isimYazisi;

    [Header("Hotbar Görsel Ayarları")]
    public Image cerceveImaj;
    public Color seciliRenk = new Color(0.6f, 0.3f, 0f);
    public Color normalRenk = Color.white;

    // YENİ: Bu slotun şu an aktif/seçili olup olmadığını aklında tutacak
    public bool isSecili = false;

    private EsyaVerisi icindekiEsya;
    private Sprite varsayilanSprite;
    private Color varsayilanRenk;
    private bool baslangicAyariYapildiMi = false;

    private static GameObject hayaletIkonObjesi;
    private static Image hayaletIkon;

    void Start()
    {
        BaslangicAyariniYap();
    }

    private void BaslangicAyariniYap()
    {
        if (baslangicAyariYapildiMi) return;
        if (ikon != null)
        {
            varsayilanSprite = ikon.sprite;
            varsayilanRenk = ikon.color;
            baslangicAyariYapildiMi = true;
        }
    }

    public bool BosMu() { return icindekiEsya == null; }
    public EsyaVerisi EsyaGetir() { return icindekiEsya; }

    public void SlotuDoldur(EsyaVerisi yeniEsya)
    {
        BaslangicAyariniYap();
        icindekiEsya = yeniEsya;

        if (ikon != null)
        {
            if (yeniEsya != null && yeniEsya.esyaIkonu != null)
            {
                ikon.sprite = yeniEsya.esyaIkonu;
                ikon.color = Color.white;
            }
            else { SlotuBosalt(); }
        }
        if (isimYazisi != null)
            isimYazisi.text = (yeniEsya != null) ? "" : "";
    }

    public void SlotuBosalt()
    {
        icindekiEsya = null;
        if (ikon != null)
        {
            ikon.sprite = varsayilanSprite;
            ikon.color = varsayilanRenk;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (BosMu() || eventData.button != PointerEventData.InputButton.Left || ikon == null) return;

        Canvas anaCanvas = GetComponentInParent<Canvas>();
        if (anaCanvas == null) return;

        if (hayaletIkonObjesi != null) Destroy(hayaletIkonObjesi);

        hayaletIkonObjesi = new GameObject("HayaletIkon");
        hayaletIkonObjesi.transform.SetParent(anaCanvas.transform);
        hayaletIkonObjesi.transform.SetAsLastSibling();

        hayaletIkon = hayaletIkonObjesi.AddComponent<Image>();
        hayaletIkon.sprite = ikon.sprite;
        hayaletIkon.color = new Color(1, 1, 1, 0.7f);
        hayaletIkon.raycastTarget = false;

        RectTransform rect = hayaletIkonObjesi.GetComponent<RectTransform>();
        rect.sizeDelta = ikon.rectTransform.sizeDelta;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (hayaletIkonObjesi != null) hayaletIkonObjesi.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (hayaletIkonObjesi != null) { Destroy(hayaletIkonObjesi); hayaletIkonObjesi = null; }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (hayaletIkonObjesi != null) { Destroy(hayaletIkonObjesi); hayaletIkonObjesi = null; }

        EnvanterSlot gelenSlot = eventData.pointerDrag?.GetComponent<EnvanterSlot>();

        if (gelenSlot != null && gelenSlot != this)
        {
            EsyaVerisi benimEskiEsyam = icindekiEsya;
            EsyaVerisi gelenEsya = gelenSlot.EsyaGetir();

            SlotuDoldur(gelenEsya);
            gelenSlot.SlotuDoldur(benimEskiEsyam);

            EsyaKusanma kusanmaSistemi = FindObjectOfType<EsyaKusanma>();
            if (kusanmaSistemi != null)
            {
                // YENİ MANTIK: Eğer üstüne eşya bıraktığımız bu hedef slot o an "seçili" olan slotsa, direkt eline ver
                if (this.isSecili)
                {
                    kusanmaSistemi.EsyaKusan(this.EsyaGetir(), this);
                }
                // Veya eşyayı söküp aldığımız kaynak slot "seçili" olan slotsa, onun da elini güncelle
                else if (gelenSlot.isSecili)
                {
                    kusanmaSistemi.EsyaKusan(gelenSlot.EsyaGetir(), gelenSlot);
                }
                else
                {
                    // İkisi de seçili değilse (örneğin çantanın içinde iki eşya yer değiştirdiyse) normal yenileme yap
                    kusanmaSistemi.EldekiNesneyiYenile();
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (BosMu() || eventData.button != PointerEventData.InputButton.Left) return;
        EsyaKusanma oyuncu = FindObjectOfType<EsyaKusanma>();
        if (oyuncu != null) oyuncu.EsyaKusan(icindekiEsya, this);
    }

    public void SecimiGuncelle(bool seciliMi)
    {
        isSecili = seciliMi; // YENİ: Durumu değişkene kaydet ki OnDrop kısmında kullanabilelim
        if (cerceveImaj != null) cerceveImaj.color = seciliMi ? seciliRenk : normalRenk;
    }
}