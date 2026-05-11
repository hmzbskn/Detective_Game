using UnityEngine;
using UnityEngine.EventSystems; // UI tıklama/sürükleme olayları için şart!

public class PencereSurukle : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Tooltip("Sürüklenecek ana pencere objesi (Örn: Pencere_Genel)")]
    public RectTransform anaPencere;

    private Canvas anaCanvas;

    void Awake()
    {
        // Eğer Inspector'dan ana pencereyi atamayı unutursan, otomatik olarak bir üst objeyi (ebeveyni) alır.
        if (anaPencere == null)
        {
            anaPencere = transform.parent.GetComponent<RectTransform>();
        }

        // Fare hareketini doğru hesaplamak için ana Canvas'ı buluyoruz.
        anaCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Üst bara tıklandığı an, bu pencereyi diğer tüm pencerelerin üstüne (en öne) çıkarır.
        if (anaPencere != null)
        {
            anaPencere.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Fare (mouse) hareket ettikçe pencerenin pozisyonunu kaydırır.
        if (anaCanvas != null && anaPencere != null)
        {
            anaPencere.anchoredPosition += eventData.delta / anaCanvas.scaleFactor;
        }
    }
}   