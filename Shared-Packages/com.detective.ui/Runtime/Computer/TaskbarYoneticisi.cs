using System.Collections.Generic;
using UnityEngine;

public class TaskbarYoneticisi : MonoBehaviour
{
    [SerializeField] private Transform windowsContainer;
    [SerializeField] private TaskbarUygulamaButonu taskbarButonPrefab;

    private readonly Dictionary<BilgisayarPenceresi, TaskbarUygulamaButonu> aktifButonlar = new();

    public void PencereAcildi(BilgisayarPenceresi pencere)
    {
        if (pencere == null || windowsContainer == null || taskbarButonPrefab == null)
            return;

        if (aktifButonlar.ContainsKey(pencere))
            return;

        TaskbarUygulamaButonu yeniButon = Instantiate(taskbarButonPrefab, windowsContainer);
        yeniButon.Kur(pencere, this);
        aktifButonlar.Add(pencere, yeniButon);
    }

    public void PencereKapandi(BilgisayarPenceresi pencere)
    {
        if (pencere == null)
            return;

        if (aktifButonlar.TryGetValue(pencere, out TaskbarUygulamaButonu buton))
        {
            if (buton != null)
                Destroy(buton.gameObject);

            aktifButonlar.Remove(pencere);
        }
    }

    public bool ButonVarMi(BilgisayarPenceresi pencere)
    {
        return aktifButonlar.ContainsKey(pencere);
    }
}