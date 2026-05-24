using UnityEngine;

public class EsyaYerlestirici : MonoBehaviour
{
    [SerializeField] private YerlestirmePreviewSistemi previewSistemi;

    public void PreviewBaslat(ItemData data)
    {
        if (data == null || data.WorldPrefab == null || previewSistemi == null)
            return;

        previewSistemi.PreviewBaslat(data.WorldPrefab);
    }

    public void PreviewDurdur()
    {
        if (previewSistemi != null)
            previewSistemi.PreviewDurdur();
    }

    public void PreviewGuncelle()
    {
        if (previewSistemi != null)
            previewSistemi.PreviewGuncelle();
    }

    public bool SahneyeSpawnla(ItemData data)
    {
        if (data == null)
            return false;

        return SahneyeSpawnla(new ItemInstanceData(data));
    }

    public bool SahneyeSpawnla(ItemInstanceData instanceData)
    {
        if (instanceData == null || instanceData.ItemData == null)
            return false;

        ItemData data = instanceData.ItemData;

        if (data.WorldPrefab == null || previewSistemi == null)
            return false;

        PlacementPose pose = previewSistemi.GuncelPoseGetir();

        if (!pose.YuzeyBulunduMu || !pose.KonulabilirMi)
        {
            Debug.LogWarning("Buraya koyulamaz: " + (!pose.YuzeyBulunduMu ? "Yüzey yok." : "Çakışma var veya yüzey eğimli."));
            return false;
        }

        GameObject yeniObje = Instantiate(data.WorldPrefab, pose.Pozisyon, pose.Rotasyon);
        yeniObje.name = data.WorldPrefab.name + "_Placed";

        Rigidbody rb = yeniObje.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        EldeTutulabilirObje tutulabilir = yeniObje.GetComponent<EldeTutulabilirObje>();
        if (tutulabilir != null)
        {
            tutulabilir.ItemInstanceDataAyarla(instanceData);
            tutulabilir.TutulmaDurumunuAyarla(false);
        }

        FotografBaskiObjesi baskiObjesi = yeniObje.GetComponentInChildren<FotografBaskiObjesi>(true);
        if (baskiObjesi != null && instanceData.FotografKaydi != null)
        {
            baskiObjesi.FotografAta(instanceData.FotografKaydi);
        }

        return true;
    }
}