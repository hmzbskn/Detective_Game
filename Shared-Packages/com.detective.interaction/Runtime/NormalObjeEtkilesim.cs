using UnityEngine;

public class NormalObjeEtkilesim : EtkilesebilirObje
{
    public override void Etkilesim()
    {
        Debug.Log(objeAdi + " ile etkileşime girildi.");
    }
}