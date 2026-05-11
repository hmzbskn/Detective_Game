using UnityEngine;

public interface IEtkilesebilir
{
    // Etkileşime girildiğinde (örn: E tuşuna basıldığında) tetiklenecek ana fonksiyon
    void Etkilesim();

    // Ekranda "Kapıyı Aç" veya "Eşyayı Al" gibi dinamik yazılar göstermek için
    string EtkilesimMetniGetir();
}
