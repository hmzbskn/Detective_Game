using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using StarterAssets;

public class CinayetTahtasiKontrol : MonoBehaviour
{
    [Header("UI Paneli")]
    public GameObject tahtaPaneli;

    [Header("Tahta acikken gizlenecek HUD objeleri")]
    [SerializeField] private GameObject crosshairUI;

    [Header("Bu mod acilinca kapatilacak scriptler")]
    [SerializeField] private List<MonoBehaviour> kapatilacakScriptler = new List<MonoBehaviour>();

    private bool tahtaAcikMi = false;
    private FirstPersonController fpsKontrol;
    private EnvanterKontrol envanterSistemi;

    private Dictionary<MonoBehaviour, bool> oncekiScriptDurumlari = new Dictionary<MonoBehaviour, bool>();

    private void Start()
    {
        if (tahtaPaneli != null)
            tahtaPaneli.SetActive(false);

        fpsKontrol = FindFirstObjectByType<FirstPersonController>();
        envanterSistemi = FindFirstObjectByType<EnvanterKontrol>();

        tahtaAcikMi = false;
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (InputFieldSeciliMi())
            return;

        if (tahtaAcikMi && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TahtayiKapat();
            return;
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (tahtaAcikMi)
                TahtayiKapat();
            else
                TahtayiAc();
        }
    }

    private bool InputFieldSeciliMi()
    {
        if (EventSystem.current == null)
            return false;

        GameObject seciliObje = EventSystem.current.currentSelectedGameObject;

        if (seciliObje == null)
            return false;

        return seciliObje.GetComponent<TMPro.TMP_InputField>() != null;
    }

    private void TahtayiAc()
    {
        if (tahtaAcikMi)
            return;

        tahtaAcikMi = true;

        if (tahtaPaneli != null)
        {
            tahtaPaneli.SetActive(true);
            tahtaPaneli.transform.SetAsLastSibling();
        }

        Time.timeScale = 0f;

        if (fpsKontrol != null)
            fpsKontrol.enabled = false;

        if (envanterSistemi != null)
        {
            envanterSistemi.tahtaModundaMi = true;

            if (envanterSistemi.hotbarPanel != null)
                envanterSistemi.hotbarPanel.SetActive(false);

            if (envanterSistemi.genelEnvanterPanel != null)
                envanterSistemi.genelEnvanterPanel.SetActive(false);
        }

        if (crosshairUI != null)
            crosshairUI.SetActive(false);

        ScriptleriKapat();
        ImleciSerbestBirak();
    }

    public void TahtayiKapat()
    {
        if (!tahtaAcikMi)
            return;

        tahtaAcikMi = false;

        if (tahtaPaneli != null)
            tahtaPaneli.SetActive(false);

        Time.timeScale = 1f;

        if (fpsKontrol != null)
            fpsKontrol.enabled = true;

        if (envanterSistemi != null)
        {
            envanterSistemi.tahtaModundaMi = false;

            if (envanterSistemi.hotbarPanel != null)
                envanterSistemi.hotbarPanel.SetActive(true);

            if (envanterSistemi.genelEnvanterPanel != null)
                envanterSistemi.genelEnvanterPanel.SetActive(false);
        }

        if (crosshairUI != null)
            crosshairUI.SetActive(true);

        ScriptleriAc();
        ImleciKilitle();
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ImleciKilitle()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}