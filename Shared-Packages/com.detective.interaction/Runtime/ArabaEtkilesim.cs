using UnityEngine;
using UnityEngine.Events;

public class ArabaEtkilesim : EtkilesebilirObje
{
    [Header("Araba Etkileşim Ayarları")]
    [SerializeField] private UnityEvent arabaEtkilesimEventi;

    public override void Etkilesim()
    {
        Debug.Log("Araba ile etkileşime girildi: " + gameObject.name);

        if (arabaEtkilesimEventi == null)
        {
            Debug.LogWarning("Araba etkileşim eventi boş: " + gameObject.name);
            return;
        }

        arabaEtkilesimEventi.Invoke();
    }
}