using UnityEngine;
using UnityEngine.InputSystem;

public class OyuncuEtkilesim : MonoBehaviour
{
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
    private BakisRaycastSurucu raycastSurucu;
    private EtkilesimHighlightYoneticisi highlightYoneticisi;

    private IEtkilesebilir aktifObje;
    private EldeTutulabilirObje aktifTutulabilirObje;

    private Vector3 sonRayBaslangic;
    private Vector3 sonRayBitis;
    private bool sonRayCarptiMi;

    private void Awake()
    {
        raycastSurucu = new BakisRaycastSurucu(mesafe, etkilesimKatmani, QueryTriggerInteraction.Ignore);
        highlightYoneticisi = new EtkilesimHighlightYoneticisi();
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

    private void Update()
    {
        if (anaKamera == null)
        {
            anaKamera = Camera.main;

            if (anaKamera == null)
                return;
        }

        bool eBasildi = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;

        bool carptiMi = raycastSurucu.Raycast(anaKamera, out RaycastHit hit);

        RaycastDebugGuncelle(carptiMi, hit);

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
                    highlightYoneticisi.Temizle();

                    if (panel != null)
                        panel.Kapat();

                    if (crosshairUI != null)
                        crosshairUI.NormalModaDon();

                    return;
                }

                highlightYoneticisi.Guncelle(yeniHighlight);
                UIGuncelle();

                if (eBasildi)
                    EtkilesimiCalistir();

                return;
            }
        }

        Temizle();
    }

    private void RaycastDebugGuncelle(bool carptiMi, RaycastHit hit)
    {
        sonRayBaslangic = anaKamera.transform.position;
        sonRayCarptiMi = carptiMi;

        if (carptiMi)
            sonRayBitis = hit.point;
        else
            sonRayBitis = sonRayBaslangic + anaKamera.transform.forward * raycastSurucu.Mesafe;

        if (!raycastGoster)
            return;

        Color renk = carptiMi ? raycastCarptiRenk : raycastBosRenk;

        Debug.DrawLine(sonRayBaslangic, sonRayBitis, renk);
    }

    private void UIGuncelle()
    {
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

    private void Temizle()
    {
        aktifObje = null;
        aktifTutulabilirObje = null;

        if (panel != null)
            panel.Kapat();

        if (crosshairUI != null)
            crosshairUI.NormalModaDon();

        highlightYoneticisi?.Temizle();
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
