using UnityEngine;

public class TahtaZemini : MonoBehaviour, IDelilBirakmaHedefi
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

    public bool DeliliBirak(ItemInstanceData delil, Sprite ikon, Vector2 ekranPozisyonu, Camera eventCamera)
    {
        if (delil == null || delil.ItemData == null)
            return false;

        if (!delil.ItemData.DelilMi)
        {
            Debug.LogWarning("Bu eşya tahtaya asılamaz. Sadece DelilMi işaretli eşyalar asılabilir.");
            return false;
        }

        Sprite gosterilecekIkon = ikon != null ? ikon : delil.IkonGetir();

        bool olustu = DelilGorseliOlustur(delil, gosterilecekIkon, ekranPozisyonu, eventCamera);

        if (olustu)
            Debug.Log(delil.ItemData.ItemAdi + " tahtaya başarıyla asıldı.");

        return olustu;
    }

    private bool DelilGorseliOlustur(ItemInstanceData delil, Sprite delilIkonu, Vector2 ekranPozisyonu, Camera eventCamera)
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
            tahtaDelili.DelilVerisiniAta(delil);
            tahtaDelili.IcerigiAyarla(delilIkonu, "");
        }
        else
        {
            Debug.LogWarning("Delil prefabı üzerinde TahtaDelili scripti yok.");
        }

        return true;
    }
}