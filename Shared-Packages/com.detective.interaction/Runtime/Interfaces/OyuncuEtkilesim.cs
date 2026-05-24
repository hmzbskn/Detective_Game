using UnityEngine;
using UnityEngine.InputSystem;

public class OyuncuEtkilesim : MonoBehaviour
{
    public static OyuncuEtkilesim Instance { get; private set; }

    private static int etkilesimKilitSayisi = 0;
    public static bool EtkilesimKilitliMi => etkilesimKilitSayisi > 0;

    [Header("Ayarlar")]
    [SerializeField] private float mesafe = 3f;
    [SerializeField] private LayerMask etkilesimKatmani;

    [Header("Debug / Raycast Görselleştirme")]
    [SerializeField] private bool raycastGoster = false;
    [SerializeField] private Color raycastBosRenk = Color.red;
    [SerializeField] private Color raycastCarptiRenk = Color.green;
    [SerializeField] private float raycastUcuBoyutu = 0.04f;

    [Header("Bağımlılıklar")]
    [SerializeField] private HotbarSistemi hotbarSistemi;
    [SerializeField] private OyuncuEsyaTutucu esyaTutucu;

    [Header("UI")]
    [SerializeField] private EtkilesimPanelController panel;
    [SerializeField] private CrosshairUIController crosshairUI;

    private Camera anaKamera;
    private IEtkilesebilir aktifObje;
    private IHighlightable aktifHighlight;
    private EldeTutulabilirObje aktifTutulabilirObje;

    private Vector3 sonRayBaslangic;
    private Vector3 sonRayBitis;
    private bool sonRayCarptiMi;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        anaKamera = Camera.main;

        Temizle();
    }

    private void OnDisable()
    {
        Temizle();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (EtkilesimKilitliMi)
        {
            Temizle();
            return;
        }

        if (anaKamera == null)
        {
            anaKamera = Camera.main;

            if (anaKamera == null)
                return;
        }

        bool eBasildi = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;

        Ray ray = new Ray(anaKamera.transform.position, anaKamera.transform.forward);

        bool carptiMi = Physics.Raycast(
            ray,
            out RaycastHit hit,
            mesafe,
            etkilesimKatmani,
            QueryTriggerInteraction.Ignore
        );

        RaycastDebugGuncelle(ray, carptiMi, hit);

        if (carptiMi)
        {
            IEtkilesebilir etkilesebilir = hit.collider.GetComponentInParent<IEtkilesebilir>();
            IHighlightable yeniHighlight = hit.collider.GetComponentInParent<IHighlightable>();
            EldeTutulabilirObje tutulabilir = hit.collider.GetComponentInParent<EldeTutulabilirObje>();

            if (etkilesebilir != null)
            {
                aktifObje = etkilesebilir;
                aktifTutulabilirObje = tutulabilir;

                bool eldeEsyaVar = esyaTutucu != null && !esyaTutucu.EliBosMu();
                bool pickupObjesiMi = aktifTutulabilirObje != null;

                if (eldeEsyaVar && pickupObjesiMi)
                {
                    HighlightGuncelle(null);

                    if (panel != null)
                        panel.Kapat();

                    if (crosshairUI != null)
                        crosshairUI.NormalModaDon();

                    if (eBasildi)
                        return;

                    return;
                }

                HighlightGuncelle(yeniHighlight);
                UIGuncelle();

                if (eBasildi)
                    EtkilesimiCalistir();

                return;
            }
        }

        Temizle();
    }

    public static void EtkilesimiKilitle()
    {
        etkilesimKilitSayisi++;

        OyuncuEtkilesim oyuncuEtkilesim = Instance;

        if (oyuncuEtkilesim == null)
            oyuncuEtkilesim = FindFirstObjectByType<OyuncuEtkilesim>();

        if (oyuncuEtkilesim != null)
            oyuncuEtkilesim.Temizle();
    }

    public static void EtkilesimKilidiniKaldir()
    {
        etkilesimKilitSayisi--;

        if (etkilesimKilitSayisi < 0)
            etkilesimKilitSayisi = 0;
    }

    public static void EtkilesimKilidiniSifirla()
    {
        etkilesimKilitSayisi = 0;

        OyuncuEtkilesim oyuncuEtkilesim = Instance;

        if (oyuncuEtkilesim == null)
            oyuncuEtkilesim = FindFirstObjectByType<OyuncuEtkilesim>();

        if (oyuncuEtkilesim != null)
            oyuncuEtkilesim.Temizle();
    }

    private void RaycastDebugGuncelle(Ray ray, bool carptiMi, RaycastHit hit)
    {
        sonRayBaslangic = ray.origin;
        sonRayCarptiMi = carptiMi;

        if (carptiMi)
            sonRayBitis = hit.point;
        else
            sonRayBitis = ray.origin + ray.direction * mesafe;

        if (!raycastGoster)
            return;

        Color renk = carptiMi ? raycastCarptiRenk : raycastBosRenk;

        Debug.DrawLine(sonRayBaslangic, sonRayBitis, renk);
    }

    private void UIGuncelle()
    {
        if (EtkilesimKilitliMi)
        {
            Temizle();
            return;
        }

        if (panel != null)
        {
            panel.Goster(
                aktifObje.ObjeAdiGetir(),
                aktifObje.EtkilesimMetniGetir()
            );
        }

        if (crosshairUI != null)
            crosshairUI.EtkilesimModunaGec();
    }

    private void EtkilesimiCalistir()
    {
        if (EtkilesimKilitliMi)
            return;

        if (aktifTutulabilirObje != null)
        {
            if (esyaTutucu != null && !esyaTutucu.EliBosMu())
            {
                Debug.Log("El dolu olduğu için başka eşya alınamaz.");
                return;
            }

            if (hotbarSistemi != null)
            {
                hotbarSistemi.EsyayiHotbaraEkleVeSec(aktifTutulabilirObje);
                return;
            }
        }

        aktifObje?.Etkilesim();
    }

    private void HighlightGuncelle(IHighlightable yeniHighlight)
    {
        if (yeniHighlight == aktifHighlight)
            return;

        if (aktifHighlight != null)
            aktifHighlight.HighlightKapat();

        aktifHighlight = yeniHighlight;

        if (aktifHighlight != null)
            aktifHighlight.HighlightAc();
    }

    private void Temizle()
    {
        aktifObje = null;
        aktifTutulabilirObje = null;

        if (panel != null)
            panel.Kapat();

        if (crosshairUI != null)
            crosshairUI.NormalModaDon();

        if (aktifHighlight != null)
        {
            aktifHighlight.HighlightKapat();
            aktifHighlight = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!raycastGoster)
            return;

        Camera kamera = anaKamera != null ? anaKamera : Camera.main;

        if (kamera == null)
            return;

        Vector3 baslangic;
        Vector3 bitis;
        Color renk;

        if (Application.isPlaying)
        {
            baslangic = sonRayBaslangic;
            bitis = sonRayBitis;
            renk = sonRayCarptiMi ? raycastCarptiRenk : raycastBosRenk;
        }
        else
        {
            baslangic = kamera.transform.position;
            bitis = kamera.transform.position + kamera.transform.forward * mesafe;
            renk = raycastBosRenk;
        }

        Gizmos.color = renk;
        Gizmos.DrawLine(baslangic, bitis);
        Gizmos.DrawSphere(bitis, raycastUcuBoyutu);
    }
}