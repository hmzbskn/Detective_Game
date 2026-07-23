using UnityEngine;
using UnityEngine.Events;

public class EtkilesimObjesi : MonoBehaviour ,IEtkilesebilir
{
    [Header("Arayüz Ayarları")]
    [Tooltip("Ekranda görünecek eylem yazısı (Örn: Kapıyı Aç)")]
    [SerializeField] private string etkilesimMesaji = "Etkileşime Geç";

    [Header("Olay Tetikleyiciler")]
    [Tooltip("Etkileşim tuşuna basıldığında yapılacak işlerin listesi")]
    public UnityEvent etkilesimOlayi;

    // Arayüzden gelen ana fonksiyon
    public void Etkilesim()
    {
        // Inspector'da listeye ne eklediysek sırayla çalıştırır
        etkilesimOlayi?.Invoke();
    }

    // Arayüzden gelen yazı fonksiyonu
    public string EtkilesimMetniGetir()
    {
        return etkilesimMesaji;
    }
}
