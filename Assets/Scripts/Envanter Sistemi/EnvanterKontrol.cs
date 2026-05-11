using UnityEngine;

public class EnvanterKontrol : MonoBehaviour
{
    [Header("Hotbar Hareket Ayarları")]
    public RectTransform hotbarRect;
    public float oyunIciYPozisyonu = 30f;
    public float cantaAcikkenYPozisyonu = 350f;

    [Header("UI Panelleri")]
    public GameObject genelEnvanterPanel;
    public GameObject hotbarPanel;

    [Header("Slot Listeleri")]
    public EnvanterSlot[] hotbarSlotlari;
    public EnvanterSlot[] genelEnvanterSlotlari;

    private int seciliHotbarIndex = -1;

    // YENİ: Tahtada mıyız diye kontrol edeceğimiz anahtar!
    [HideInInspector] public bool tahtaModundaMi = false;

    void Start()
    {
        if (genelEnvanterPanel != null) genelEnvanterPanel.SetActive(false);
        if (hotbarPanel != null) hotbarPanel.SetActive(true);

        HotbarSec(0);
    }

    void Update()
    {
        // 1. DURUM: EĞER TAHTA AÇIKSA
        if (tahtaModundaMi)
        {
            // Eğer çanta arka planda bug'da kalıp açıldıysa zorla kapat!
            if (genelEnvanterPanel != null && genelEnvanterPanel.activeSelf)
            {
                genelEnvanterPanel.SetActive(false);
            }

            // SADECE HOTBARI AÇ/KAPAT (Basılı tutma yok, tek tıkla Toggle)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (hotbarPanel != null)
                {
                    bool yeniDurum = !hotbarPanel.activeSelf;
                    hotbarPanel.SetActive(yeniDurum);

                    // ÇÖZÜM: Hotbar açıldığında hiyerarşide en alta (yani ekranda EN ÖNE) at!
                    if (yeniDurum)
                    {
                        hotbarPanel.transform.SetAsLastSibling();
                    }
                }
            }
        }
        // 2. DURUM: TAHTA KAPALIYSA (NORMAL OYUN MODU)
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab)) EnvanterAc();
            else if (Input.GetKeyUp(KeyCode.Tab)) EnvanterKapat();
        }

        // Sayı tuşları hep çalışsın
        if (Input.GetKeyDown(KeyCode.Alpha1)) HotbarSec(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) HotbarSec(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) HotbarSec(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) HotbarSec(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) HotbarSec(4);
    }

    private void HotbarSec(int index)
    {
        if (hotbarSlotlari == null || index >= hotbarSlotlari.Length) return;

        for (int i = 0; i < hotbarSlotlari.Length; i++)
        {
            if (hotbarSlotlari[i] != null) hotbarSlotlari[i].SecimiGuncelle(false);
        }

        if (hotbarSlotlari[index] != null)
        {
            hotbarSlotlari[index].SecimiGuncelle(true);
            seciliHotbarIndex = index;

            EsyaKusanma oyuncu = FindFirstObjectByType<EsyaKusanma>();
            if (oyuncu != null)
            {
                EsyaVerisi slotTakiEsya = hotbarSlotlari[index].EsyaGetir();
                if (slotTakiEsya != null) oyuncu.EsyaKusan(slotTakiEsya, hotbarSlotlari[index]);
                else oyuncu.ElindekiniTemizle();
            }
        }
    }

    public bool EsyaEkle(EsyaVerisi eklenecekEsya)
    {
        if (seciliHotbarIndex >= 0 && seciliHotbarIndex < hotbarSlotlari.Length)
        {
            if (hotbarSlotlari[seciliHotbarIndex].BosMu())
            {
                hotbarSlotlari[seciliHotbarIndex].SlotuDoldur(eklenecekEsya);
                HotbarSec(seciliHotbarIndex);
                return true;
            }
        }

        for (int i = 0; i < hotbarSlotlari.Length; i++)
        {
            if (hotbarSlotlari[i].BosMu())
            {
                hotbarSlotlari[i].SlotuDoldur(eklenecekEsya);
                return true;
            }
        }

        for (int i = 0; i < genelEnvanterSlotlari.Length; i++)
        {
            if (genelEnvanterSlotlari[i].BosMu())
            {
                genelEnvanterSlotlari[i].SlotuDoldur(eklenecekEsya);
                return true;
            }
        }
        return false;
    }

    private void EnvanterAc()
    {
        genelEnvanterPanel.SetActive(true);
        if (hotbarRect != null) hotbarRect.anchoredPosition = new Vector2(hotbarRect.anchoredPosition.x, cantaAcikkenYPozisyonu);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void EnvanterKapat()
    {
        genelEnvanterPanel.SetActive(false);
        if (hotbarRect != null) hotbarRect.anchoredPosition = new Vector2(hotbarRect.anchoredPosition.x, oyunIciYPozisyonu);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}