using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Envanter panelinin açık/kapalı durumunu ve hotbar'ın hangi RectTransform yuvasında göründüğünü
/// yönetir. <see cref="Acik"/>, bu durumun TEK doğruluk kaynağıdır — önceden HotbarSistemi hem
/// bunu EnvanteriAc/Kapat çağrılarında elle, hem de her karede
/// envanterMenusuUI.activeInHierarchy okuyarak ayrıca belirliyordu (iki kaynak, birbirinden
/// habersiz); o ikinci, her-kare çalışan senkronizasyon tamamen kaldırıldı.
/// </summary>
public class EnvanterPanelDurumu
{
    private readonly GameObject envanterMenusuUI;
    private readonly GameObject crosshairUI;
    private readonly RectTransform hotbarKapsayici;
    private readonly RectTransform hotbarNormalSlot;
    private readonly RectTransform hotbarEnvanterSlot;
    private readonly PlayerInput oyuncuInput;
    private readonly MonoBehaviour[] kapatilacakScriptler;
    private readonly bool imlecAcilsin;

    private bool crosshairOncekiAktiflikDurumu;

    public bool Acik { get; private set; }

    public EnvanterPanelDurumu(
        GameObject envanterMenusuUI,
        GameObject crosshairUI,
        RectTransform hotbarKapsayici,
        RectTransform hotbarNormalSlot,
        RectTransform hotbarEnvanterSlot,
        PlayerInput oyuncuInput,
        MonoBehaviour[] kapatilacakScriptler,
        bool imlecAcilsin)
    {
        this.envanterMenusuUI = envanterMenusuUI;
        this.crosshairUI = crosshairUI;
        this.hotbarKapsayici = hotbarKapsayici;
        this.hotbarNormalSlot = hotbarNormalSlot;
        this.hotbarEnvanterSlot = hotbarEnvanterSlot;
        this.oyuncuInput = oyuncuInput;
        this.kapatilacakScriptler = kapatilacakScriptler;
        this.imlecAcilsin = imlecAcilsin;
    }

    public void BaslangicYuvasinaTasi()
    {
        HotbariYuvayaTasi(hotbarNormalSlot);
    }

    public void Ac()
    {
        Acik = true;

        CrosshairGizle();

        if (envanterMenusuUI != null)
            envanterMenusuUI.SetActive(true);

        HotbariYuvayaTasi(hotbarEnvanterSlot);

        OyuncuKontrolleriniAyarla(false);

        if (imlecAcilsin)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Kapat()
    {
        Acik = false;

        HotbariYuvayaTasi(hotbarNormalSlot);

        if (envanterMenusuUI != null)
            envanterMenusuUI.SetActive(false);

        OyuncuKontrolleriniAyarla(true);

        CrosshairGeriGetir();

        if (imlecAcilsin)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OyuncuKontrolleriniAyarla(bool aktifMi)
    {
        if (oyuncuInput != null)
            oyuncuInput.enabled = aktifMi;

        if (kapatilacakScriptler == null)
            return;

        foreach (MonoBehaviour script in kapatilacakScriptler)
        {
            if (script != null)
                script.enabled = aktifMi;
        }
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

    private void HotbariYuvayaTasi(RectTransform hedefYuva)
    {
        if (hotbarKapsayici == null)
        {
            Debug.LogWarning("Hotbar Kapsayici atanmadı.");
            return;
        }

        if (hedefYuva == null)
        {
            Debug.LogWarning("Hotbar hedef yuvası atanmadı.");
            return;
        }

        if (hotbarKapsayici.parent == hedefYuva)
            return;

        hotbarKapsayici.SetParent(hedefYuva, false);

        hotbarKapsayici.anchorMin = Vector2.zero;
        hotbarKapsayici.anchorMax = Vector2.one;
        hotbarKapsayici.pivot = new Vector2(0.5f, 0.5f);

        hotbarKapsayici.offsetMin = Vector2.zero;
        hotbarKapsayici.offsetMax = Vector2.zero;
        hotbarKapsayici.anchoredPosition = Vector2.zero;
        hotbarKapsayici.sizeDelta = Vector2.zero;

        hotbarKapsayici.localScale = Vector3.one;
        hotbarKapsayici.localRotation = Quaternion.identity;

        hotbarKapsayici.SetAsLastSibling();

        Canvas.ForceUpdateCanvases();
    }
}
