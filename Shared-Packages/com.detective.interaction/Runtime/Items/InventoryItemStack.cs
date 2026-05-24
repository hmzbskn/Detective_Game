using System;
using UnityEngine;

[Serializable]
public class InventoryItemStack
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int adet;
    [SerializeField] private ItemInstanceData instanceData;

    public ItemData ItemData => itemData;
    public int Adet => adet;
    public ItemInstanceData InstanceData => instanceData;

    public bool RuntimeVerisiVarMi => instanceData != null && instanceData.RuntimeVerisiVarMi;
    public bool FotografVerisiVarMi => instanceData != null && instanceData.FotografVerisiVarMi;

    public bool BosMu()
    {
        return itemData == null || adet <= 0;
    }

    public void Ayarla(ItemData yeniItemData, int yeniAdet)
    {
        itemData = yeniItemData;
        instanceData = null;

        if (itemData == null)
        {
            adet = 0;
            return;
        }

        adet = Mathf.Max(1, yeniAdet);

        if (itemData.StacklenebilirMi)
            adet = Mathf.Min(adet, itemData.MaxStack);
        else
            adet = 1;
    }

    public void Ayarla(ItemInstanceData yeniInstanceData)
    {
        instanceData = yeniInstanceData;

        if (instanceData == null || instanceData.ItemData == null)
        {
            Temizle();
            return;
        }

        itemData = instanceData.ItemData;

        // Runtime veri taşıyan itemlar stacklenmez.
        adet = 1;
    }

    public void Temizle()
    {
        itemData = null;
        adet = 0;
        instanceData = null;
    }

    public bool AyniItemMi(ItemData digerItemData)
    {
        if (itemData == null || digerItemData == null)
            return false;

        return itemData == digerItemData ||
               itemData.ItemID == digerItemData.ItemID;
    }

    public bool EklenebilirMi(ItemData eklenecekItemData)
    {
        if (eklenecekItemData == null)
            return false;

        if (BosMu())
            return true;

        // Fotoğraf gibi runtime veri taşıyan slotlar stacklenmez.
        if (RuntimeVerisiVarMi)
            return false;

        if (!AyniItemMi(eklenecekItemData))
            return false;

        if (!itemData.StacklenebilirMi)
            return false;

        return adet < itemData.MaxStack;
    }

    public bool BirAdetEkle(ItemData eklenecekItemData)
    {
        if (eklenecekItemData == null)
            return false;

        if (RuntimeVerisiVarMi)
            return false;

        if (BosMu())
        {
            Ayarla(eklenecekItemData, 1);
            return true;
        }

        if (!AyniItemMi(eklenecekItemData))
            return false;

        if (!itemData.StacklenebilirMi)
            return false;

        if (adet >= itemData.MaxStack)
            return false;

        adet++;
        return true;
    }

    public bool BirAdetAzalt()
    {
        if (BosMu())
            return false;

        adet--;

        if (adet <= 0)
            Temizle();

        return true;
    }

    public Sprite IkonGetir()
    {
        if (BosMu())
            return null;

        if (instanceData != null)
        {
            Sprite runtimeIkon = instanceData.IkonGetir();
            if (runtimeIkon != null)
                return runtimeIkon;
        }

        return itemData != null ? itemData.HotbarIkonu : null;
    }

    public ItemInstanceData TekAdetlikInstanceOlustur()
    {
        if (BosMu())
            return null;

        if (instanceData != null)
            return instanceData;

        return new ItemInstanceData(itemData);
    }
}