using UnityEngine;

public class PencereKontrol : MonoBehaviour
{
    [Header("Ayarlar")]
    public RectTransform anaPencere; // Sürükle bırak (Pencere_Genel)

    private Vector2 eskiBoyut;
    private Vector2 eskiPozisyon;
    private bool tamEkranMi = false;

    void Start()
    {
        // Eğer atamayı unutursan script kendini garantiye alır
        if (anaPencere == null) anaPencere = GetComponent<RectTransform>();
    }

    public void TamEkranGecis()
    {
        if (anaPencere == null) return;

        if (!tamEkranMi)
        {
            // Şimdiki normal boyutunu ve pozisyonunu hafızaya al
            eskiBoyut = anaPencere.sizeDelta;
            eskiPozisyon = anaPencere.anchoredPosition;

            // Tam ekran yap (İçinde bulunduğu Canvas'ın/Ekranın boyutunu alıp tam yayılır)
            RectTransform calismaAlani = anaPencere.parent.GetComponent<RectTransform>();
            anaPencere.sizeDelta = calismaAlani.rect.size;
            anaPencere.anchoredPosition = Vector2.zero; // Merkeze yapıştır

            tamEkranMi = true;
        }
        else
        {
            // İkinci kez basıldıysa eski haline (küçük boyuta) geri dön
            anaPencere.sizeDelta = eskiBoyut;
            anaPencere.anchoredPosition = eskiPozisyon;

            tamEkranMi = false;
        }
    }

    public void AltSekmeyeAl()
    {
        // Görev çubuğu animasyonu vs. yapmayacaksak, alt sekmeye almak
        // aslında pencereyi gizlemektir. Masaüstündeki veya görev çubuğundaki 
        // kendi ikonuna tıklandığında zaten UygulamaYoneticisi bunu geri açacaktır.
        if (anaPencere != null)
        {
            anaPencere.gameObject.SetActive(false);
        }
    }
}