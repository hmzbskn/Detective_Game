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

    private DNAAnaliziGorunumu gorunum;

    private int aktifSupheliIndex = 0;
    private bool cevapBekleniyor = false;
    private Coroutine aktifAkis;

    private void Awake()
    {
        gorunum = new DNAAnaliziGorunumu(
            spectrumRect,
            spectrumDisplay,
            soruAlani,
            soruText,
            durumText,
            sonucPaneli,
            sonucBaslikText,
            sonucDetayText,
            spectrumMerkezPozisyon
        );
    }

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

        gorunum.Sifirla();
    }

    private IEnumerator OyunAkisi()
    {
        if (olayYeriDNA == null)
        {
            SonucGoster(DNAAnaliziMetinleri.AnalizBaslatilamadiBaslik, DNAAnaliziMetinleri.OlayYeriDNAsiYok);
            yield break;
        }

        if (supheliDNAlar == null || supheliDNAlar.Length == 0)
        {
            SonucGoster(DNAAnaliziMetinleri.AnalizBaslatilamadiBaslik, DNAAnaliziMetinleri.SupheliDNAsiYok);
            yield break;
        }

        aktifSupheliIndex = 0;
        cevapBekleniyor = false;

        gorunum.SpectrumuMerkezeAl();

        gorunum.SpectrumTemizle();
        yield return null;
        gorunum.SpectrumGoster(olayYeriDNA);

        gorunum.SoruAlaniniAc(false);

        float kalanSure = olayYeriDNASuresi;

        while (kalanSure > 0)
        {
            gorunum.DurumYaz(DNAAnaliziMetinleri.OlayYeriIncelemeDurumu(Mathf.CeilToInt(kalanSure)));
            kalanSure -= Time.deltaTime;
            yield return null;
        }

        gorunum.SpectrumTemizle();

        yield return new WaitForSeconds(supheliOncesiBekleme);

        aktifAkis = StartCoroutine(SiradakiSupheliyiGoster());
    }

    private IEnumerator SiradakiSupheliyiGoster()
    {
        if (supheliDNAlar == null || aktifSupheliIndex >= supheliDNAlar.Length)
        {
            SonucGoster(DNAAnaliziMetinleri.EslesmeBulunamadiBaslik, DNAAnaliziMetinleri.EslesmeBulunamadiDetay);
            yield break;
        }

        cevapBekleniyor = false;

        gorunum.SoruAlaniniAc(false);

        DNAData aktifSupheliDNA = supheliDNAlar[aktifSupheliIndex];

        if (aktifSupheliDNA == null)
        {
            aktifSupheliIndex++;
            aktifAkis = StartCoroutine(SiradakiSupheliyiGoster());
            yield break;
        }

        gorunum.SpectrumuMerkezeAl();

        gorunum.SpectrumTemizle();
        yield return null;
        gorunum.SpectrumGoster(aktifSupheliDNA);

        float kalanSure = supheliGostermeSuresi;

        while (kalanSure > 0)
        {
            gorunum.DurumYaz(DNAAnaliziMetinleri.SupheliIncelemeDurumu(aktifSupheliDNA.sahibiAdi, Mathf.CeilToInt(kalanSure)));
            kalanSure -= Time.deltaTime;
            yield return null;
        }

        gorunum.DurumYaz("");

        gorunum.SoruYaz(DNAAnaliziMetinleri.EslesmeSorusu(aktifSupheliDNA.sahibiAdi));
        gorunum.SoruAlaniniAc(true);

        cevapBekleniyor = true;
    }

    public void Eslesti()
    {
        if (!cevapBekleniyor)
            return;

        DNAData aktifSupheliDNA = supheliDNAlar[aktifSupheliIndex];

        gorunum.SoruAlaniniAc(false);

        cevapBekleniyor = false;

        SonucGoster(
            DNAAnaliziMetinleri.RaporOlusturulduBaslik,
            DNAAnaliziMetinleri.RaporOlusturulduDetay(GuvenliAd(olayYeriDNA), GuvenliAd(aktifSupheliDNA))
        );

        // İstersen burada ileride rapor sistemine kayıt atarsın:
        // DNAAnalizRaporManager.Instance.RaporKaydet(olayYeriDNA, aktifSupheliDNA);
    }

    public void Eslesmedi()
    {
        if (!cevapBekleniyor)
            return;

        gorunum.SoruAlaniniAc(false);

        cevapBekleniyor = false;

        aktifSupheliIndex++;

        if (aktifSupheliIndex >= supheliDNAlar.Length)
        {
            SonucGoster(
                DNAAnaliziMetinleri.RaporTamamlandiBaslik,
                DNAAnaliziMetinleri.RaporTamamlandiDetay(GuvenliAd(olayYeriDNA))
            );

            return;
        }

        aktifAkis = StartCoroutine(SupheliArasiGecis());
    }

    private IEnumerator SupheliArasiGecis()
    {
        gorunum.SpectrumTemizle();
        gorunum.SpectrumuMerkezeAl();

        gorunum.DurumYaz(DNAAnaliziMetinleri.SupheliGecisDurumu);

        yield return new WaitForSeconds(supheliOncesiBekleme);

        aktifAkis = StartCoroutine(SiradakiSupheliyiGoster());
    }

    private void SonucGoster(string baslik, string detay)
    {
        if (aktifAkis != null)
        {
            StopCoroutine(aktifAkis);
            aktifAkis = null;
        }

        cevapBekleniyor = false;

        gorunum.SonucGoster(baslik, detay);
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
}
