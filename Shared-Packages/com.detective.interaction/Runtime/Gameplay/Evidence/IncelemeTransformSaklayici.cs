using UnityEngine;

/// <summary>
/// İncelenen objenin eski transform (parent/pozisyon/rotasyon/scale), rigidbody ve collider
/// durumunu kaydedip inceleme bitince geri yükler. IncelemeSistemi'nin kendisi input/zoom/döndürme
/// gibi etkileşim mantığına odaklanabilsin diye bu "önce/sonra" durum yönetimi buraya taşındı.
/// </summary>
public class IncelemeTransformSaklayici
{
    private Transform oncekiParent;
    private Vector3 oncekiLocalPozisyon;
    private Quaternion oncekiLocalRotasyon;
    private Vector3 oncekiLocalScale;

    private Rigidbody aktifRb;
    private Collider[] aktifColliderlar;

    public Vector3 OncekiLocalScale => oncekiLocalScale;

    public void Kaydet(GameObject hedefObje)
    {
        oncekiParent = hedefObje.transform.parent;
        oncekiLocalPozisyon = hedefObje.transform.localPosition;
        oncekiLocalRotasyon = hedefObje.transform.localRotation;
        oncekiLocalScale = hedefObje.transform.localScale;

        aktifRb = hedefObje.GetComponent<Rigidbody>();
        if (aktifRb != null)
        {
            aktifRb.linearVelocity = Vector3.zero;
            aktifRb.angularVelocity = Vector3.zero;
            aktifRb.isKinematic = true;
            aktifRb.useGravity = false;
        }

        aktifColliderlar = hedefObje.GetComponentsInChildren<Collider>(true);

        foreach (var col in aktifColliderlar)
        {
            col.enabled = false;
        }
    }

    public void GeriYukle(GameObject hedefObje)
    {
        hedefObje.transform.SetParent(oncekiParent);
        hedefObje.transform.localPosition = oncekiLocalPozisyon;
        hedefObje.transform.localRotation = oncekiLocalRotasyon;
        hedefObje.transform.localScale = oncekiLocalScale;

        if (aktifRb != null)
        {
            aktifRb.isKinematic = true;
            aktifRb.useGravity = false;
        }

        if (aktifColliderlar != null)
        {
            foreach (var col in aktifColliderlar)
            {
                col.enabled = false;
            }
        }

        aktifRb = null;
        aktifColliderlar = null;
    }
}
