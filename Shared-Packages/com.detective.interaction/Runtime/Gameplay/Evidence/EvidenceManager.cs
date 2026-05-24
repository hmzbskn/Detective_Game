using System.Collections.Generic;
using UnityEngine;

public class EvidenceManager : MonoBehaviour
{
    public static EvidenceManager Instance { get; private set; }

    private readonly HashSet<ItemData> sahipOlunanDeliller = new HashSet<ItemData>();
    private readonly Dictionary<ItemData, DNAData> delilDNAEslesmeleri = new Dictionary<ItemData, DNAData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool VarMi(ItemData item)
    {
        if (item == null)
            return false;

        return sahipOlunanDeliller.Contains(item);
    }

    public void Ekle(ItemData item)
    {
        if (item == null)
            return;

        if (!item.DelilMi)
            return;

        sahipOlunanDeliller.Add(item);
    }

    public void DNADeliliEkle(ItemData item, DNAData dnaData)
    {
        if (item == null)
            return;

        if (!item.DelilMi)
            return;

        sahipOlunanDeliller.Add(item);

        if (dnaData != null)
        {
            delilDNAEslesmeleri[item] = dnaData;
            Debug.Log($"DNA delili kaydedildi: {item.ItemAdi} -> {dnaData.dnaID}");
        }
        else
        {
            Debug.LogWarning($"{item.ItemAdi} delili eklendi ama DNAData boş.");
        }
    }

    public bool DNADatasiVarMi(ItemData item)
    {
        if (item == null)
            return false;

        return delilDNAEslesmeleri.ContainsKey(item) && delilDNAEslesmeleri[item] != null;
    }

    public bool TryDNADatasiGetir(ItemData item, out DNAData dnaData)
    {
        dnaData = null;

        if (item == null)
            return false;

        return delilDNAEslesmeleri.TryGetValue(item, out dnaData) && dnaData != null;
    }

    public DNAData DNADatasiGetir(ItemData item)
    {
        if (item == null)
            return null;

        if (delilDNAEslesmeleri.TryGetValue(item, out DNAData dnaData))
            return dnaData;

        return null;
    }

    public void Cikar(ItemData item)
    {
        if (item == null)
            return;

        sahipOlunanDeliller.Remove(item);

        if (delilDNAEslesmeleri.ContainsKey(item))
            delilDNAEslesmeleri.Remove(item);
    }
}