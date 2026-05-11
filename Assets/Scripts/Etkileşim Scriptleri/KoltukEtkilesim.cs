using UnityEngine;
using StarterAssets;

public class KoltukEtkilesim : MonoBehaviour, IEtkilesebilir
{
    [Header("Oturma Ayarları")]
    public Transform oturmaNoktasi;
    public Transform kalkmaNoktasi;

    private bool oturuyorMu = false;
    private GameObject oyuncu;
    private FirstPersonController fpsKontrol;
    private CharacterController charKontrol;
    private StarterAssetsInputs oyuncuGirdileri;

    // Sadece hız ayarlarını yedekleyeceğiz
    private float eskiYurumeHizi;
    private float eskiKosmaHizi;
    private float eskiZiplayisGucu;

    private Collider[] koltukColliderlari;

    public string EtkilesimMetniGetir()
    {
        return oturuyorMu ? "" : "Otur: Koltuk";
    }

    public void Etkilesim()
    {
        if (!oturuyorMu) Otur();
    }

    private void Otur()
    {
        oyuncu = FindFirstObjectByType<FirstPersonController>().gameObject;
        fpsKontrol = oyuncu.GetComponent<FirstPersonController>();
        charKontrol = oyuncu.GetComponent<CharacterController>();
        oyuncuGirdileri = oyuncu.GetComponent<StarterAssetsInputs>();

        // 1. Orijinal hızları yedekle
        eskiYurumeHizi = fpsKontrol.MoveSpeed;
        eskiKosmaHizi = fpsKontrol.SprintSpeed;
        eskiZiplayisGucu = fpsKontrol.JumpHeight;

        // 2. Koltuğun görünmez duvarlarını kapat ki bizi ittirmesin (Fırlama bug'ı çözümü)
        koltukColliderlari = GetComponentsInChildren<Collider>();
        foreach (Collider col in koltukColliderlari)
        {
            col.enabled = false;
        }

        // 3. Işınla (Sen nereye koyduysan tam oraya)
        charKontrol.enabled = false;
        oyuncu.transform.position = oturmaNoktasi.position;
        charKontrol.enabled = true;

        // 4. Yürümeyi sıfırla (Sadece fare ile etrafa bakılabilsin)
        fpsKontrol.MoveSpeed = 0f;
        fpsKontrol.SprintSpeed = 0f;
        fpsKontrol.JumpHeight = 0f;

        oturuyorMu = true;
        Debug.Log("Koltuğa oturdun. Kalkmak için Zıplama (Boşluk) tuşuna bas!");
    }

    void Update()
    {
        if (!oturuyorMu) return;

        // Space tuşuna basıldığında kalk
        if (oyuncuGirdileri != null && oyuncuGirdileri.jump)
        {
            oyuncuGirdileri.jump = false;
            Kalk();
        }
    }

    private void Kalk()
    {
        // 1. Kalkma noktasına ışınla
        charKontrol.enabled = false;
        oyuncu.transform.position = kalkmaNoktasi.position;

        // 2. Koltuğun duvarlarını geri aç
        if (koltukColliderlari != null)
        {
            foreach (Collider col in koltukColliderlari)
            {
                col.enabled = true;
            }
        }

        charKontrol.enabled = true;

        // 3. Hızları geri ver (Tekrar yürüyebil)
        fpsKontrol.MoveSpeed = eskiYurumeHizi;
        fpsKontrol.SprintSpeed = eskiKosmaHizi;
        fpsKontrol.JumpHeight = eskiZiplayisGucu;

        oturuyorMu = false;
    }
}