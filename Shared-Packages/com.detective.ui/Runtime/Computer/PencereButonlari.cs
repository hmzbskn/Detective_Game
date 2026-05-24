using UnityEngine;

public class PencereButonlari : MonoBehaviour
{
    [SerializeField] private BilgisayarPenceresi pencere;

    public void Kapat()
    {
        if (pencere != null)
            pencere.Kapat();
    }

    public void Kucult()
    {
        if (pencere != null)
            pencere.Kucult();
    }
}