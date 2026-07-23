using UnityEngine;
using UnityEngine.InputSystem;

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

    [HideInInspector] public bool tahtaModundaMi = false;

    private void Start()
    {
        if (genelEnvanterPanel != null)
            genelEnvanterPanel.SetActive(false);

        if (hotbarPanel != null)
            hotbarPanel.SetActive(true);

        HotbarSec(0);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (tahtaModundaMi)
        {
            if (genelEnvanterPanel != null && genelEnvanterPanel.activeSelf)
            {
                genelEnvanterPanel.SetActive(false);
            }

            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if (hotbarPanel != null)
                {
                    bool yeniDurum = !hotbarPanel.activeSelf;
                    hotbarPanel.SetActive(yeniDurum);

                    if (yeniDurum)
                    {
                        hotbarPanel.transform.SetAsLastSibling();
                    }
                }
            }
        }
        else
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                EnvanterAc();
            }
            else if (Keyboard.current.tabKey.wasReleasedThisFrame)
            {
                EnvanterKapat();
            }
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
            HotbarSec(0);

        if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
            HotbarSec(1);

        if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
            HotbarSec(2);

        if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.numpad4Key.wasPressedThisFrame)
            HotbarSec(3);

        if (Keyboard.current.digit5Key.wasPressedThisFrame || Keyboard.current.numpad5Key.wasPressedThisFrame)
            HotbarSec(4);
    }

    private void HotbarSec(int index)
    {
        if (hotbarSlotlari == null || index >= hotbarSlotlari.Length)
            return;

        for (int i = 0; i < hotbarSlotlari.Length; i++)
        {
            if (hotbarSlotlari[i] != null)
                hotbarSlotlari[i].SecimiGuncelle(false);
        }

        if (hotbarSlotlari[index] != null)
        {
            hotbarSlotlari[index].SecimiGuncelle(true);
            seciliHotbarIndex = index;

            EsyaKusanma oyuncu = FindFirstObjectByType<EsyaKusanma>();

            if (oyuncu != null)
            {
                EsyaVerisi slotTakiEsya = hotbarSlotlari[index].EsyaGetir();

                if (slotTakiEsya != null)
                    oyuncu.EsyaKusan(slotTakiEsya, hotbarSlotlari[index]);
                else
                    oyuncu.ElindekiniTemizle();
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
        if (genelEnvanterPanel != null)
            genelEnvanterPanel.SetActive(true);

        if (hotbarRect != null)
        {
            hotbarRect.anchoredPosition = new Vector2(
                hotbarRect.anchoredPosition.x,
                cantaAcikkenYPozisyonu
            );
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void EnvanterKapat()
    {
        if (genelEnvanterPanel != null)
            genelEnvanterPanel.SetActive(false);

        if (hotbarRect != null)
        {
            hotbarRect.anchoredPosition = new Vector2(
                hotbarRect.anchoredPosition.x,
                oyunIciYPozisyonu
            );
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}