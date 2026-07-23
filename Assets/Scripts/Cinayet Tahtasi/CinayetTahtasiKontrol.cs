using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CinayetTahtasiKontrol : MonoBehaviour
{
    [Header("UI Paneli")]
    public GameObject tahtaPaneli;

    [Header("Tahta acikken gizlenecek HUD objeleri")]
    [SerializeField] private GameObject crosshairUI;

    private bool tahtaAcikMi = false;

    private void Start()
    {
        if (tahtaPaneli != null)
            tahtaPaneli.SetActive(false);

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

        if (OyuncuKontrolKilidi.KilitliMi)
            return;

        tahtaAcikMi = true;

        if (tahtaPaneli != null)
        {
            tahtaPaneli.SetActive(true);
            tahtaPaneli.transform.SetAsLastSibling();
        }

        Time.timeScale = 0f;

        if (crosshairUI != null)
            crosshairUI.SetActive(false);

        OyuncuKontrolKilidi.Kilitle();
    }

    public void TahtayiKapat()
    {
        if (!tahtaAcikMi)
            return;

        tahtaAcikMi = false;

        if (tahtaPaneli != null)
            tahtaPaneli.SetActive(false);

        Time.timeScale = 1f;

        if (crosshairUI != null)
            crosshairUI.SetActive(true);

        OyuncuKontrolKilidi.KilidiKaldir();
    }
}
