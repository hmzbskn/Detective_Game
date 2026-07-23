using System.Collections.Generic;
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

    public bool isSecili = false;

    private EsyaVerisi icindekiEsya;
    private Sprite varsayilanSprite;
    private Color varsayilanRenk;
    private bool baslangicAyariYapildiMi = false;

    private static GameObject hayaletIkonObjesi;
    private static Image hayaletIkon;

    private void Start()
    {
        BaslangicAyariniYap();
    }

    private void BaslangicAyariniYap()
    {
        if (baslangicAyariYapildiMi)
            return;

        if (ikon != null)
        {
            varsayilanSprite = ikon.sprite;
            varsayilanRenk = ikon.color;
            baslangicAyariYapildiMi = true;
        }
    }

    public bool BosMu()
    {
        return icindekiEsya == null;
    }

    public EsyaVerisi EsyaGetir()
    {
        return icindekiEsya;
    }

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
            else
            {
                SlotuBosalt();
            }
        }

        if (isimYazisi != null)
            isimYazisi.text = "";
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
        if (BosMu())
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (ikon == null)
            return;

        Canvas anaCanvas = GetComponentInParent<Canvas>();

        if (anaCanvas == null)
            return;

        if (hayaletIkonObjesi != null)
            Destroy(hayaletIkonObjesi);

        hayaletIkonObjesi = new GameObject("HayaletIkon");
        hayaletIkonObjesi.transform.SetParent(anaCanvas.transform, false);
        hayaletIkonObjesi.transform.SetAsLastSibling();

        hayaletIkon = hayaletIkonObjesi.AddComponent<Image>();
        hayaletIkon.sprite = ikon.sprite;
        hayaletIkon.color = new Color(1f, 1f, 1f, 0.7f);
        hayaletIkon.raycastTarget = false;
        hayaletIkon.preserveAspect = true;

        RectTransform rect = hayaletIkonObjesi.GetComponent<RectTransform>();
        rect.sizeDelta = ikon.rectTransform.sizeDelta;

        rect.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (hayaletIkonObjesi == null)
            return;

        hayaletIkonObjesi.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (hayaletIkonObjesi != null)
        {
            Destroy(hayaletIkonObjesi);
            hayaletIkonObjesi = null;
        }

    }

    public void OnDrop(PointerEventData eventData)
    {
        if (hayaletIkonObjesi != null)
        {
            Destroy(hayaletIkonObjesi);
            hayaletIkonObjesi = null;
        }

        EnvanterSlot gelenSlot = eventData.pointerDrag != null
            ? eventData.pointerDrag.GetComponent<EnvanterSlot>()
            : null;

        if (gelenSlot == null)
            return;

        if (gelenSlot == this)
            return;

        EsyaVerisi benimEskiEsyam = icindekiEsya;
        EsyaVerisi gelenEsya = gelenSlot.EsyaGetir();

        SlotuDoldur(gelenEsya);
        gelenSlot.SlotuDoldur(benimEskiEsyam);

        EsyaKusanma kusanmaSistemi = FindFirstObjectByType<EsyaKusanma>();

        if (kusanmaSistemi != null)
        {
            if (this.isSecili)
            {
                kusanmaSistemi.EsyaKusan(this.EsyaGetir(), this);
            }
            else if (gelenSlot.isSecili)
            {
                kusanmaSistemi.EsyaKusan(gelenSlot.EsyaGetir(), gelenSlot);
            }
            else
            {
                kusanmaSistemi.EldekiNesneyiYenile();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (BosMu())
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        EsyaKusanma oyuncu = FindFirstObjectByType<EsyaKusanma>();

        if (oyuncu != null)
            oyuncu.EsyaKusan(icindekiEsya, this);
    }

    public void SecimiGuncelle(bool seciliMi)
    {
        isSecili = seciliMi;

        if (cerceveImaj != null)
            cerceveImaj.color = seciliMi ? seciliRenk : normalRenk;
    }

}