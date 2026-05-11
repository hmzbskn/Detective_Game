using System;
using UnityEngine;

[Serializable]
public class ItemInstanceData
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private FotografKaydi fotografKaydi;

    public ItemData ItemData => itemData;
    public FotografKaydi FotografKaydi => fotografKaydi;

    public bool RuntimeVerisiVarMi => fotografKaydi != null;
    public bool FotografVerisiVarMi => fotografKaydi != null;

    public ItemInstanceData(ItemData itemData)
    {
        this.itemData = itemData;
        fotografKaydi = null;
    }

    public ItemInstanceData(ItemData itemData, FotografKaydi fotografKaydi)
    {
        this.itemData = itemData;
        this.fotografKaydi = fotografKaydi;
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