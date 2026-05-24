using UnityEngine;

public class DelilEtkilesim : EtkilesebilirObje
{
    private bool alindiMi = false;

    public override void Etkilesim()
    {
        if (alindiMi) return;

        alindiMi = true;
        Debug.Log(objeAdi + " delili alındı.");

        // Envantere ekleme burada yapılabilir
        gameObject.SetActive(false);
    }
}