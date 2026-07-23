/// <summary>
/// DNA mini oyununun sonuç ekranlarında gösterilen sabit anlatım metinleri. Akış (coroutine)
/// kodunun okunabilirliğini bozmasın diye uzun Türkçe metin blokları buraya toplandı.
/// </summary>
public static class DNAAnaliziMetinleri
{
    public const string AnalizBaslatilamadiBaslik = "ANALİZ BAŞLATILAMADI";

    public const string OlayYeriDNAsiYok =
        "Olay yerinden alınmış DNA örneği bulunamadı.\n\nÖnce cesetten veya olay yerinden DNA örneği alınmalıdır.";

    public const string SupheliDNAsiYok =
        "Şüphelilerden alınmış DNA örneği bulunamadı.\n\nEn az bir şüpheliden DNA örneği alınmalıdır.";

    public const string EslesmeBulunamadiBaslik = "DNA EŞLEŞMESİ BULUNAMADI";

    public const string EslesmeBulunamadiDetay =
        "Olay yeri DNA’sı, alınan şüpheli DNA örneklerinin hiçbiriyle eşleşmedi.\n\n" +
        "Sonuç: Mevcut DNA örnekleri katili doğrulamak için yeterli değildir.";

    public const string RaporOlusturulduBaslik = "DNA RAPORU OLUŞTURULDU";

    public static string RaporOlusturulduDetay(string olayYeriAdi, string supheliAdi)
    {
        return "Olay yeri DNA’sı ile " + supheliAdi + " DNA’sı eşleşti olarak rapora işlendi.\n\n" +
            "Olay yeri DNA’sı: " + olayYeriAdi + "\n" +
            "Raporlanan şüpheli: " + supheliAdi + "\n\n" +
            "Sonuç: Bu eşleşme soruşturma dosyasına delil olarak eklendi.";
    }

    public const string RaporTamamlandiBaslik = "DNA RAPORU TAMAMLANDI";

    public static string RaporTamamlandiDetay(string olayYeriAdi)
    {
        return "Olay yeri DNA’sı ile mevcut şüpheli DNA örnekleri arasında eşleşme raporlanmadı.\n\n" +
            "Olay yeri DNA’sı: " + olayYeriAdi + "\n\n" +
            "Sonuç: Yeni şüpheli DNA örnekleri toplanabilir veya mevcut soruşturma farklı delillerle sürdürülebilir.";
    }

    public static string OlayYeriIncelemeDurumu(int kalanSaniye)
    {
        return "Olay yerinde bulunan DNA inceleniyor... " + kalanSaniye;
    }

    public static string SupheliIncelemeDurumu(string supheliAdi, int kalanSaniye)
    {
        return supheliAdi + " DNA'sı inceleniyor... " + kalanSaniye;
    }

    public static string EslesmeSorusu(string supheliAdi)
    {
        return supheliAdi + " DNA’sı olay yerinde bulunan DNA ile eşleşiyor mu?";
    }

    public const string SupheliGecisDurumu = "Bu DNA eşleşmedi. Sıradaki şüpheli DNA hazırlanıyor...";
}
