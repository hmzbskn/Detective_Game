using UnityEngine;
using UnityEngine.InputSystem;

public class OyuncuDNAToplamaKontrolcusu : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private Camera oyuncuKamerasi;
    [SerializeField] private HotbarSistemi hotbarSistemi;
    [SerializeField] private IncelemeSistemi incelemeSistemi;

    [Header("Gerekli Eşya")]
    [SerializeField] private ItemData temizKulakCopuItemData;

    [Header("Raycast Ayarları")]
    [SerializeField] private float toplamaMesafesi = 3f;
    [SerializeField] private LayerMask dnaNoktasiKatmani;

    [Header("Sistem")]
    [SerializeField] private bool sistemAktif = true;
    [SerializeField] private bool debugMesajlari = true;

    private BakisRaycastSurucu raycastSurucu;
    private EtkilesimHighlightYoneticisi highlightYoneticisi;

    private DNAToplamaNoktasi bakilanNokta;

    private void Awake()
    {
        if (oyuncuKamerasi == null)
            oyuncuKamerasi = Camera.main;

        if (hotbarSistemi == null)
            hotbarSistemi = FindFirstObjectByType<HotbarSistemi>();

        if (incelemeSistemi == null)
            incelemeSistemi = FindFirstObjectByType<IncelemeSistemi>();

        raycastSurucu = new BakisRaycastSurucu(toplamaMesafesi, dnaNoktasiKatmani, QueryTriggerInteraction.Collide);
        highlightYoneticisi = new EtkilesimHighlightYoneticisi();
    }

    private void Update()
    {
        if (!SistemCalisabilirMi())
        {
            BakilanNoktayiTemizle();
            return;
        }

        BakilanNoktayiGuncelle();

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            DNAToplamayiDene();
        }
    }

    private bool SistemCalisabilirMi()
    {
        if (!sistemAktif)
            return false;

        if (oyuncuKamerasi == null)
            return false;

        if (hotbarSistemi == null)
            return false;

        if (temizKulakCopuItemData == null)
            return false;

        if (incelemeSistemi != null && incelemeSistemi.IncelemeAktifMi)
            return false;

        if (hotbarSistemi.EnvanterAcikMi())
            return false;

        if (!hotbarSistemi.AktifHotbarItemMi(temizKulakCopuItemData))
            return false;

        return true;
    }

    private void BakilanNoktayiGuncelle()
    {
        if (raycastSurucu.Raycast(oyuncuKamerasi, out RaycastHit hit))
        {
            DNAToplamaNoktasi yeniNokta = hit.collider.GetComponentInParent<DNAToplamaNoktasi>();

            if (yeniNokta != null && yeniNokta.ToplanabilirMi())
            {
                if (bakilanNokta != yeniNokta)
                {
                    bakilanNokta = yeniNokta;
                    highlightYoneticisi.Guncelle(bakilanNokta);

                    if (debugMesajlari)
                        Debug.Log("DNA alınabilir nokta: " + bakilanNokta.NoktaAdi);
                }

                return;
            }
        }

        BakilanNoktayiTemizle();
    }

    private void DNAToplamayiDene()
    {
        if (debugMesajlari)
            Debug.Log("R tuşu algılandı. DNA toplama deneniyor.");

        if (bakilanNokta == null)
        {
            if (debugMesajlari)
                Debug.Log("DNA alınamadı: Bakılan DNA noktası yok.");
            return;
        }

        DNAToplamaNoktasi hedefNokta = bakilanNokta;
        bakilanNokta = null;

        if (hedefNokta == null)
            return;

        if (!hedefNokta.ToplanabilirMi())
        {
            if (debugMesajlari)
                Debug.LogWarning("DNA alınamadı: Hedef nokta toplanabilir değil. DNA Delili ItemData veya DNAData eksik olabilir.");
            return;
        }

        ItemData dnaDelili = hedefNokta.DNADeliliItemData;
        DNAData alinanDNAData = hedefNokta.DNAData;

        if (dnaDelili == null)
        {
            Debug.LogWarning("DNA noktasında oluşacak delil ItemData atanmamış.");
            return;
        }

        if (alinanDNAData == null)
        {
            Debug.LogWarning("DNA noktasında DNAData atanmamış.");
            return;
        }

        bool degisimBasarili = hotbarSistemi.AktifHotbarIteminiDNAInstanceIleDegistir(
            temizKulakCopuItemData,
            dnaDelili,
            alinanDNAData
        );

        if (!degisimBasarili)
        {
            Debug.LogWarning("DNA alınamadı. Aktif hotbar slotunda temiz kulak çöpü yok veya item değişimi başarısız.");
            return;
        }

        hedefNokta.ToplandiOlarakIsaretle();

        if (debugMesajlari)
            Debug.Log($"DNA başarıyla toplandı: {dnaDelili.ItemAdi} | DNAData: {alinanDNAData.dnaID}");
    }

    private void BakilanNoktayiTemizle()
    {
        if (bakilanNokta != null)
        {
            highlightYoneticisi.Temizle();
            bakilanNokta = null;
        }
    }

    public void SistemAktiflikAyarla(bool aktifMi)
    {
        sistemAktif = aktifMi;

        if (!aktifMi)
            BakilanNoktayiTemizle();
    }
}
