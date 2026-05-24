using System.Collections;
using TMPro;
using UnityEngine;

public class DNAMiniGameManager : MonoBehaviour
{
    [Header("Runtime DNA Verileri")]
    [SerializeField] private DNAData olayYeriDNA;
    [SerializeField] private DNAData[] supheliDNAlar;

    [Header("Tek Spectrum Display")]
    [SerializeField] private RectTransform spectrumRect;
    [SerializeField] private DNADisplay spectrumDisplay;

    [Header("Soru UI")]
    [SerializeField] private GameObject soruAlani;
    [SerializeField] private TextMeshProUGUI soruText;
    [SerializeField] private TextMeshProUGUI durumText;

    [Header("Sonuç UI")]
    [SerializeField] private GameObject sonucPaneli;
    [SerializeField] private TextMeshProUGUI sonucBaslikText;
    [SerializeField] private TextMeshProUGUI sonucDetayText;

    [Header("Pozisyon")]
    [SerializeField] private Vector2 spectrumMerkezPozisyon = Vector2.zero;

    [Header("Süreler")]
    [SerializeField] private float olayYeriDNASuresi = 5f;
    [SerializeField] private float supheliOncesiBekleme = 2f;
    [SerializeField] private float supheliGostermeSuresi = 3f;

    private int aktifSupheliIndex = 0;
    private bool cevapBekleniyor = false;
    private Coroutine aktifAkis;

    private void Start()
    {
        AnaliziSifirla();
    }

    private void OnDisable()
    {
        AnaliziSifirla();
    }

    public void MiniGameBaslat()
    {
        MiniGameBaslat(olayYeriDNA, supheliDNAlar);
    }

    public void MiniGameBaslat(DNAData runtimeOlayYeriDNA, DNAData[] runtimeSupheliDNAlar)
    {
        gameObject.SetActive(true);

        olayYeriDNA = runtimeOlayYeriDNA;
        supheliDNAlar = runtimeSupheliDNAlar;

        AnaliziSifirla();

        aktifAkis = StartCoroutine(OyunAkisi());
    }

    public void AnaliziSifirla()
    {
        if (aktifAkis != null)
        {
            StopCoroutine(aktifAkis);
            aktifAkis = null;
        }

        aktifSupheliIndex = 0;
        cevapBekleniyor = false;

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

        if (spectrumDisplay != null)
            spectrumDisplay.Temizle();

        if (spectrumRect != null)
        {
            spectrumRect.anchoredPosition = spectrumMerkezPozisyon;
            spectrumRect.gameObject.SetActive(false);
        }
    }

    private IEnumerator OyunAkisi()
    {
        if (olayYeriDNA == null)
        {
            SonucGoster(
                "ANALİZ BAŞLATILAMADI",
                "Olay yerinden alınmış DNA örneği bulunamadı.\n\nÖnce cesetten veya olay yerinden DNA örneği alınmalıdır."
            );
            yield break;
        }

        if (supheliDNAlar == null || supheliDNAlar.Length == 0)
        {
            SonucGoster(
                "ANALİZ BAŞLATILAMADI",
                "Şüphelilerden alınmış DNA örneği bulunamadı.\n\nEn az bir şüpheliden DNA örneği alınmalıdır."
            );
            yield break;
        }

        aktifSupheliIndex = 0;
        cevapBekleniyor = false;

        SpectrumuMerkezeAl();

        if (spectrumDisplay != null)
        {
            spectrumDisplay.Temizle();
            yield return null;
            spectrumDisplay.DNAGoster(olayYeriDNA);
        }

        if (soruAlani != null)
            soruAlani.SetActive(false);

        float kalanSure = olayYeriDNASuresi;

        while (kalanSure > 0)
        {
            DurumYaz("Olay yerinde bulunan DNA inceleniyor... " + Mathf.CeilToInt(kalanSure));
            kalanSure -= Time.deltaTime;
            yield return null;
        }

        if (spectrumDisplay != null)
            spectrumDisplay.Temizle();

        yield return new WaitForSeconds(supheliOncesiBekleme);

        aktifAkis = StartCoroutine(SiradakiSupheliyiGoster());
    }

    private IEnumerator SiradakiSupheliyiGoster()
    {
        if (supheliDNAlar == null || aktifSupheliIndex >= supheliDNAlar.Length)
        {
            SonucGoster(
                "DNA EŞLEŞMESİ BULUNAMADI",
                "Olay yeri DNA’sı, alınan şüpheli DNA örneklerinin hiçbiriyle eşleşmedi.\n\n" +
                "Sonuç: Mevcut DNA örnekleri katili doğrulamak için yeterli değildir."
            );
            yield break;
        }

        cevapBekleniyor = false;

        if (soruAlani != null)
            soruAlani.SetActive(false);

        DNAData aktifSupheliDNA = supheliDNAlar[aktifSupheliIndex];

        if (aktifSupheliDNA == null)
        {
            aktifSupheliIndex++;
            aktifAkis = StartCoroutine(SiradakiSupheliyiGoster());
            yield break;
        }

        SpectrumuMerkezeAl();

        if (spectrumDisplay != null)
        {
            spectrumDisplay.Temizle();
            yield return null;
            spectrumDisplay.DNAGoster(aktifSupheliDNA);
        }

        float kalanSure = supheliGostermeSuresi;

        while (kalanSure > 0)
        {
            DurumYaz(aktifSupheliDNA.sahibiAdi + " DNA'sı inceleniyor... " + Mathf.CeilToInt(kalanSure));
            kalanSure -= Time.deltaTime;
            yield return null;
        }

        DurumYaz("");

        if (soruText != null)
        {
            soruText.text =
                aktifSupheliDNA.sahibiAdi +
                " DNA’sı olay yerinde bulunan DNA ile eşleşiyor mu?";
        }

        if (soruAlani != null)
            soruAlani.SetActive(true);

        cevapBekleniyor = true;
    }

    public void Eslesti()
    {
        if (!cevapBekleniyor)
            return;

        DNAData aktifSupheliDNA = supheliDNAlar[aktifSupheliIndex];

        if (soruAlani != null)
            soruAlani.SetActive(false);

        cevapBekleniyor = false;

        SonucGoster(
            "DNA RAPORU OLUŞTURULDU",
            "Olay yeri DNA’sı ile " + GuvenliAd(aktifSupheliDNA) + " DNA’sı eşleşti olarak rapora işlendi.\n\n" +
            "Olay yeri DNA’sı: " + GuvenliAd(olayYeriDNA) + "\n" +
            "Raporlanan şüpheli: " + GuvenliAd(aktifSupheliDNA) + "\n\n" +
            "Sonuç: Bu eşleşme soruşturma dosyasına delil olarak eklendi."
        );

        // İstersen burada ileride rapor sistemine kayıt atarsın:
        // DNAAnalizRaporManager.Instance.RaporKaydet(olayYeriDNA, aktifSupheliDNA);
    }

    public void Eslesmedi()
    {
        if (!cevapBekleniyor)
            return;

        DNAData aktifSupheliDNA = supheliDNAlar[aktifSupheliIndex];

        if (soruAlani != null)
            soruAlani.SetActive(false);

        cevapBekleniyor = false;

        aktifSupheliIndex++;

        if (aktifSupheliIndex >= supheliDNAlar.Length)
        {
            SonucGoster(
                "DNA RAPORU TAMAMLANDI",
                "Olay yeri DNA’sı ile mevcut şüpheli DNA örnekleri arasında eşleşme raporlanmadı.\n\n" +
                "Olay yeri DNA’sı: " + GuvenliAd(olayYeriDNA) + "\n\n" +
                "Sonuç: Yeni şüpheli DNA örnekleri toplanabilir veya mevcut soruşturma farklı delillerle sürdürülebilir."
            );

            return;
        }

        aktifAkis = StartCoroutine(SupheliArasiGecis());
    }

    private IEnumerator SupheliArasiGecis()
    {
        if (spectrumDisplay != null)
            spectrumDisplay.Temizle();

        SpectrumuMerkezeAl();

        DurumYaz("Bu DNA eşleşmedi. Sıradaki şüpheli DNA hazırlanıyor...");

        yield return new WaitForSeconds(supheliOncesiBekleme);

        aktifAkis = StartCoroutine(SiradakiSupheliyiGoster());
    }

    private bool DNALarEslesiyorMu(DNAData birinci, DNAData ikinci)
    {
        if (birinci == null || ikinci == null)
            return false;

        if (birinci.spektrum == null || ikinci.spektrum == null)
            return false;

        if (birinci.spektrum.Length != ikinci.spektrum.Length)
            return false;

        for (int i = 0; i < birinci.spektrum.Length; i++)
        {
            if (birinci.spektrum[i] != ikinci.spektrum[i])
                return false;
        }

        return true;
    }

    private void SpectrumuMerkezeAl()
    {
        if (spectrumRect == null)
            return;

        spectrumRect.gameObject.SetActive(true);
        spectrumRect.anchoredPosition = spectrumMerkezPozisyon;
    }

    private void SonucGoster(string baslik, string detay)
    {
        if (aktifAkis != null)
        {
            StopCoroutine(aktifAkis);
            aktifAkis = null;
        }

        cevapBekleniyor = false;

        if (soruAlani != null)
            soruAlani.SetActive(false);

        if (spectrumDisplay != null)
            spectrumDisplay.Temizle();

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

    private string GuvenliAd(DNAData dna)
    {
        if (dna == null)
            return "Bilinmeyen DNA";

        if (!string.IsNullOrWhiteSpace(dna.sahibiAdi))
            return dna.sahibiAdi;

        if (!string.IsNullOrWhiteSpace(dna.dnaID))
            return dna.dnaID;

        return "İsimsiz DNA";
    }

    private void DurumYaz(string mesaj)
    {
        if (durumText != null)
            durumText.text = mesaj;

        Debug.Log(mesaj);
    }
}