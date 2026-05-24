using UnityEngine;

public class EsyaFirlatici : MonoBehaviour
{
    [SerializeField] private float firlatmaGucu = 1.5f;

    public void Firlat(ItemData data)
    {
        if (data == null)
            return;

        Firlat(new ItemInstanceData(data));
    }

    public void Firlat(ItemInstanceData instanceData)
    {
        if (instanceData == null || instanceData.ItemData == null)
            return;

        ItemData data = instanceData.ItemData;

        if (data.WorldPrefab == null)
            return;

        GameObject obje = Instantiate(data.WorldPrefab);
        obje.name = data.WorldPrefab.name + "_Dropped";

        EldeTutulabilirObje tutulabilir = obje.GetComponent<EldeTutulabilirObje>();
        if (tutulabilir != null)
        {
            tutulabilir.ItemInstanceDataAyarla(instanceData);
            tutulabilir.TutulmaDurumunuAyarla(false);
        }

        FotografBaskiObjesi baskiObjesi = obje.GetComponentInChildren<FotografBaskiObjesi>(true);
        if (baskiObjesi != null && instanceData.FotografKaydi != null)
        {
            baskiObjesi.FotografAta(instanceData.FotografKaydi);
        }

        Camera anaKamera = Camera.main;

        Vector3 cikisPozisyonu = anaKamera != null
            ? anaKamera.transform.position + anaKamera.transform.forward * 1.5f
            : transform.position + transform.forward * 1.5f;

        Vector3 ileriYon = anaKamera != null
            ? anaKamera.transform.forward
            : transform.forward;

        obje.transform.position = cikisPozisyonu;

        Rigidbody rb = obje.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(ileriYon * firlatmaGucu, ForceMode.Impulse);
        }
    }
}