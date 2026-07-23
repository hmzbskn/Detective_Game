using System.Collections.Generic;
using UnityEngine;

public class OyuncuEldeTutma : MonoBehaviour
{
    [SerializeField] private Transform tutmaNoktasi;

    private const int IGNORE_RAYCAST_LAYER = 2;

    public ItemData EldekiData { get; private set; }
    public ItemInstanceData EldekiInstanceData { get; private set; }
    public GameObject GuncelGorsel { get; private set; }

    private Dictionary<Transform, int> orijinalLayerlar = new Dictionary<Transform, int>();

    public void GorselOlustur(ItemData data)
    {
        if (data == null)
            return;

        GorselOlustur(new ItemInstanceData(data));
    }

    public void GorselOlustur(ItemInstanceData instanceData)
    {
        if (instanceData == null || instanceData.ItemData == null)
            return;

        ItemData data = instanceData.ItemData;

        if (data.WorldPrefab == null)
            return;

        orijinalLayerlar.Clear();

        EldekiData = data;
        EldekiInstanceData = instanceData;

        GuncelGorsel = Instantiate(data.WorldPrefab, tutmaNoktasi);
        GuncelGorsel.name = data.WorldPrefab.name + "_VisualDummy";

        FizikleriUyut(GuncelGorsel);

        GuncelGorsel.transform.localPosition = Vector3.zero;
        GuncelGorsel.transform.localRotation = Quaternion.identity;
        GuncelGorsel.transform.localScale = data.WorldPrefab.transform.localScale;

        LayerDegistirVeSaklaRekursif(GuncelGorsel.transform, IGNORE_RAYCAST_LAYER);

        EldeTutulabilirObje tutulabilir = GuncelGorsel.GetComponent<EldeTutulabilirObje>();
        if (tutulabilir != null)
        {
            tutulabilir.ItemInstanceDataAyarla(instanceData);
            tutulabilir.TutulmaDurumunuAyarla(true);
        }

        FotografBaskiObjesi baskiObjesi = GuncelGorsel.GetComponentInChildren<FotografBaskiObjesi>(true);
        if (baskiObjesi != null && instanceData.FotografKaydi != null)
        {
            baskiObjesi.FotografAta(instanceData.FotografKaydi);
        }
    }

    private void LayerDegistirVeSaklaRekursif(Transform parent, int yeniLayer)
    {
        orijinalLayerlar[parent] = parent.gameObject.layer;
        parent.gameObject.layer = yeniLayer;

        foreach (Transform child in parent)
        {
            LayerDegistirVeSaklaRekursif(child, yeniLayer);
        }
    }

    private void LayerlariGeriYukle()
    {
        foreach (var kvp in orijinalLayerlar)
        {
            if (kvp.Key != null)
                kvp.Key.gameObject.layer = kvp.Value;
        }

        orijinalLayerlar.Clear();
    }

    public void Temizle()
    {
        if (GuncelGorsel != null)
        {
            LayerlariGeriYukle();
            Destroy(GuncelGorsel);
        }

        EldekiData = null;
        EldekiInstanceData = null;
        GuncelGorsel = null;
    }

    private void FizikleriUyut(GameObject obje)
    {
        Rigidbody rb = obje.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (var col in obje.GetComponentsInChildren<Collider>(true))
        {
            col.enabled = false;
        }
    }
}