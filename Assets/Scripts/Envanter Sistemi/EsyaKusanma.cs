using UnityEngine;
using UnityEngine.InputSystem;

public class EsyaKusanma : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform tutmaNoktasi;
    public Camera oyuncuKamerasi;
    public float yerlestirmeMenzili = 3f;
    public float maksimumEgim = 15f;

    private GameObject elimdekiEsya;
    private EnvanterSlot secilenSlot;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (elimdekiEsya != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            EsyaYerlestir();
        }
    }

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
                if (elimdekiEsya == null || guncelEsya.esyaPrefab != null)
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
            elimdekiEsya = Instantiate(
                kusanilacakEsya.esyaPrefab,
                tutmaNoktasi.position,
                tutmaNoktasi.rotation
            );

            elimdekiEsya.transform.SetParent(tutmaNoktasi);

            Rigidbody[] rbs = elimdekiEsya.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in rbs)
                rb.isKinematic = true;

            Collider[] colliders = elimdekiEsya.GetComponentsInChildren<Collider>();

            foreach (Collider col in colliders)
                col.enabled = false;
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

    private void EsyaYerlestir()
    {
        if (oyuncuKamerasi == null)
            return;

        if (elimdekiEsya == null)
            return;

        Ray ray = oyuncuKamerasi.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit[] vuranObjeler = Physics.RaycastAll(ray, yerlestirmeMenzili);

        System.Array.Sort(vuranObjeler, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in vuranObjeler)
        {
            if (hit.collider.gameObject.CompareTag("Player") || hit.collider.GetComponent<CharacterController>() != null)
            {
                continue;
            }

            float yuzeyEgimi = Vector3.Angle(Vector3.up, hit.normal);

            if (yuzeyEgimi <= maksimumEgim)
            {
                elimdekiEsya.transform.SetParent(null);
                elimdekiEsya.transform.position = hit.point;
                elimdekiEsya.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                Collider[] colliders = elimdekiEsya.GetComponentsInChildren<Collider>();

                foreach (Collider col in colliders)
                    col.enabled = true;

                Rigidbody[] rbs = elimdekiEsya.GetComponentsInChildren<Rigidbody>();

                foreach (Rigidbody rb in rbs)
                    rb.isKinematic = false;

                elimdekiEsya = null;

                if (secilenSlot != null)
                {
                    secilenSlot.SlotuBosalt();
                    secilenSlot = null;
                }

                return;
            }
            else
            {
                Debug.LogWarning("Burası eşya koymak için çok dik.");
                return;
            }
        }
    }
}