/// <summary>
/// "Boş fotoğraf kağıdını tüket, fotoğrafı çek, basılı fotoğrafı envantere ekle; envantere
/// eklenemezse kağıdı geri ver" işleminin tek seferlik uygulanması. FotografMakinesiController bu
/// işlemi yalnızca tetikler (sol tık girdisi) ve sonucu loglar; adımların sırası ve rollback mantığı
/// burada toplanır.
/// </summary>
public class FotografCekimTransaksiyonu
{
    public enum Sonuc
    {
        Basarili,
        KagitYok,
        CekimBasarisiz,
        KagitTuketilemedi,
        EnvantereEklenemedi
    }

    private readonly HotbarSistemi hotbarSistemi;
    private readonly FotografCekimSistemi fotografCekimSistemi;
    private readonly ItemData bosFotografKagidiItem;
    private readonly ItemData basiliFotografItem;

    public bool KagitRollbackBasarisizMi { get; private set; }

    public FotografCekimTransaksiyonu(
        HotbarSistemi hotbarSistemi,
        FotografCekimSistemi fotografCekimSistemi,
        ItemData bosFotografKagidiItem,
        ItemData basiliFotografItem)
    {
        this.hotbarSistemi = hotbarSistemi;
        this.fotografCekimSistemi = fotografCekimSistemi;
        this.bosFotografKagidiItem = bosFotografKagidiItem;
        this.basiliFotografItem = basiliFotografItem;
    }

    public Sonuc Uygula()
    {
        if (!hotbarSistemi.ItemVarMi(bosFotografKagidiItem))
            return Sonuc.KagitYok;

        FotografKaydi kayit = fotografCekimSistemi.FotografCek();

        if (kayit == null)
            return Sonuc.CekimBasarisiz;

        bool kagitTuketildi = hotbarSistemi.ItemdenBirAdetAzalt(bosFotografKagidiItem);

        if (!kagitTuketildi)
            return Sonuc.KagitTuketilemedi;

        ItemInstanceData basiliFotografInstance = new ItemInstanceData(basiliFotografItem, kayit);

        bool fotografEklendi = hotbarSistemi.ItemInstanceEkle(basiliFotografInstance);

        if (!fotografEklendi)
        {
            bool kagitGeriVerildi = hotbarSistemi.ItemEkle(bosFotografKagidiItem, 1);
            KagitRollbackBasarisizMi = !kagitGeriVerildi;
            return Sonuc.EnvantereEklenemedi;
        }

        return Sonuc.Basarili;
    }
}
