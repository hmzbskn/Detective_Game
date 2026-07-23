using UnityEngine;

public class EldeTutulabilirObje : EtkilesebilirObje, IIncelenebilir
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("Runtime Instance Data")]
    [SerializeField] private ItemInstanceData instanceData;

    private bool tutuluyorMu = false;

    private void Awake()
    {
        ItemBilgileriniGuncelle();
        RuntimeGorseliUygula();
    }

    private void Start()
    {
        ItemBilgileriniGuncelle();
        RuntimeGorseliUygula();
    }

    public ItemData ItemDataGetir()
    {
        if (GecerliRuntimeInstanceVarMi())
            return instanceData.ItemData;

        return itemData;
    }

    public ItemInstanceData ItemInstanceDataGetir()
    {
        if (GecerliRuntimeInstanceVarMi())
            return instanceData;

        if (itemData == null)
            return null;

        return new ItemInstanceData(itemData);
    }

    public void ItemDataAyarla(ItemData yeniItemData)
    {
        itemData = yeniItemData;
        instanceData = null;

        ItemBilgileriniGuncelle();
    }

    public void ItemInstanceDataAyarla(ItemInstanceData yeniInstanceData)
    {
        instanceData = yeniInstanceData;

        if (GecerliRuntimeInstanceVarMi())
        {
            itemData = instanceData.ItemData;
        }
        else
        {
            instanceData = null;
        }

        ItemBilgileriniGuncelle();
        RuntimeGorseliUygula();
    }

    private bool GecerliRuntimeInstanceVarMi()
    {
        return instanceData != null && instanceData.ItemData != null;
    }

    private void ItemBilgileriniGuncelle()
    {
        ItemData aktifItemData = ItemDataGetir();

        if (aktifItemData == null)
            return;

        if (!string.IsNullOrWhiteSpace(aktifItemData.ItemAdi))
            objeAdi = aktifItemData.ItemAdi;

        if (string.IsNullOrWhiteSpace(etkilesimMetni))
            etkilesimMetni = "Al";
    }

    private void RuntimeGorseliUygula()
    {
        if (!GecerliRuntimeInstanceVarMi())
            return;

        if (instanceData.FotografKaydi == null)
            return;

        FotografBaskiObjesi baskiObjesi = GetComponentInChildren<FotografBaskiObjesi>(true);

        if (baskiObjesi != null)
            baskiObjesi.FotografAta(instanceData.FotografKaydi);
    }

    public override string ObjeAdiGetir()
    {
        ItemData aktifItemData = ItemDataGetir();

        if (aktifItemData != null && !string.IsNullOrWhiteSpace(aktifItemData.ItemAdi))
            return aktifItemData.ItemAdi;

        return objeAdi;
    }

    public override string EtkilesimMetniGetir()
    {
        if (string.IsNullOrWhiteSpace(etkilesimMetni))
            return "Al";

        return etkilesimMetni;
    }

    public string ItemIDGetir()
    {
        ItemData aktifItemData = ItemDataGetir();

        if (aktifItemData == null)
            return string.Empty;

        return aktifItemData.ItemID;
    }

    public string ItemAdiGetir()
    {
        ItemData aktifItemData = ItemDataGetir();

        if (aktifItemData == null)
            return objeAdi;

        return string.IsNullOrWhiteSpace(aktifItemData.ItemAdi)
            ? objeAdi
            : aktifItemData.ItemAdi;
    }

    public Sprite HotbarIkonuGetir()
    {
        if (GecerliRuntimeInstanceVarMi())
        {
            Sprite runtimeIkon = instanceData.IkonGetir();

            if (runtimeIkon != null)
                return runtimeIkon;
        }

        ItemData aktifItemData = ItemDataGetir();

        if (aktifItemData == null)
            return null;

        return aktifItemData.HotbarIkonu;
    }

    public GameObject WorldPrefabGetir()
    {
        ItemData aktifItemData = ItemDataGetir();

        if (aktifItemData == null)
            return null;

        return aktifItemData.WorldPrefab;
    }

    public bool StacklenebilirMi()
    {
        ItemData aktifItemData = ItemDataGetir();

        return aktifItemData != null &&
               aktifItemData.StacklenebilirMi &&
               !GecerliRuntimeInstanceVarMi();
    }

    public int MaxStackGetir()
    {
        ItemData aktifItemData = ItemDataGetir();

        if (aktifItemData == null)
            return 1;

        return aktifItemData.MaxStack;
    }

    public override void Etkilesim()
    {
        // OyuncuEtkilesim sistemi pickup işlemini yönetiyor.
    }

    public bool TutuluyorMu()
    {
        return tutuluyorMu;
    }

    public void TutulmaDurumunuAyarla(bool yeniDurum)
    {
        tutuluyorMu = yeniDurum;
    }

    public bool GecerliItemDataVarMi()
    {
        return ItemDataGetir() != null;
    }

    public bool IncelenebilirMi()
    {
        ItemData aktifItemData = ItemDataGetir();

        return aktifItemData != null && aktifItemData.DelilMi;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ItemBilgileriniGuncelle();
    }
#endif
}