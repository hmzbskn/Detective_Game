using UnityEngine;
using UnityEngine.InputSystem;

public class IncelemeSistemi : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private Transform incelemeNoktasi;
    [SerializeField] private Camera oyuncuKamerasi;
    [SerializeField] private OyuncuEsyaTutucu oyuncuEsyaTutucu;

    [Header("Inceleme sirasinda gizlenecek UI objeleri")]
    [SerializeField] private GameObject crosshairUI;

    [Header("Ayarlar")]
    [SerializeField] private float dondurmeHizi = 15f;
    [SerializeField] private float zoomHizi = 0.01f;
    [SerializeField] private float minMesafe = 0.2f;
    [SerializeField] private float maxMesafe = 2f;
    [SerializeField] private float baslangicMesafesi = 0.3f;
    [SerializeField] private float incelemeOlcekCarpani = 2.5f;

    private GameObject aktifObje;
    private Transform oncekiParent;
    private Vector3 oncekiLocalPozisyon;
    private Quaternion oncekiLocalRotasyon;
    private Vector3 oncekiLocalScale;

    private Rigidbody aktifRb;
    private Collider[] aktifColliderlar;

    private float mevcutMesafe;
    private bool crosshairOncekiAktiflikDurumu;

    public bool IncelemeAktifMi { get; private set; }

    private void Update()
    {
        if (!IncelemeAktifMi || aktifObje == null)
            return;

        ObjeDondur();
        ZoomYap();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            IncelemeyiBitir();
        }
    }

    public void IncelemeyiBaslat(GameObject hedefObje)
    {
        if (IncelemeAktifMi || hedefObje == null || incelemeNoktasi == null)
            return;

        if (OyuncuKontrolKilidi.KilitliMi)
            return;

        aktifObje = hedefObje;
        mevcutMesafe = baslangicMesafesi;
        IncelemeAktifMi = true;

        CrosshairGizle();

        OyuncuKontrolKilidi.Kilitle();

        oncekiParent = aktifObje.transform.parent;
        oncekiLocalPozisyon = aktifObje.transform.localPosition;
        oncekiLocalRotasyon = aktifObje.transform.localRotation;
        oncekiLocalScale = aktifObje.transform.localScale;

        aktifRb = aktifObje.GetComponent<Rigidbody>();
        if (aktifRb != null)
        {
            aktifRb.linearVelocity = Vector3.zero;
            aktifRb.angularVelocity = Vector3.zero;
            aktifRb.isKinematic = true;
            aktifRb.useGravity = false;
        }

        aktifColliderlar = aktifObje.GetComponentsInChildren<Collider>(true);
        foreach (var col in aktifColliderlar)
        {
            col.enabled = false;
        }

        if (oyuncuEsyaTutucu != null)
        {
            oyuncuEsyaTutucu.PreviewDurdur();
        }

        aktifObje.transform.SetParent(incelemeNoktasi);
        aktifObje.transform.localRotation = Quaternion.identity;
        aktifObje.transform.localPosition = Vector3.forward * mevcutMesafe;

        aktifObje.transform.localScale = oncekiLocalScale * incelemeOlcekCarpani;
    }

    public void IncelemeyiBitir()
    {
        if (!IncelemeAktifMi || aktifObje == null)
            return;

        aktifObje.transform.SetParent(oncekiParent);
        aktifObje.transform.localPosition = oncekiLocalPozisyon;
        aktifObje.transform.localRotation = oncekiLocalRotasyon;
        aktifObje.transform.localScale = oncekiLocalScale;

        if (aktifRb != null)
        {
            aktifRb.isKinematic = true;
            aktifRb.useGravity = false;
        }

        if (aktifColliderlar != null)
        {
            foreach (var col in aktifColliderlar)
            {
                col.enabled = false;
            }
        }

        aktifObje = null;
        aktifRb = null;
        aktifColliderlar = null;
        IncelemeAktifMi = false;

        CrosshairGeriGetir();

        OyuncuKontrolKilidi.KilidiKaldir();
    }

    private void CrosshairGizle()
    {
        if (crosshairUI == null)
            return;

        crosshairOncekiAktiflikDurumu = crosshairUI.activeSelf;
        crosshairUI.SetActive(false);
    }

    private void CrosshairGeriGetir()
    {
        if (crosshairUI == null)
            return;

        crosshairUI.SetActive(crosshairOncekiAktiflikDurumu);
    }

    private void ObjeDondur()
    {
        if (Mouse.current == null)
            return;

        Vector2 delta = Mouse.current.delta.ReadValue();

        float yatay = delta.x * dondurmeHizi * Time.deltaTime;
        float dikey = delta.y * dondurmeHizi * Time.deltaTime;

        aktifObje.transform.Rotate(Vector3.up, -yatay, Space.World);
        aktifObje.transform.Rotate(oyuncuKamerasi.transform.right, dikey, Space.World);
    }

    private void ZoomYap()
    {
        if (Mouse.current == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) < 0.001f)
            return;

        mevcutMesafe -= scroll * zoomHizi;
        mevcutMesafe = Mathf.Clamp(mevcutMesafe, minMesafe, maxMesafe);

        aktifObje.transform.localPosition = Vector3.forward * mevcutMesafe;
    }
}