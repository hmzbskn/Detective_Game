/// <summary>
/// Hotbar ve envanter slot dizilerinin ham depolama + arama/birleştirme mantığı. Seçim durumu,
/// UI senkronizasyonu veya elde-gösterim gibi üst düzey orkestrasyonu bilmez — HotbarSistemi bu
/// sınıfı kullanarak boş slot arama ve stack birleştirme mantığını tek yerde uygular (önceden
/// HotbarSistemi içinde üç ayrı metotta (EsyayiHotbaraEkleVeSec, ItemEkle, ItemInstanceEkle)
/// neredeyse birebir kopyalanmıştı).
/// </summary>
public class EnvanterDeposu
{
    public InventoryItemStack[] HotbarSlotlari { get; }
    public InventoryItemStack[] EnvanterSlotlari { get; }

    public EnvanterDeposu(int hotbarAdet, int envanterAdet)
    {
        HotbarSlotlari = YeniStackDizisiOlustur(hotbarAdet);
        EnvanterSlotlari = YeniStackDizisiOlustur(envanterAdet);
    }

    public InventoryItemStack GlobalStackGetir(int globalIndex)
    {
        if (SlotAdresleme.EnvanterIndexiMi(globalIndex))
            return EnvanterSlotlari[globalIndex];

        if (SlotAdresleme.HotbarIndexiMi(globalIndex))
            return HotbarSlotlari[SlotAdresleme.HotbarLocalIndex(globalIndex)];

        return null;
    }

    public void GlobalStackAta(int globalIndex, InventoryItemStack stack)
    {
        if (SlotAdresleme.EnvanterIndexiMi(globalIndex))
        {
            EnvanterSlotlari[globalIndex] = stack;
            return;
        }

        if (SlotAdresleme.HotbarIndexiMi(globalIndex))
            HotbarSlotlari[SlotAdresleme.HotbarLocalIndex(globalIndex)] = stack;
    }

    public int IlkBosHotbarBul()
    {
        for (int i = 0; i < HotbarSlotlari.Length; i++)
        {
            if (HotbarSlotlari[i].BosMu())
                return i;
        }

        return -1;
    }

    public int IlkBosEnvanterBul()
    {
        for (int i = 0; i < EnvanterSlotlari.Length; i++)
        {
            if (EnvanterSlotlari[i].BosMu())
                return i;
        }

        return -1;
    }

    public int StacklenebilirSlotBul(InventoryItemStack[] slotlar, ItemData itemData)
    {
        for (int i = 0; i < slotlar.Length; i++)
        {
            if (slotlar[i].EklenebilirMi(itemData))
                return i;
        }

        return -1;
    }

    public bool StackleriBirlestir(InventoryItemStack kaynakStack, InventoryItemStack hedefStack)
    {
        if (kaynakStack == null || hedefStack == null)
            return false;

        if (kaynakStack.BosMu() || hedefStack.BosMu())
            return false;

        if (kaynakStack.RuntimeVerisiVarMi || hedefStack.RuntimeVerisiVarMi)
            return false;

        if (!kaynakStack.AyniItemMi(hedefStack.ItemData))
            return false;

        if (!hedefStack.ItemData.StacklenebilirMi)
            return false;

        int maxStack = hedefStack.ItemData.MaxStack;
        int hedefAdet = hedefStack.Adet;
        int kaynakAdet = kaynakStack.Adet;

        if (hedefAdet >= maxStack)
            return false;

        int bosYer = maxStack - hedefAdet;
        int aktarilacakMiktar = kaynakAdet < bosYer ? kaynakAdet : bosYer;

        if (aktarilacakMiktar <= 0)
            return false;

        hedefStack.Ayarla(hedefStack.ItemData, hedefAdet + aktarilacakMiktar);

        int kalanKaynak = kaynakAdet - aktarilacakMiktar;
        if (kalanKaynak <= 0)
            kaynakStack.Temizle();
        else
            kaynakStack.Ayarla(kaynakStack.ItemData, kalanKaynak);

        return true;
    }

    private static InventoryItemStack[] YeniStackDizisiOlustur(int adet)
    {
        InventoryItemStack[] dizi = new InventoryItemStack[adet];

        for (int i = 0; i < adet; i++)
            dizi[i] = new InventoryItemStack();

        return dizi;
    }
}
