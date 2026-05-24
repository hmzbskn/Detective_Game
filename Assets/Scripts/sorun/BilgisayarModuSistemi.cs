using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BilgisayarModuSistemi : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject bilgisayarUI;

    [Header("Bilgisayar acikken gizlenecek HUD objeleri")]
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private GameObject hotbarUI;

    [Header("Bu mod acilinca kapatilacak scriptler")]
    [SerializeField] private List<MonoBehaviour> kapatilacakScriptler = new List<MonoBehaviour>();

    public bool BilgisayarModuAktifMi { get; private set; }

    private Dictionary<MonoBehaviour, bool> oncekiScriptDurumlari = new Dictionary<MonoBehaviour, bool>();

    private void Start()
    {
        if (bilgisayarUI != null)
            bilgisayarUI.SetActive(false);

        BilgisayarModuAktifMi = false;

        HUDGoster();
        ImleciKilitle();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (BilgisayarModuAktifMi && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            BilgisayarModunuKapat();
            return;
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (BilgisayarModuAktifMi)
                BilgisayarModunuKapat();
            else
                BilgisayarModunuAc();
        }
    }

    public void BilgisayarModunuAc()
    {
        if (BilgisayarModuAktifMi)
            return;

        BilgisayarModuAktifMi = true;

        if (bilgisayarUI != null)
        {
            bilgisayarUI.SetActive(true);
            bilgisayarUI.transform.SetAsLastSibling();
        }

        HUDGizle();
        ScriptleriKapat();
        ImleciSerbestBirak();
    }

    public void BilgisayarModunuKapat()
    {
        if (!BilgisayarModuAktifMi)
            return;

        BilgisayarModuAktifMi = false;

        if (bilgisayarUI != null)
            bilgisayarUI.SetActive(false);

        HUDGoster();
        ScriptleriAc();
        ImleciKilitle();
    }

    private void HUDGizle()
    {
        if (crosshairUI != null)
            crosshairUI.SetActive(false);

        if (hotbarUI != null)
            hotbarUI.SetActive(false);
    }

    private void HUDGoster()
    {
        if (crosshairUI != null)
            crosshairUI.SetActive(true);

        if (hotbarUI != null)
            hotbarUI.SetActive(true);
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

    private void ImleciSerbestBirak()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ImleciKilitle()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}