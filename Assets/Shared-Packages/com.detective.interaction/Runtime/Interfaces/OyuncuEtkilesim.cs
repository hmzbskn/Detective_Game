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
    private IEtkilesebilir aktifObje;
    private IHighlightable aktifHighlight;
    private EldeTutulabilirObje aktifTutulabilirObje;

    private Vector3 sonRayBaslangic;
    private Vector3 sonRayBitis;
    private bool sonRayCarptiMi;

    private void Start()
    {
        anaKamera = Camera.main;

        if (panel != null)
            panel.Kapat();

        if (crosshairUI != null)
            crosshairUI.NormalModaDon();
    }

    private void Update()
    {
        if (anaKamera == null)
            return;

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

                // El doluyken pickup objeleri tamamen pasif görünsün
                if (eldeEsyaVar && pickupObjesiMi)
                {
                    HighlightGuncelle(null);

                    if (panel != null)
                        panel.Kapat();

                    if (crosshairUI != null)
                        crosshairUI.NormalModaDon();

                    if (eBasildi)
                    {
                        // El doluyken başka pickup alma yok
                        return;
                    }

                    return;
                }

                HighlightGuncelle(yeniHighlight);
                UIGuncelle();

                if (eBasildi)
                {
                    EtkilesimiCalistir();
                }

                return;
            }
        }

        Temizle();
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
        // Pickup objesi ise özel kural
        if (aktifTutulabilirObje != null)
        {
            // Elde eşya varken başka pickup alma
            if (esyaTutucu != null && !esyaTutucu.EliBosMu())
            {
                Debug.Log("El dolu olduğu için başka eşya alınamaz.");
                return;
            }

            // El boşsa hotbara ekle ve ele geçir
            if (hotbarSistemi != null)
            {
                hotbarSistemi.EsyayiHotbaraEkleVeSec(aktifTutulabilirObje);
                return;
            }
        }

        // Pickup olmayan objelerde normal etkileşim
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