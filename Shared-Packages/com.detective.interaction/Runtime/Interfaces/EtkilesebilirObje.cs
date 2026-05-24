using UnityEngine;

public abstract class EtkilesebilirObje : MonoBehaviour, IEtkilesebilir
{
    [Header("UI Bilgileri")]
    [SerializeField] protected string objeAdi;
    [SerializeField] protected string etkilesimMetni;

    public virtual string ObjeAdiGetir()
    {
        return objeAdi;
    }

    public virtual string EtkilesimMetniGetir()
    {
        return etkilesimMetni;
    }

    public abstract void Etkilesim();
}