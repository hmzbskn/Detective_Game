using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskbarUygulamaButonu : MonoBehaviour
{
    [SerializeField] private Button buton;
    [SerializeField] private TMP_Text baslikText;

    private BilgisayarPenceresi bagliPencere;
    private TaskbarYoneticisi taskbarYoneticisi;

    public void Kur(BilgisayarPenceresi pencere, TaskbarYoneticisi yonetici)
    {
        bagliPencere = pencere;
        taskbarYoneticisi = yonetici;

        if (buton == null)
            buton = GetComponent<Button>();

        if (baslikText != null && bagliPencere != null)
            baslikText.text = bagliPencere.PencereAdi;

        if (buton != null)
        {
            buton.onClick.RemoveAllListeners();
            buton.onClick.AddListener(Tiklandi);
        }
    }

    private void Tiklandi()
    {
        if (bagliPencere == null)
            return;

        if (bagliPencere.Durum == BilgisayarPenceresi.PencereDurumu.Kucultulmus)
            bagliPencere.Ac();
        else
            bagliPencere.ONeGetir();
    }

    public BilgisayarPenceresi GetPencere()
    {
        return bagliPencere;
    }
}