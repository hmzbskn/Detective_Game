using System.Collections.Generic;
using UnityEngine;

public class NPCKonusmaKilidi : MonoBehaviour
{
    [Header("Konuşma sırasında gizlenecek HUD objeleri")]
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private GameObject hotbarUI;

    [Header("Konuşma sırasında kapatılacak scriptler")]
    [SerializeField] private List<MonoBehaviour> kapatilacakScriptler = new List<MonoBehaviour>();

    public bool KonusmaKilidiAktifMi { get; private set; }

    private void Start()
    {
        KonusmaKilidiAktifMi = false;
        HUDGoster();
        ImleciKilitle();
    }

    public void KonusmaKilidiniAc()
    {
        if (KonusmaKilidiAktifMi)
            return;

        KonusmaKilidiAktifMi = true;

        HUDGizle();
        ScriptleriKapat();
        ImleciSerbestBirak();
    }

    public void KonusmaKilidiniKapat()
    {
        if (!KonusmaKilidiAktifMi)
            return;

        KonusmaKilidiAktifMi = false;

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
        for (int i = 0; i < kapatilacakScriptler.Count; i++)
        {
            if (kapatilacakScriptler[i] != null)
                kapatilacakScriptler[i].enabled = false;
        }
    }

    private void ScriptleriAc()
    {
        for (int i = 0; i < kapatilacakScriptler.Count; i++)
        {
            if (kapatilacakScriptler[i] != null)
                kapatilacakScriptler[i].enabled = true;
        }
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