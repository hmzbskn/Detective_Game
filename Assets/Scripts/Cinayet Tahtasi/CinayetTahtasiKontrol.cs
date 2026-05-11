using UnityEngine;
using StarterAssets;
using UnityEngine.EventSystems; // YENİ: Oyuncunun nereye tıkladığını (InputField'ı) anlamak için!

public class CinayetTahtasiKontrol : MonoBehaviour
{
    [Header("UI Paneli")]
    public GameObject tahtaPaneli;

    private bool tahtaAcikMi = false;
    private FirstPersonController fpsKontrol;
    private EnvanterKontrol envanterSistemi;

    void Start()
    {
        if (tahtaPaneli != null) tahtaPaneli.SetActive(false);
        fpsKontrol = FindFirstObjectByType<FirstPersonController>();
        envanterSistemi = FindFirstObjectByType<EnvanterKontrol>();
    }

    void Update()
    {
        // 1. BÜYÜK ÇÖZÜM: T Harfi Bug'ı Kalkanı
        // Eğer oyuncu şu an ekrandaki bir UI objesine odaklanmışsa (yazı yazıyorsa)...
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            // O odaklandığı şey bir InputField ise klavye kısayollarını iptal et!
            if (EventSystem.current.currentSelectedGameObject.GetComponent<TMPro.TMP_InputField>() != null)
            {
                return; // Kod buradan döner, aşağıdaki T veya ESC tuşlarını okumaz bile!
            }
        }

        // Normal kısayollar
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (tahtaAcikMi) TahtayiKapat();
            else TahtayiAc();
        }

        if (tahtaAcikMi && Input.GetKeyDown(KeyCode.Escape))
        {
            TahtayiKapat();
        }
    }

    private void TahtayiAc()
    {
        tahtaPaneli.SetActive(true);
        tahtaAcikMi = true;
        Time.timeScale = 0f;

        if (fpsKontrol != null) fpsKontrol.enabled = false;

        if (envanterSistemi != null)
        {
            envanterSistemi.tahtaModundaMi = true;
            if (envanterSistemi.hotbarPanel != null) envanterSistemi.hotbarPanel.SetActive(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void TahtayiKapat()
    {
        tahtaPaneli.SetActive(false);
        tahtaAcikMi = false;
        Time.timeScale = 1f;

        if (fpsKontrol != null) fpsKontrol.enabled = true;

        if (envanterSistemi != null)
        {
            envanterSistemi.tahtaModundaMi = false;
            if (envanterSistemi.hotbarPanel != null) envanterSistemi.hotbarPanel.SetActive(true);
            if (envanterSistemi.genelEnvanterPanel != null) envanterSistemi.genelEnvanterPanel.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}