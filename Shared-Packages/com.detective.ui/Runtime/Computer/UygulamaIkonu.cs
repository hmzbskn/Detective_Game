using UnityEngine;

public class UygulamaIkonu : MonoBehaviour
{
    [SerializeField] private BilgisayarPenceresi hedefPencere;

    public void Tiklandi()
    {
        if (hedefPencere == null)
        {
            Debug.LogError($"{name} ikonuna hedef pencere atanmadı.");
            return;
        }

        hedefPencere.Ac();
    }
}