using UnityEngine;

[System.Serializable]
public class YerlestirmeCakismaDenetleyici
{
    [Header("Çakışma Ayarları")]
    [SerializeField] private LayerMask cakismaKatmani = ~0;
    [SerializeField] private float cakismaPayi = 0.01f;

    public bool CakismaVarMi(
        GameObject kaynakObje,
        Collider[] kaynakColliderlar,
        Collider temasCollideri,
        Vector3 hedefPozisyon,
        Quaternion hedefRotasyon)
    {
        if (kaynakObje == null || kaynakColliderlar == null || kaynakColliderlar.Length == 0)
            return false;

        for (int i = 0; i < kaynakColliderlar.Length; i++)
        {
            Collider col = kaynakColliderlar[i];
            if (col == null)
                continue;

            if (col is BoxCollider box)
            {
                if (BoxColliderCakisiyorMu(kaynakObje, box, temasCollideri, hedefPozisyon, hedefRotasyon))
                    return true;
            }
            else if (col is SphereCollider sphere)
            {
                if (SphereColliderCakisiyorMu(kaynakObje, sphere, temasCollideri, hedefPozisyon, hedefRotasyon))
                    return true;
            }
            else if (col is CapsuleCollider capsule)
            {
                if (CapsuleColliderCakisiyorMu(kaynakObje, capsule, temasCollideri, hedefPozisyon, hedefRotasyon))
                    return true;
            }
            else
            {
                if (FallbackBoundsCakisiyorMu(kaynakObje, col, temasCollideri, hedefPozisyon, hedefRotasyon))
                    return true;
            }
        }

        return false;
    }

    private bool BoxColliderCakisiyorMu(
        GameObject kaynakObje,
        BoxCollider box,
        Collider temasCollideri,
        Vector3 hedefPozisyon,
        Quaternion hedefRotasyon)
    {
        Vector3 merkez = TransformNoktasi(kaynakObje, box.transform, hedefPozisyon, hedefRotasyon, box.center);

        Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, MutlakLossyScale(box.transform));
        halfExtents = Kucult(halfExtents, cakismaPayi);

        Quaternion rot = hedefRotasyon * Quaternion.Inverse(kaynakObje.transform.rotation) * box.transform.rotation;

        Collider[] carpanlar = Physics.OverlapBox(
            merkez,
            halfExtents,
            rot,
            cakismaKatmani,
            QueryTriggerInteraction.Ignore
        );

        return GecerliCakismaVarMi(kaynakObje, temasCollideri, carpanlar);
    }

    private bool SphereColliderCakisiyorMu(
        GameObject kaynakObje,
        SphereCollider sphere,
        Collider temasCollideri,
        Vector3 hedefPozisyon,
        Quaternion hedefRotasyon)
    {
        Vector3 merkez = TransformNoktasi(kaynakObje, sphere.transform, hedefPozisyon, hedefRotasyon, sphere.center);

        Vector3 scale = MutlakLossyScale(sphere.transform);
        float maxScale = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
        float yaricap = Mathf.Max(0f, sphere.radius * maxScale - cakismaPayi);

        Collider[] carpanlar = Physics.OverlapSphere(
            merkez,
            yaricap,
            cakismaKatmani,
            QueryTriggerInteraction.Ignore
        );

        return GecerliCakismaVarMi(kaynakObje, temasCollideri, carpanlar);
    }

    private bool CapsuleColliderCakisiyorMu(
        GameObject kaynakObje,
        CapsuleCollider capsule,
        Collider temasCollideri,
        Vector3 hedefPozisyon,
        Quaternion hedefRotasyon)
    {
        Vector3 scale = MutlakLossyScale(capsule.transform);

        Vector3 localCenter = capsule.center;
        float radiusScale;
        float yukseklikScale;
        Vector3 axis;

        switch (capsule.direction)
        {
            case 0:
                radiusScale = Mathf.Max(scale.y, scale.z);
                yukseklikScale = scale.x;
                axis = hedefRotasyon * Vector3.right;
                break;
            case 1:
                radiusScale = Mathf.Max(scale.x, scale.z);
                yukseklikScale = scale.y;
                axis = hedefRotasyon * Vector3.up;
                break;
            default:
                radiusScale = Mathf.Max(scale.x, scale.y);
                yukseklikScale = scale.z;
                axis = hedefRotasyon * Vector3.forward;
                break;
        }

        float yaricap = Mathf.Max(0f, capsule.radius * radiusScale - cakismaPayi);
        float yukseklik = Mathf.Max(capsule.height * yukseklikScale, yaricap * 2f);
        float silindirUzunlugu = Mathf.Max(0f, yukseklik * 0.5f - yaricap);

        Vector3 merkez = TransformNoktasi(kaynakObje, capsule.transform, hedefPozisyon, hedefRotasyon, localCenter);
        Vector3 nokta1 = merkez + axis * silindirUzunlugu;
        Vector3 nokta2 = merkez - axis * silindirUzunlugu;

        Collider[] carpanlar = Physics.OverlapCapsule(
            nokta1,
            nokta2,
            yaricap,
            cakismaKatmani,
            QueryTriggerInteraction.Ignore
        );

        return GecerliCakismaVarMi(kaynakObje, temasCollideri, carpanlar);
    }

    private bool FallbackBoundsCakisiyorMu(
        GameObject kaynakObje,
        Collider col,
        Collider temasCollideri,
        Vector3 hedefPozisyon,
        Quaternion hedefRotasyon)
    {
        Bounds b = col.bounds;
        Vector3 merkezKaymasi = b.center - kaynakObje.transform.position;
        Vector3 merkez = hedefPozisyon + (hedefRotasyon * Quaternion.Inverse(kaynakObje.transform.rotation)) * merkezKaymasi;

        Vector3 halfExtents = Kucult(b.extents, cakismaPayi);

        Collider[] carpanlar = Physics.OverlapBox(
            merkez,
            halfExtents,
            hedefRotasyon,
            cakismaKatmani,
            QueryTriggerInteraction.Ignore
        );

        return GecerliCakismaVarMi(kaynakObje, temasCollideri, carpanlar);
    }

    private bool GecerliCakismaVarMi(GameObject kaynakObje, Collider temasCollideri, Collider[] carpanlar)
    {
        for (int i = 0; i < carpanlar.Length; i++)
        {
            Collider diger = carpanlar[i];
            if (diger == null) continue;

            // 1. Kendi Ghost parçalarımızı görmezden gel
            if (kaynakObje != null && diger.transform.IsChildOf(kaynakObje.transform))
                continue;

            // 2. Üzerinde durduğumuz zemini görmezden gel
            if (temasCollideri != null && (diger == temasCollideri || diger.transform.IsChildOf(temasCollideri.transform)))
                continue;

            // 3. KRİTİK: Karakteri (Player) görmezden gel (Hatanın kaynağı muhtemelen buydu)
            if (diger.CompareTag("Player") || diger.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
                continue;

            return true;
        }
        return false;
    }

    private Vector3 TransformNoktasi(
        GameObject kaynakObje,
        Transform altTransform,
        Vector3 hedefKokPozisyon,
        Quaternion hedefKokRotasyon,
        Vector3 localPoint)
    {
        Matrix4x4 localToRoot = kaynakObje.transform.worldToLocalMatrix * altTransform.localToWorldMatrix;
        Vector3 rootLocalPoint = localToRoot.MultiplyPoint3x4(localPoint);
        return hedefKokPozisyon + hedefKokRotasyon * rootLocalPoint;
    }

    private Vector3 MutlakLossyScale(Transform t)
    {
        Vector3 s = t.lossyScale;
        return new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
    }

    private Vector3 Kucult(Vector3 v, float miktar)
    {
        return new Vector3(
            Mathf.Max(0.0001f, v.x - miktar),
            Mathf.Max(0.0001f, v.y - miktar),
            Mathf.Max(0.0001f, v.z - miktar)
        );
    }
}