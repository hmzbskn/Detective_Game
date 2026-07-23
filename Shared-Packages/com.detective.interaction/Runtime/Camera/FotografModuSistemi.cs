using System.Collections.Generic;
using UnityEngine;

public class FotografModuSistemi : MonoBehaviour
{
    [Header("Fotoğraf Overlay")]
    [SerializeField] private GameObject fotografOverlay;

    [Header("Fotoğraf Modunda Gizlenecek UI Objeleri")]
    [SerializeField] private GameObject[] gizlenecekUIObjeleri;

    [Header("Eşya Preview Kilidi")]
    [SerializeField] private OyuncuEsyaTutucu oyuncuEsyaTutucu;

    [Header("Cursor")]
    [Tooltip("Açık iken cursor kilitli/gizli kalsın mı? (Fotoğraf modu nişan alma tabanlı olduğu için varsayılan olarak açık.)")]
    [SerializeField] private bool cursorKilitliKalsin = true;

    private readonly Dictionary<GameObject, bool> uiEskiDurumlari = new Dictionary<GameObject, bool>();

    public bool ModAktifMi { get; private set; }

    private void Awake()
    {
        if (fotografOverlay != null)
            fotografOverlay.SetActive(false);

        if (oyuncuEsyaTutucu == null)
            oyuncuEsyaTutucu = FindFirstObjectByType<OyuncuEsyaTutucu>();
    }

    public void Ac()
    {
        if (ModAktifMi)
            return;

        if (OyuncuKontrolKilidi.KilitliMi)
            return;

        ModAktifMi = true;

        UIObjeleriniGizle();
        OyuncuKontrolKilidi.Kilitle(!cursorKilitliKalsin);

        if (oyuncuEsyaTutucu != null)
            oyuncuEsyaTutucu.PreviewDurdur();

        if (fotografOverlay != null)
            fotografOverlay.SetActive(true);
    }

    public void Kapat()
    {
        if (!ModAktifMi)
            return;

        ModAktifMi = false;

        if (fotografOverlay != null)
            fotografOverlay.SetActive(false);

        UIObjeleriniGeriAc();
        OyuncuKontrolKilidi.KilidiKaldir();
    }

    private void UIObjeleriniGizle()
    {
        uiEskiDurumlari.Clear();

        if (gizlenecekUIObjeleri == null)
            return;

        for (int i = 0; i < gizlenecekUIObjeleri.Length; i++)
        {
            GameObject ui = gizlenecekUIObjeleri[i];

            if (ui == null)
                continue;

            uiEskiDurumlari[ui] = ui.activeSelf;
            ui.SetActive(false);
        }
    }

    private void UIObjeleriniGeriAc()
    {
        foreach (KeyValuePair<GameObject, bool> pair in uiEskiDurumlari)
        {
            if (pair.Key != null)
                pair.Key.SetActive(pair.Value);
        }

        uiEskiDurumlari.Clear();
    }
}
