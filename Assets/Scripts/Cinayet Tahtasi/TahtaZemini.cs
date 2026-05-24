using UnityEngine;
using UnityEngine.EventSystems;

public class TahtaZemini : MonoBehaviour, IDropHandler
{
    [Header("Ayarlar")]
    [SerializeField] private GameObject delilUIPrefab;

    [Header("Delil Görsel Boyutu")]
    [SerializeField] private Vector2 delilKartBoyutu = new Vector2(220f, 250f);

    private RectTransform tahtaRect;

    private void Awake()
    {
        tahtaRect = GetComponent<RectTransform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        EnvanterSlot gelenSlot = eventData.pointerDrag != null
            ? eventData.pointerDrag.GetComponentInParent<EnvanterSlot>()
            : null;

        if (gelenSlot == null)
            return;

        SlotuTahtayaAs(gelenSlot, eventData.position, eventData.pressEventCamera);
    }

    public bool SlotuTahtayaAs(EnvanterSlot gelenSlot, Vector2 ekranPozisyonu, Camera eventCamera)
    {
        if (gelenSlot == null)
            return false;

        if (gelenSlot.BosMu())
            return false;

        EsyaVerisi gelenEsya = gelenSlot.EsyaGetir();

        if (gelenEsya == null)
            return false;

        if (!gelenEsya.delilMi)
        {
            Debug.LogWarning("Bu eşya tahtaya asılamaz. Sadece delilMi işaretli eşyalar asılabilir.");
            return false;
        }

        bool olustu = DelilGorseliOlustur(
            gelenEsya.esyaIkonu,
            ekranPozisyonu,
            eventCamera
        );

        if (!olustu)
            return false;

        gelenSlot.SlotuBosalt();

        EsyaKusanma oyuncu = FindFirstObjectByType<EsyaKusanma>();

        if (oyuncu != null)
            oyuncu.EldekiNesneyiYenile();

        Debug.Log(gelenEsya.esyaAdi + " tahtaya başarıyla asıldı.");

        return true;
    }

    public bool HotbarIkonunuTahtayaAs(Sprite ikon, Vector2 ekranPozisyonu, Camera eventCamera)
    {
        if (ikon == null)
        {
            Debug.LogWarning("Hotbar ikon verisi boş. Tahtaya delil görseli oluşturulamadı.");
            return false;
        }

        bool olustu = DelilGorseliOlustur(
            ikon,
            ekranPozisyonu,
            eventCamera
        );

        if (olustu)
            Debug.Log("Hotbar itemi tahtaya referans olarak asıldı.");

        return olustu;
    }

    private bool DelilGorseliOlustur(Sprite delilIkonu, Vector2 ekranPozisyonu, Camera eventCamera)
    {
        if (delilUIPrefab == null)
        {
            Debug.LogError("TahtaZemini üzerinde Delil UI Prefab atanmamış.");
            return false;
        }

        GameObject yeniDelil = Instantiate(delilUIPrefab, transform);

        RectTransform yeniDelilRect = yeniDelil.GetComponent<RectTransform>();

        if (yeniDelilRect != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tahtaRect,
                ekranPozisyonu,
                eventCamera,
                out Vector2 localPoint
            );

            yeniDelilRect.anchoredPosition = localPoint;
            yeniDelilRect.sizeDelta = delilKartBoyutu;
        }

        TahtaDelili tahtaDelili = yeniDelil.GetComponent<TahtaDelili>();

        if (tahtaDelili != null)
        {
            tahtaDelili.IcerigiAyarla(delilIkonu, "");
        }
        else
        {
            Debug.LogWarning("Delil prefabı üzerinde TahtaDelili scripti yok.");
        }

        return true;
    }
}