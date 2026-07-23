using UnityEngine;
using StarterAssets; // FPS Controller ve Input bileşenleri için şart

public class BilgisayarliEtkilesim : MonoBehaviour, IEtkilesebilir
{
    [Header("UI Ayarları")]
    public GameObject bilgisayarPaneli; // Hiyerarşideki "Bilgisayar Sistemi" objesi

    private bool bilgisayarAcikMi = false;
    private FirstPersonController fpsKontrol;
    private StarterAssetsInputs oyuncuGirdileri;

    void Awake()
    {
        // Başlangıçta FPS kontrolcüsünü ve girdi sistemini bul
        fpsKontrol = FindFirstObjectByType<FirstPersonController>();
        if (fpsKontrol != null)
        {
            oyuncuGirdileri = fpsKontrol.GetComponent<StarterAssetsInputs>();
        }

        // Eğer Inspector'dan sürüklemeyi unuttuysan hiyerarşiden isme göre bulmayı dener
        if (bilgisayarPaneli == null)
        {
            bilgisayarPaneli = GameObject.Find("Bilgisayar Sistemi");
        }
    }

    void Start()
    {
        // Oyun başında bilgisayar ekranı kapalı başlasın
        if (bilgisayarPaneli != null)
        {
            bilgisayarPaneli.SetActive(false);
        }
    }

    // INTERFACE FONKSİYONLARI (OyuncuEtkilesim.cs buradan okur)
    public string EtkilesimMetniGetir()
    {
        return "Bilgisayarı Kullan";
    }

    public void Etkilesim()
    {
        if (!bilgisayarAcikMi)
        {
            BilgisayariAc();
        }
    }

    public void BilgisayariAc()
    {
        if (bilgisayarPaneli == null) return;

        bilgisayarAcikMi = true;
        bilgisayarPaneli.SetActive(true);

        // 1. Karakter hareketini durdur
        if (fpsKontrol != null) fpsKontrol.enabled = false;

        // 2. Starter Assets Mouse Kilidini Çöz (En kritik kısım burası)
        if (oyuncuGirdileri != null)
        {
            oyuncuGirdileri.cursorLocked = false;
            oyuncuGirdileri.cursorInputForLook = false;

            // Mouse'u görünür ve serbest yap
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void BilgisayariKapat()
    {
        if (bilgisayarPaneli == null) return;

        bilgisayarAcikMi = false;
        bilgisayarPaneli.SetActive(false);

        // 1. Karakter hareketini geri aç
        if (fpsKontrol != null) fpsKontrol.enabled = true;

        // 2. Mouse'u oyuna geri kilitle
        if (oyuncuGirdileri != null)
        {
            oyuncuGirdileri.cursorLocked = true;
            oyuncuGirdileri.cursorInputForLook = true;

            // Mouse'u gizle ve ekrana hapset
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        // Bilgisayar açıkken ESC tuşuyla hızlı çıkış
        if (bilgisayarAcikMi && Input.GetKeyDown(KeyCode.Escape))
        {
            BilgisayariKapat();
        }
    }
}