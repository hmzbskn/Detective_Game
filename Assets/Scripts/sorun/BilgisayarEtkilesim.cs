using UnityEngine;

public class BilgisayarEtkilesim : EtkilesebilirObje
{
    [Header("Bilgisayar Modu")]
    [SerializeField] private BilgisayarModuSistemi bilgisayarModu;

    public override void Etkilesim()
    {
        if (bilgisayarModu == null)
        {
            Debug.LogWarning("BilgisayarModuSistemi referansı atanmadı: " + gameObject.name);
            return;
        }

        bilgisayarModu.BilgisayarModunuAc();
    }
}