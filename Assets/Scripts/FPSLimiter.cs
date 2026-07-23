using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    [Header("FPS Ayarları")]
    [SerializeField] private int hedefFPS = 60;

    [Header("VSync Kullanılsın mı?")]
    [SerializeField] private bool vSyncKullan = false;

    void Awake()
    {
        FPSAyarla(hedefFPS);
    }

    public void FPSAyarla(int fps)
    {
        hedefFPS = fps;

        if (vSyncKullan)
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1; // VSync kontrol eder
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = hedefFPS;
        }
    }

    // UI'dan toggle ile aç/kapat
    public void VSyncDegistir(bool aktifMi)
    {
        vSyncKullan = aktifMi;
        FPSAyarla(hedefFPS);
    }
}