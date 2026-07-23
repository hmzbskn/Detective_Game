/// <summary>
/// "Aynı highlight'ı tekrar tekrar açıp kapatma, sadece hedef değiştiğinde geçiş yap" mantığını tek
/// yerde toplar. Önceden OyuncuEtkilesim bunu kendi HighlightGuncelle metoduyla, OyuncuDNAToplamaKontrolcusu
/// ise DNAToplamaNoktasi'nin somut tipini elle karşılaştırarak (bakilanNokta != yeniNokta) ayrı ayrı
/// uyguluyordu.
/// </summary>
public class EtkilesimHighlightYoneticisi
{
    private IHighlightable aktifHighlight;

    public void Guncelle(IHighlightable yeniHighlight)
    {
        if (yeniHighlight == aktifHighlight)
            return;

        if (aktifHighlight != null)
            aktifHighlight.HighlightKapat();

        aktifHighlight = yeniHighlight;

        if (aktifHighlight != null)
            aktifHighlight.HighlightAc();
    }

    public void Temizle()
    {
        Guncelle(null);
    }
}
