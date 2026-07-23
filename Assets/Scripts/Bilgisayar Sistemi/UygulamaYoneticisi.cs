using UnityEngine;

public class UygulamaYoneticisi : MonoBehaviour
{
    [Header("Pencere Ayarları")]
    [Tooltip("Bu ikon tıklandığında hangi pencere açılacak?")]
    public GameObject acilacakPencere;

    // 1. UYGULAMAYI AÇMA
    public void UygulamayiAc()
    {
        if (acilacakPencere != null)
        {
            // Pencereyi görünür yap
            acilacakPencere.SetActive(true);

            // Pencereyi görsel olarak en öne (diğer pencerelerin üstüne) taşı
            acilacakPencere.transform.SetAsLastSibling();

            Debug.Log(acilacakPencere.name + " uygulaması açıldı.");
        }
        else
        {
            Debug.LogError("Hata: " + gameObject.name + " ikonuna bir pencere atanmamış!");
        }
    }

    // 2. UYGULAMAYI KAPATMA
    public void UygulamayiKapat()
    {
        if (acilacakPencere != null)
        {
            acilacakPencere.SetActive(false);
        }
    }
}