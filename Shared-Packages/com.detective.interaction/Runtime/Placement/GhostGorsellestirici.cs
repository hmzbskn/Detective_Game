using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class GhostGorsellestirici
{
    [Header("Ghost Ayarları")]
    [SerializeField] private Material ghostMateryal;
    [SerializeField] private Color gecerliRenk = Color.green;
    [SerializeField] private Color gecersizRenk = Color.red;

    private GameObject aktifGhostObje;
    private Transform aktifGhostTransform;
    private readonly List<Material> ghostMateryalKopyalari = new();

    public GameObject AktifGhostObje => aktifGhostObje;
    public Transform AktifGhostTransform => aktifGhostTransform;

    public void GhostOlustur(GameObject kaynakObje)
    {
        GhostYokEt();

        if (kaynakObje == null)
            return;

        aktifGhostObje = Object.Instantiate(kaynakObje);
        aktifGhostObje.name = kaynakObje.name + "_GhostPreview";

        // KRİTİK EKSİK BURADAYDI
        aktifGhostTransform = aktifGhostObje.transform;

        int ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");
        aktifGhostObje.layer = ignoreLayer;

        foreach (Transform t in aktifGhostObje.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = ignoreLayer;

        MonoBehaviour[] davranislar = aktifGhostObje.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour davranis in davranislar)
        {
            Object.Destroy(davranis);
        }

        Collider[] colliderlar = aktifGhostObje.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in colliderlar)
        {
            Object.Destroy(col);
        }

        Rigidbody[] rigidbodyler = aktifGhostObje.GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody rb in rigidbodyler)
        {
            Object.Destroy(rb);
        }

        Renderer[] rendererlar = aktifGhostObje.GetComponentsInChildren<Renderer>(true);
        ghostMateryalKopyalari.Clear();

        foreach (Renderer rend in rendererlar)
        {
            Material[] yeniMateryaller = new Material[rend.sharedMaterials.Length];

            for (int i = 0; i < yeniMateryaller.Length; i++)
            {
                Material matKopya = ghostMateryal != null
                    ? new Material(ghostMateryal)
                    : new Material(Shader.Find("Universal Render Pipeline/Lit"));

                yeniMateryaller[i] = matKopya;
                ghostMateryalKopyalari.Add(matKopya);
            }

            rend.materials = yeniMateryaller;
            rend.shadowCastingMode = ShadowCastingMode.Off;
            rend.receiveShadows = false;
        }

        GhostRenginiAyarla(true);
        aktifGhostObje.SetActive(false);
    }

    public void GhostuGoster(PlacementPose pose)
    {
        if (aktifGhostObje == null || aktifGhostTransform == null)
            return;

        if (!pose.YuzeyBulunduMu)
        {
            aktifGhostObje.SetActive(false);
            return;
        }

        aktifGhostObje.SetActive(true);
        aktifGhostTransform.position = pose.Pozisyon;
        aktifGhostTransform.rotation = pose.Rotasyon;
        GhostRenginiAyarla(pose.KonulabilirMi);
    }

    public void GhostuGizle()
    {
        if (aktifGhostObje != null)
            aktifGhostObje.SetActive(false);
    }

    public void GhostYokEt()
    {
        if (aktifGhostObje != null)
        {
            Object.Destroy(aktifGhostObje);
            aktifGhostObje = null;
            aktifGhostTransform = null;
        }

        ghostMateryalKopyalari.Clear();
    }

    private void GhostRenginiAyarla(bool konulabilirMi)
    {
        Color hedefRenk = konulabilirMi ? gecerliRenk : gecersizRenk;

        for (int i = 0; i < ghostMateryalKopyalari.Count; i++)
        {
            Material mat = ghostMateryalKopyalari[i];
            if (mat == null) continue;

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", hedefRenk);

            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", hedefRenk);
        }
    }
}