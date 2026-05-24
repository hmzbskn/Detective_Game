using System;
using UnityEngine;

[Serializable]
public class ItemInstanceData
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private FotografKaydi fotografKaydi;
    [SerializeField] private DNAData dnaData;

    public ItemData ItemData => itemData;
    public FotografKaydi FotografKaydi => fotografKaydi;
    public DNAData DNAData => dnaData;

    public bool RuntimeVerisiVarMi => fotografKaydi != null || dnaData != null;
    public bool FotografVerisiVarMi => fotografKaydi != null;
    public bool DNAVerisiVarMi => dnaData != null;

    public ItemInstanceData(ItemData itemData)
    {
        this.itemData = itemData;
        fotografKaydi = null;
        dnaData = null;
    }

    public ItemInstanceData(ItemData itemData, FotografKaydi fotografKaydi)
    {
        this.itemData = itemData;
        this.fotografKaydi = fotografKaydi;
        dnaData = null;
    }

    public ItemInstanceData(ItemData itemData, DNAData dnaData)
    {
        this.itemData = itemData;
        fotografKaydi = null;
        this.dnaData = dnaData;
    }

    public ItemInstanceData(ItemData itemData, FotografKaydi fotografKaydi, DNAData dnaData)
    {
        this.itemData = itemData;
        this.fotografKaydi = fotografKaydi;
        this.dnaData = dnaData;
    }

    public void DNADataAyarla(DNAData yeniDNAData)
    {
        dnaData = yeniDNAData;
    }

    public void FotografKaydiAyarla(FotografKaydi yeniFotografKaydi)
    {
        fotografKaydi = yeniFotografKaydi;
    }

    public Sprite IkonGetir()
    {
        if (fotografKaydi != null && fotografKaydi.FotografSprite != null)
            return fotografKaydi.FotografSprite;

        if (itemData != null)
            return itemData.HotbarIkonu;

        return null;
    }
}