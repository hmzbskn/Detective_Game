using UnityEngine;
using TMPro; // TextMeshPro kullanıyorsan kalsın

public class FPSGosterici : MonoBehaviour
{
    public TextMeshProUGUI fpsYazisi; // UI'daki yazı objesi
    private float sayac;
    private float zaman;
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        // Kareleri say
        sayac++;
        zaman += Time.unscaledDeltaTime;

        // Her 1 saniyede bir güncelle (ekranın sürekli titrememesi için)
        if (zaman >= 1.0f)
        {
            int fps = Mathf.RoundToInt(sayac / zaman);
            fpsYazisi.text = "FPS: " + fps;

            // Renk paleti: FPS düşükse kırmızı, yüksekse yeşil yapalım
            if (fps < 30) fpsYazisi.color = Color.red;
            else if (fps < 60) fpsYazisi.color = Color.yellow;
            else fpsYazisi.color = Color.green;

            sayac = 0;
            zaman = 0;
        }
    }
}