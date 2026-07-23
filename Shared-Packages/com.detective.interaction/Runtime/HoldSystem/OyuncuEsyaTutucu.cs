using System;
using UnityEngine;

public class OyuncuEsyaTutucu : MonoBehaviour
{
    [Header("Alt Sistemler")]
    [SerializeField] private OyuncuEldeTutma eldeTutma;
    [SerializeField] private EsyaYerlestirici yerlestirici;
    [SerializeField] private EsyaFirlatici firlatici;

    public event Action<ItemData> EsyaDunyayaBirakildi;

    public bool EliBosMu()
    {
        return eldeTutma == null || eldeTutma.EldekiData == null;
    }

    public GameObject AktifEldeTutulanObjeGetir()
    {
        return eldeTutma != null ? eldeTutma.GuncelGorsel : null;
    }

    public void SlotEsyasiniEldeGoster(ItemData itemData)
    {
        if (itemData == null)
        {
            EldekiniGizle();
            return;
        }

        SlotEsyasiniEldeGoster(new ItemInstanceData(itemData));
    }

    public void SlotEsyasiniEldeGoster(ItemInstanceData instanceData)
    {
        EldekiniGizle();

        if (instanceData == null || instanceData.ItemData == null)
            return;

        ItemData itemData = instanceData.ItemData;

        if (itemData.WorldPrefab == null)
        {
            Debug.LogError($"'{itemData.name}' isimli eşyanın WorldPrefab'i atanmamış.");
            return;
        }

        if (eldeTutma != null)
            eldeTutma.GorselOlustur(instanceData);

        // Kritik düzeltme:
        // Her item yerleştirilebilir değildir. Kamera gibi itemlarda preview açılmayacak.
        if (yerlestirici != null)
        {
            if (itemData.YerlestirilebilirMi)
                yerlestirici.PreviewBaslat(itemData);
            else
                yerlestirici.PreviewDurdur();
        }
    }

    public void EldekiniGizle()
    {
        if (eldeTutma != null)
            eldeTutma.Temizle();

        if (yerlestirici != null)
            yerlestirici.PreviewDurdur();
    }

    public void YerlestirmeyiDene()
    {
        if (EliBosMu())
            return;

        if (yerlestirici == null || eldeTutma == null)
            return;

        ItemInstanceData yerlestirilecekInstance = eldeTutma.EldekiInstanceData;

        if (yerlestirilecekInstance == null && eldeTutma.EldekiData != null)
            yerlestirilecekInstance = new ItemInstanceData(eldeTutma.EldekiData);

        if (yerlestirilecekInstance == null || yerlestirilecekInstance.ItemData == null)
            return;

        ItemData itemData = yerlestirilecekInstance.ItemData;

        // Kritik düzeltme:
        // Kamera gibi yerleştirilemeyen itemlarda sol tık / yerleştirme komutu hiçbir şey yapmayacak.
        if (!itemData.YerlestirilebilirMi)
            return;

        bool basarili = yerlestirici.SahneyeSpawnla(yerlestirilecekInstance);

        if (basarili)
        {
            ItemData birakilanItemData = yerlestirilecekInstance.ItemData;

            EldekiniGizle();

            EsyaDunyayaBirakildi?.Invoke(birakilanItemData);
        }
    }

    public void SistemiGuncelle()
    {
        if (yerlestirici != null)
            yerlestirici.PreviewGuncelle();
    }

    public void PreviewDurdur()
    {
        if (yerlestirici != null)
            yerlestirici.PreviewDurdur();
    }

    public void ItemDatayiDunyayaAt(ItemData itemData)
    {
        if (itemData == null)
            return;

        ItemInstanceDunyayaAt(new ItemInstanceData(itemData));
    }

    public void ItemInstanceDunyayaAt(ItemInstanceData instanceData)
    {
        if (instanceData == null || instanceData.ItemData == null)
            return;

        if (firlatici == null)
        {
            Debug.LogWarning("EsyaFirlatici referansı OyuncuEsyaTutucu'ya atanmamış.");
            return;
        }

        bool eldekiyleAyniRuntimeInstance =
            eldeTutma != null &&
            eldeTutma.EldekiInstanceData != null &&
            eldeTutma.EldekiInstanceData == instanceData;

        bool normalItemOlarakAyni =
            eldeTutma != null &&
            !instanceData.RuntimeVerisiVarMi &&
            eldeTutma.EldekiData == instanceData.ItemData;

        if (!EliBosMu() && (eldekiyleAyniRuntimeInstance || normalItemOlarakAyni))
        {
            EldekiniGizle();
        }

        firlatici.Firlat(instanceData);
    }
}