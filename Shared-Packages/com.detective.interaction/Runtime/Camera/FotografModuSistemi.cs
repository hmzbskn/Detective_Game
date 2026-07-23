using System.Collections.Generic;
using UnityEngine;

public class FotografModuSistemi : MonoBehaviour
{
    [Header("Fotoğraf Overlay")]
    [SerializeField] private GameObject fotografOverlay;

    [Header("Fotoğraf Modunda Gizlenecek UI Objeleri")]
    [SerializeField] private GameObject[] gizlenecekUIObjeleri;

    [Header("Fotoğraf Modunda Kapatılacak Scriptler")]
    [Tooltip("OyuncuEtkilesim, OyuncuEsyaInput gibi scriptleri koy. FotografMakinesiController'ı buraya koyma.")]
    [SerializeField] private MonoBehaviour[] kapatilacakScriptler;

    [Header("Eşya Preview Kilidi")]
    [SerializeField] private OyuncuEsyaTutucu oyuncuEsyaTutucu;

    [Header("Cursor")]
    [SerializeField] private bool cursorKilitliKalsin = true;

    private readonly Dictionary<GameObject, bool> uiEskiDurumlari = new Dictionary<GameObject, bool>();
    private readonly Dictionary<MonoBehaviour, bool> scriptEskiDurumlari = new Dictionary<MonoBehaviour, bool>();

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

        ModAktifMi = true;

        UIObjeleriniGizle();
        ScriptleriKapat();

        if (oyuncuEsyaTutucu != null)
            oyuncuEsyaTutucu.PreviewDurdur();

        if (fotografOverlay != null)
            fotografOverlay.SetActive(true);

        if (cursorKilitliKalsin)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Kapat()
    {
        if (!ModAktifMi)
            return;

        ModAktifMi = false;

        if (fotografOverlay != null)
            fotografOverlay.SetActive(false);

        UIObjeleriniGeriAc();
        ScriptleriGeriAc();

        if (cursorKilitliKalsin)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
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

    private void ScriptleriKapat()
    {
        scriptEskiDurumlari.Clear();

        if (kapatilacakScriptler == null)
            return;

        for (int i = 0; i < kapatilacakScriptler.Length; i++)
        {
            MonoBehaviour script = kapatilacakScriptler[i];

            if (script == null)
                continue;

            if (script == this)
                continue;

            scriptEskiDurumlari[script] = script.enabled;
            script.enabled = false;
        }
    }

    private void ScriptleriGeriAc()
    {
        foreach (KeyValuePair<MonoBehaviour, bool> pair in scriptEskiDurumlari)
        {
            if (pair.Key != null)
                pair.Key.enabled = pair.Value;
        }

        scriptEskiDurumlari.Clear();
    }
}