using System.Collections.Generic;
using UnityEngine;

public class EvidenceManager : MonoBehaviour
{
    public static EvidenceManager Instance { get; private set; }

    private readonly HashSet<ItemData> sahipOlunanDeliller = new HashSet<ItemData>();

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

    public void Cikar(ItemData item)
    {
        if (item == null)
            return;

        sahipOlunanDeliller.Remove(item);
    }
}