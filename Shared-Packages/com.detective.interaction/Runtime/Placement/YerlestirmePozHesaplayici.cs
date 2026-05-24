using UnityEngine;

[System.Serializable]
public class YerlestirmePozHesaplayici
{
    [Header("Referanslar")]
    [SerializeField] private Camera oyuncuKamerasi;

    [Header("Yerleştirme Ayarları")]
    [SerializeField] private LayerMask yerlestirmeKatmani = ~0;
    [SerializeField] private float yerlestirmeMesafesi = 8f;
    [SerializeField] private float yuzeyOffseti = 0.002f;

    [Header("Yüzey Kısıtı")]
    [Tooltip("1'e yaklaştıkça daha düz/yatay yüzey ister. 0.1 iyi başlangıçtır.")]
    [SerializeField] private float minimumYuzeyYukariDot = 0.1f;

    [Header("Model Düzeltme")]
    [SerializeField] private Vector3 ekEulerRotasyon = Vector3.zero;

    public Collider SonTemasCollideri { get; private set; }

    public void KameraAyarla(Camera kamera)
    {
        oyuncuKamerasi = kamera;
    }

    public PlacementPose PozHesapla(
        Transform ghostTransform,
        GameObject ghostObje,
        float birikmisYRotasyonu,
        float ekYukseklik)
    {
        if (oyuncuKamerasi == null || ghostTransform == null || ghostObje == null)
            return new PlacementPose(false, false, Vector3.zero, Quaternion.identity);

        Vector3 rayBaslangic = oyuncuKamerasi.transform.position + (oyuncuKamerasi.transform.forward * 0.5f);
        Ray ray = new Ray(rayBaslangic, oyuncuKamerasi.transform.forward);

        Debug.DrawRay(rayBaslangic, ray.direction * yerlestirmeMesafesi, Color.red);

        if (!Physics.Raycast(ray, out RaycastHit hit, yerlestirmeMesafesi, yerlestirmeKatmani, QueryTriggerInteraction.Ignore))
        {
            Debug.LogWarning($"Ray hiçbir şeye çarpmadı. Mask: {yerlestirmeKatmani.value}");
            SonTemasCollideri = null;
            return new PlacementPose(false, false, Vector3.zero, Quaternion.identity);
        }

        Debug.Log($"Ray çarptı: {hit.collider.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
        SonTemasCollideri = hit.collider;

        Vector3 yuzeyNormali = hit.normal.normalized;

        float yuzeyDot = Vector3.Dot(yuzeyNormali, Vector3.up);
        bool yuzeyUygunMu = yuzeyDot >= minimumYuzeyYukariDot;

        Vector3 duzlemIciIleriYon = Vector3.ProjectOnPlane(oyuncuKamerasi.transform.forward, yuzeyNormali);

        if (duzlemIciIleriYon.sqrMagnitude < 0.0001f)
        {
            duzlemIciIleriYon = Vector3.ProjectOnPlane(oyuncuKamerasi.transform.right, yuzeyNormali);
        }

        if (duzlemIciIleriYon.sqrMagnitude < 0.0001f)
        {
            duzlemIciIleriYon = Vector3.forward;
        }

        duzlemIciIleriYon.Normalize();

        Quaternion yuzeyRotasyonu = Quaternion.LookRotation(duzlemIciIleriYon, yuzeyNormali);
        Quaternion kullaniciRotasyonu = Quaternion.AngleAxis(birikmisYRotasyonu, yuzeyNormali);
        Quaternion modelDuzeltmeRotasyonu = Quaternion.Euler(ekEulerRotasyon);

        Quaternion nihaiRotasyon = yuzeyRotasyonu * kullaniciRotasyonu * modelDuzeltmeRotasyonu;

        Vector3 geciciPozisyon = hit.point + yuzeyNormali * yuzeyOffseti;

        ghostTransform.position = geciciPozisyon;
        ghostTransform.rotation = nihaiRotasyon;

        float tabanOffseti = GhostTabanOffsetiniHesapla(ghostObje, hit.point, yuzeyNormali);

        Vector3 nihaiPozisyon =
            geciciPozisyon +
            yuzeyNormali * tabanOffseti +
            yuzeyNormali * ekYukseklik;

        return new PlacementPose(true, yuzeyUygunMu, nihaiPozisyon, nihaiRotasyon);
    }

    private float GhostTabanOffsetiniHesapla(GameObject ghostObje, Vector3 temasNoktasi, Vector3 yuzeyNormali)
    {
        Renderer[] rendererlar = ghostObje.GetComponentsInChildren<Renderer>(true);
        if (rendererlar == null || rendererlar.Length == 0)
            return 0f;

        float enAltProjeksiyon = float.PositiveInfinity;

        for (int i = 0; i < rendererlar.Length; i++)
        {
            Bounds b = rendererlar[i].bounds;
            Vector3[] koseler = BoundsKoseNoktalariGetir(b);

            for (int j = 0; j < koseler.Length; j++)
            {
                float projeksiyon = Vector3.Dot(koseler[j] - temasNoktasi, yuzeyNormali);
                if (projeksiyon < enAltProjeksiyon)
                    enAltProjeksiyon = projeksiyon;
            }
        }

        if (enAltProjeksiyon < 0f)
            return -enAltProjeksiyon;

        return 0f;
    }

    private Vector3[] BoundsKoseNoktalariGetir(Bounds b)
    {
        Vector3 min = b.min;
        Vector3 max = b.max;

        return new Vector3[]
        {
            new Vector3(min.x, min.y, min.z),
            new Vector3(min.x, min.y, max.z),
            new Vector3(min.x, max.y, min.z),
            new Vector3(min.x, max.y, max.z),
            new Vector3(max.x, min.y, min.z),
            new Vector3(max.x, min.y, max.z),
            new Vector3(max.x, max.y, min.z),
            new Vector3(max.x, max.y, max.z)
        };
    }
}