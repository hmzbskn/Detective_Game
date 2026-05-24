using UnityEngine;

public class YerlestirmePreviewSistemi : MonoBehaviour
{
    [Header("Alt Sistemler")]
    [SerializeField] private YerlestirmeGirdiDenetleyici girdiDenetleyici;
    [SerializeField] private YerlestirmePozHesaplayici pozHesaplayici;
    [SerializeField] private YerlestirmeCakismaDenetleyici cakismaDenetleyici;
    [SerializeField] private GhostGorsellestirici ghostGorsellestirici;

    private GameObject kaynakObje;
    private Collider[] kaynakColliderlar;

    private void Awake()
    {
        if (pozHesaplayici != null)
            pozHesaplayici.KameraAyarla(Camera.main);
    }

    public void PreviewBaslat(GameObject kaynakPrefab)
    {
        if (kaynakPrefab == null || ghostGorsellestirici == null)
            return;

        PreviewDurdur();

        kaynakObje = kaynakPrefab;
        kaynakColliderlar = kaynakPrefab.GetComponentsInChildren<Collider>(true);

        if (girdiDenetleyici != null)
            girdiDenetleyici.Sifirla();

        ghostGorsellestirici.GhostOlustur(kaynakPrefab);
    }

    public void PreviewGuncelle()
    {
        if (kaynakObje == null || ghostGorsellestirici.AktifGhostObje == null)
            return;

        if (girdiDenetleyici != null)
            girdiDenetleyici.GirdiyiIsle();

        PlacementPose pose = GuncelPoseGetir();
        ghostGorsellestirici.GhostuGoster(pose);
    }

    public PlacementPose GuncelPoseGetir()
    {
        if (kaynakObje == null || ghostGorsellestirici.AktifGhostObje == null || pozHesaplayici == null)
            return new PlacementPose(false, false, Vector3.zero, Quaternion.identity);

        Transform ghostTransform = ghostGorsellestirici.AktifGhostTransform;
        GameObject ghostObje = ghostGorsellestirici.AktifGhostObje;

        PlacementPose pose = pozHesaplayici.PozHesapla(
            ghostTransform,
            ghostObje,
            girdiDenetleyici != null ? girdiDenetleyici.BirikmisYRotasyonu : 0f,
            girdiDenetleyici != null ? girdiDenetleyici.EkYukseklik : 0f
        );

        if (!pose.YuzeyBulunduMu) return pose;

        bool cakismaVar = cakismaDenetleyici != null &&
                          cakismaDenetleyici.CakismaVarMi(
                              kaynakObje,
                              kaynakColliderlar,
                              pozHesaplayici.SonTemasCollideri,
                              pose.Pozisyon,
                              pose.Rotasyon
                          );

        pose.KonulabilirMi = pose.KonulabilirMi && !cakismaVar;
        return pose;
    }

    public void PreviewDurdur()
    {
        kaynakObje = null;
        kaynakColliderlar = null;

        if (girdiDenetleyici != null)
            girdiDenetleyici.Sifirla();

        if (ghostGorsellestirici != null)
            ghostGorsellestirici.GhostYokEt();
    }
}