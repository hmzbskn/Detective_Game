using System.Collections;
using TMPro;
using UnityEngine;

public class DNAMiniGameManager : MonoBehaviour
{
    [Header("DNA Verileri")]
    [SerializeField] private DNAData bulunanDNA;
    [SerializeField] private DNAData[] supheliDNAlar;

    [Header("Spectrum")]
    [SerializeField] private RectTransform spectrumRect;
    [SerializeField] private DNADisplay spectrumDisplay;

    [Header("Soru UI")]
    [SerializeField] private GameObject soruAlani;
    [SerializeField] private TextMeshProUGUI soruText;
    [SerializeField] private TextMeshProUGUI durumText;

    [Header("Pozisyonlar")]
    [SerializeField] private Vector2 spectrumMerkezPozisyon = Vector2.zero;
    [SerializeField] private Vector2 spectrumSolaPozisyon = new Vector2(-600f, 0f);

    [Header("Süreler")]
    [SerializeField] private float bulunanDNASuresi = 5f;
    [SerializeField] private float supheliOncesiBekleme = 2f;
    [SerializeField] private float supheliGostermeSuresi = 3f;

    private int aktifSupheliIndex = 0;
    private bool cevapBekleniyor = false;

    private void Start()
    {
        if (soruAlani != null)
            soruAlani.SetActive(false);

        if (durumText != null)
            durumText.text = "";

        if (spectrumDisplay != null)
            spectrumDisplay.Temizle();
    }

    public void MiniGameBaslat()
    {
        Debug.Log("DNA MINI GAME BASLADI");

        gameObject.SetActive(true);
        StopAllCoroutines();

        aktifSupheliIndex = 0;
        cevapBekleniyor = false;

        if (soruAlani != null)
            soruAlani.SetActive(false);

        if (durumText != null)
            durumText.text = "";

        StartCoroutine(OyunAkisi());
    }

    private IEnumerator OyunAkisi()
    {
        aktifSupheliIndex = 0;
        cevapBekleniyor = false;

        if (spectrumRect != null)
        {
            spectrumRect.gameObject.SetActive(true);
            spectrumRect.anchoredPosition = spectrumMerkezPozisyon;
        }

        if (spectrumDisplay != null)
        {
            spectrumDisplay.Temizle();
            yield return null;
            spectrumDisplay.DNAGoster(bulunanDNA);
        }

        if (soruAlani != null)
            soruAlani.SetActive(false);

        float kalanSure = bulunanDNASuresi;

        while (kalanSure > 0)
        {
            if (durumText != null)
                durumText.text = "Bulunan DNA inceleniyor... " + Mathf.CeilToInt(kalanSure);

            kalanSure -= Time.deltaTime;
            yield return null;
        }

        if (spectrumDisplay != null)
            spectrumDisplay.Temizle();

        yield return new WaitForSeconds(supheliOncesiBekleme);

        StartCoroutine(SiradakiSupheliyiGoster());
    }

    private IEnumerator SiradakiSupheliyiGoster()
    {
        if (aktifSupheliIndex >= supheliDNAlar.Length)
        {
            OyunBitti("Eşleşme bulunamadı!");
            yield break;
        }

        cevapBekleniyor = false;

        if (soruAlani != null)
            soruAlani.SetActive(false);

        DNAData aktifDNA = supheliDNAlar[aktifSupheliIndex];

        if (spectrumRect != null)
        {
            spectrumRect.gameObject.SetActive(true);
            spectrumRect.anchoredPosition = spectrumSolaPozisyon;
        }

        if (spectrumDisplay != null)
        {
            spectrumDisplay.Temizle();
            yield return null;
            spectrumDisplay.DNAGoster(aktifDNA);
        }

        float kalanSure = supheliGostermeSuresi;

        while (kalanSure > 0)
        {
            if (durumText != null)
                durumText.text = aktifDNA.sahibiAdi + " DNA'sı inceleniyor... " + Mathf.CeilToInt(kalanSure);

            kalanSure -= Time.deltaTime;
            yield return null;
        }

        if (durumText != null)
            durumText.text = "";

        if (soruText != null)
            soruText.text = "Bu DNA eşleşiyor mu?";

        if (soruAlani != null)
            soruAlani.SetActive(true);

        cevapBekleniyor = true;
    }

    public void Eslesti()
    {
        if (!cevapBekleniyor)
            return;

        DNAData aktifDNA = supheliDNAlar[aktifSupheliIndex];

        if (soruAlani != null)
            soruAlani.SetActive(false);

        cevapBekleniyor = false;

        if (aktifDNA.dogruEslesmeMi)
            OyunBitti("DOĞRU! DNA eşleşti.");
        else
            OyunBitti("YANLIŞ! Bu DNA eşleşmiyor.");
    }

    public void Eslesmedi()
    {
        if (!cevapBekleniyor)
            return;

        DNAData aktifDNA = supheliDNAlar[aktifSupheliIndex];

        if (soruAlani != null)
            soruAlani.SetActive(false);

        cevapBekleniyor = false;

        if (aktifDNA.dogruEslesmeMi)
        {
            OyunBitti("Yanlış karar! Doğru DNA'yı kaçırdın.");
        }
        else
        {
            aktifSupheliIndex++;
            StartCoroutine(SupheliArasiGecis());
        }
    }

    private IEnumerator SupheliArasiGecis()
    {
        if (spectrumDisplay != null)
            spectrumDisplay.Temizle();

        if (spectrumRect != null)
            spectrumRect.anchoredPosition = spectrumMerkezPozisyon;

        if (durumText != null)
            durumText.text = "Sıradaki DNA hazırlanıyor...";

        yield return new WaitForSeconds(supheliOncesiBekleme);

        StartCoroutine(SiradakiSupheliyiGoster());
    }

    private void OyunBitti(string mesaj)
    {
        if (soruAlani != null)
            soruAlani.SetActive(false);

        cevapBekleniyor = false;

        if (durumText != null)
            durumText.text = mesaj;
    }
}