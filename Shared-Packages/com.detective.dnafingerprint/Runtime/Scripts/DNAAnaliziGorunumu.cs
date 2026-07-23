using TMPro;
using UnityEngine;

/// <summary>
/// DNA mini oyununun ekranda gösterdiği her şeyi (soru/sonuç panelleri, durum yazısı, spektrum
/// görseli) tek yerde toplar. DNAMiniGameManager'ın akış (coroutine sıralaması) mantığı, hangi UI
/// elemanının ne zaman görüneceğine karışmadan sadece "şimdi soruyu göster", "şimdi sonucu göster"
/// gibi niyetleri buraya bildirir.
/// </summary>
public class DNAAnaliziGorunumu
{
    private readonly RectTransform spectrumRect;
    private readonly DNADisplay spectrumDisplay;
    private readonly GameObject soruAlani;
    private readonly TextMeshProUGUI soruText;
    private readonly TextMeshProUGUI durumText;
    private readonly GameObject sonucPaneli;
    private readonly TextMeshProUGUI sonucBaslikText;
    private readonly TextMeshProUGUI sonucDetayText;
    private readonly Vector2 spectrumMerkezPozisyon;

    public DNAAnaliziGorunumu(
        RectTransform spectrumRect,
        DNADisplay spectrumDisplay,
        GameObject soruAlani,
        TextMeshProUGUI soruText,
        TextMeshProUGUI durumText,
        GameObject sonucPaneli,
        TextMeshProUGUI sonucBaslikText,
        TextMeshProUGUI sonucDetayText,
        Vector2 spectrumMerkezPozisyon)
    {
        this.spectrumRect = spectrumRect;
        this.spectrumDisplay = spectrumDisplay;
        this.soruAlani = soruAlani;
        this.soruText = soruText;
        this.durumText = durumText;
        this.sonucPaneli = sonucPaneli;
        this.sonucBaslikText = sonucBaslikText;
        this.sonucDetayText = sonucDetayText;
        this.spectrumMerkezPozisyon = spectrumMerkezPozisyon;
    }

    public void Sifirla()
    {
        if (soruAlani != null)
            soruAlani.SetActive(false);

        if (soruText != null)
            soruText.text = "";

        if (durumText != null)
            durumText.text = "";

        if (sonucPaneli != null)
            sonucPaneli.SetActive(false);

        if (sonucBaslikText != null)
            sonucBaslikText.text = "";

        if (sonucDetayText != null)
            sonucDetayText.text = "";

        SpectrumTemizle();

        if (spectrumRect != null)
        {
            spectrumRect.anchoredPosition = spectrumMerkezPozisyon;
            spectrumRect.gameObject.SetActive(false);
        }
    }

    public void SpectrumuMerkezeAl()
    {
        if (spectrumRect == null)
            return;

        spectrumRect.gameObject.SetActive(true);
        spectrumRect.anchoredPosition = spectrumMerkezPozisyon;
    }

    public void SpectrumGoster(DNAData dna)
    {
        if (spectrumDisplay != null)
            spectrumDisplay.DNAGoster(dna);
    }

    public void SpectrumTemizle()
    {
        if (spectrumDisplay != null)
            spectrumDisplay.Temizle();
    }

    public void SoruAlaniniAc(bool acikMi)
    {
        if (soruAlani != null)
            soruAlani.SetActive(acikMi);
    }

    public void SoruYaz(string metin)
    {
        if (soruText != null)
            soruText.text = metin;
    }

    public void DurumYaz(string mesaj)
    {
        if (durumText != null)
            durumText.text = mesaj;

        Debug.Log(mesaj);
    }

    public void SonucGoster(string baslik, string detay)
    {
        SoruAlaniniAc(false);
        SpectrumTemizle();

        if (spectrumRect != null)
            spectrumRect.gameObject.SetActive(false);

        if (sonucPaneli != null)
            sonucPaneli.SetActive(true);

        if (sonucBaslikText != null)
            sonucBaslikText.text = baslik;

        if (sonucDetayText != null)
            sonucDetayText.text = detay;

        DurumYaz(baslik);
    }
}
