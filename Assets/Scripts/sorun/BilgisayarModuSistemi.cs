using UnityEngine;
using UnityEngine.InputSystem;

public class BilgisayarModuSistemi : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject bilgisayarUI;

    [Header("Bilgisayar acikken gizlenecek HUD objeleri")]
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private GameObject hotbarUI;

    public bool BilgisayarModuAktifMi { get; private set; }

    private void Start()
    {
        if (bilgisayarUI != null)
            bilgisayarUI.SetActive(false);

        BilgisayarModuAktifMi = false;

        HUDGoster();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        if (OyuncuKontrolKilidi.KilitliMi)
            return;

        BilgisayarModuAktifMi = true;

        if (bilgisayarUI != null)
        {
            bilgisayarUI.SetActive(true);
            bilgisayarUI.transform.SetAsLastSibling();
        }

        HUDGizle();
        OyuncuKontrolKilidi.Kilitle();
    }

    public void BilgisayarModunuKapat()
    {
        if (!BilgisayarModuAktifMi)
            return;

        BilgisayarModuAktifMi = false;

        if (bilgisayarUI != null)
            bilgisayarUI.SetActive(false);

        HUDGoster();
        OyuncuKontrolKilidi.KilidiKaldir();
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
}
