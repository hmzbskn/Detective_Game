using UnityEngine;
using System.Linq; // Sıralama yapmak için gerekli

public class EsyaKusanma : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform tutmaNoktasi;
    public Camera oyuncuKamerasi;
    public float yerlestirmeMenzili = 3f;
    public float maksimumEgim = 15f;

    private GameObject elimdekiEsya;
    private EnvanterSlot secilenSlot;

    void Update()
    {
        // BÜYÜK DÜZELTME: Cursor.visible Unity'de başlangıçta bug'a girebilir. 
        // Bunun yerine farenin ekrana kilitli olup olmadığını soruyoruz!
        if (Cursor.lockState != CursorLockMode.Locked) return;

        if (elimdekiEsya != null && Input.GetMouseButtonDown(0))
        {
            EsyaYerlestir();
        }
    }
    // Tahta sistemi eldeki eşyayı alabilsin diye eklediğimiz köprü
    public EnvanterSlot SecilenSlotuGetir()
    {
        return secilenSlot;
    }
    public void EldekiNesneyiYenile()
    {
        if (secilenSlot != null)
        {
            EsyaVerisi guncelEsya = secilenSlot.EsyaGetir();

            if (guncelEsya != null)
            {
                if (elimdekiEsya == null || (elimdekiEsya != null && guncelEsya.esyaPrefab != null))
                {
                    EsyaKusan(guncelEsya, secilenSlot);
                }
            }
            else
            {
                ElindekiniTemizle();
            }
        }
    }

    public void EsyaKusan(EsyaVerisi kusanilacakEsya, EnvanterSlot kaynakSlot)
    {
        ElindekiniTemizle();
        secilenSlot = kaynakSlot;

        if (kusanilacakEsya != null && kusanilacakEsya.esyaPrefab != null)
        {
            elimdekiEsya = Instantiate(kusanilacakEsya.esyaPrefab, tutmaNoktasi.position, tutmaNoktasi.rotation);
            elimdekiEsya.transform.SetParent(tutmaNoktasi);

            Rigidbody[] rbs = elimdekiEsya.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs) rb.isKinematic = true;

            Collider[] colliders = elimdekiEsya.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders) col.enabled = false;
        }
    }

    public void ElindekiniTemizle()
    {
        if (elimdekiEsya != null)
        {
            Destroy(elimdekiEsya);
            elimdekiEsya = null;
        }
    }

    // İŞTE EFSANE BÖLÜM BURASI: Kendi vücudumuzu delip geçen akıllı Raycast
    private void EsyaYerlestir()
    {
        Ray ray = oyuncuKamerasi.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Tek bir obje yerine, lazerin delip geçtiği TÜM objeleri listeye al
        RaycastHit[] vuranObjeler = Physics.RaycastAll(ray, yerlestirmeMenzili);

        // Vurulan objeleri kameraya olan uzaklıklarına göre yakından uzağa sırala
        System.Array.Sort(vuranObjeler, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in vuranObjeler)
        {
            // Eğer ışın kendi vücudumuza (Player tag'li veya CharacterController'a sahip) çarpıyorsa ES GEÇ!
            if (hit.collider.gameObject.CompareTag("Player") || hit.collider.GetComponent<CharacterController>() != null)
            {
                continue;
            }

            // Eğer buraya indiysek, karşımızda duran gerçek bir yüzeye (yer, masa vs.) çarptık demektir.
            float yuzeyEgimi = Vector3.Angle(Vector3.up, hit.normal);

            if (yuzeyEgimi <= maksimumEgim)
            {
                elimdekiEsya.transform.SetParent(null);
                elimdekiEsya.transform.position = hit.point;
                elimdekiEsya.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                Collider[] colliders = elimdekiEsya.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders) col.enabled = true;

                Rigidbody[] rbs = elimdekiEsya.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rb in rbs) rb.isKinematic = false;

                elimdekiEsya = null;

                if (secilenSlot != null)
                {
                    secilenSlot.SlotuBosalt();
                    secilenSlot = null;
                }

                // Başarıyla koyduk, işlemi bitir
                return;
            }
            else
            {
                Debug.LogWarning("Burası eşya koymak için çok dik!");
                return; // Geçerli ilk yüzey çok dikse, onun arkasındaki duvarlara koymaya çalışma
            }
        }
    }
}