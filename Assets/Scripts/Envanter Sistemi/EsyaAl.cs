using UnityEngine;

public class EsyaAl : MonoBehaviour , IEtkilesebilir
{
    [Tooltip("Bu obje yerden alınınca envantere hangi veri gidecek?")]
    public EsyaVerisi esyaVerisi;

    public void Etkilesim()
    {
        // Sahnede envanter yöneticisini bul
        EnvanterKontrol envanter = FindFirstObjectByType<EnvanterKontrol>();

        if (envanter != null)
        {
            // Eşyayı çantaya eklemeyi dene (EnvanterKontrol içine yazdığımız EsyaEkle metodunu çağırıyoruz)
            bool eklendiMi = envanter.EsyaEkle(esyaVerisi);

            if (eklendiMi)
            {
                // Başarıyla eklendiyse, sahnede var olan 3D objeyi yok et
                Debug.Log(esyaVerisi.esyaAdi + " çantaya atıldı.");
                Destroy(gameObject);
            }
        }
    }

    public string EtkilesimMetniGetir()
    {
        // Ekranda "E : Al: Su Bardağı" yazmasını sağlar
        return "Al: " + esyaVerisi.esyaAdi;
    }
}
