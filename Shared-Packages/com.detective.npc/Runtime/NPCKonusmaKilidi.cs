using System.Collections.Generic;
using UnityEngine;

public class NPCKonusmaKilidi : MonoBehaviour
{
    [Header("Konuşma sırasında gizlenecek HUD objeleri")]
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private GameObject hotbarUI;

    [Header("Konuşma sırasında gizlenecek ekstra UI objeleri")]
    [SerializeField] private GameObject etkilesimPaneli;

    [Header("Konuşma sırasında kapatılacak scriptler")]
    [SerializeField] private List<MonoBehaviour> kapatilacakScriptler = new List<MonoBehaviour>();

    public bool KonusmaKilidiAktifMi { get; private set; }

    private Dictionary<GameObject, bool> oncekiUIDurumlari = new Dictionary<GameObject, bool>();
    private Dictionary<MonoBehaviour, bool> oncekiScriptDurumlari = new Dictionary<MonoBehaviour, bool>();

    private CursorLockMode oncekiCursorLockState;
    private bool oncekiCursorVisible;

    private void Start()
    {
        KonusmaKilidiAktifMi = false;
    }

    public void KonusmaKilidiniAc()
    {
        if (KonusmaKilidiAktifMi)
            return;

        KonusmaKilidiAktifMi = true;

        CursorDurumunuKaydet();

        UIObjeleriniGizle();
        ScriptleriKapat();
        ImleciSerbestBirak();
    }

    public void KonusmaKilidiniKapat()
    {
        if (!KonusmaKilidiAktifMi)
            return;

        KonusmaKilidiAktifMi = false;

        UIObjeleriniGeriYukle();
        ScriptleriAc();
        CursorDurumunuGeriYukle();
    }

    private void UIObjeleriniGizle()
    {
        oncekiUIDurumlari.Clear();

        UIObjesiniGizle(crosshairUI);
        UIObjesiniGizle(hotbarUI);
        UIObjesiniGizle(etkilesimPaneli);
    }

    private void UIObjesiniGizle(GameObject uiObjesi)
    {
        if (uiObjesi == null)
            return;

        if (!oncekiUIDurumlari.ContainsKey(uiObjesi))
            oncekiUIDurumlari.Add(uiObjesi, uiObjesi.activeSelf);

        uiObjesi.SetActive(false);
    }

    private void UIObjeleriniGeriYukle()
    {
        foreach (KeyValuePair<GameObject, bool> kayit in oncekiUIDurumlari)
        {
            if (kayit.Key == null)
                continue;

            kayit.Key.SetActive(kayit.Value);
        }

        oncekiUIDurumlari.Clear();
    }

    private void ScriptleriKapat()
    {
        oncekiScriptDurumlari.Clear();

        for (int i = 0; i < kapatilacakScriptler.Count; i++)
        {
            MonoBehaviour script = kapatilacakScriptler[i];

            if (script == null)
                continue;

            if (script == this)
                continue;

            if (!oncekiScriptDurumlari.ContainsKey(script))
                oncekiScriptDurumlari.Add(script, script.enabled);

            script.enabled = false;
        }
    }

    private void ScriptleriAc()
    {
        foreach (KeyValuePair<MonoBehaviour, bool> kayit in oncekiScriptDurumlari)
        {
            if (kayit.Key == null)
                continue;

            kayit.Key.enabled = kayit.Value;
        }

        oncekiScriptDurumlari.Clear();
    }

    private void CursorDurumunuKaydet()
    {
        oncekiCursorLockState = Cursor.lockState;
        oncekiCursorVisible = Cursor.visible;
    }

    private void CursorDurumunuGeriYukle()
    {
        Cursor.lockState = oncekiCursorLockState;
        Cursor.visible = oncekiCursorVisible;
    }

    private void ImleciSerbestBirak()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}