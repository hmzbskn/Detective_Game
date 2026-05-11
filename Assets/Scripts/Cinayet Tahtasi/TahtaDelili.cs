using UnityEngine;
using UnityEngine.EventSystems;

public class TahtaDelili : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    // YENİ: İçinde bulunduğumuz mantar panonun (Ebeveynin) sınırları
    private RectTransform tahtaRect;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Ebeveynimizi (Mantar Panoyu) bul
        tahtaRect = transform.parent.GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Fareyle sürüklendiğinde pozisyonu güncelle
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // BÜYÜK DÜZELTME: Güncellenen bu pozisyon tahtanın sınırlarını aştı mı diye kontrol et!
        SinirlarinIcindeTut();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.SetAsLastSibling();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (IpYoneticisi.Instance != null)
            {
                IpYoneticisi.Instance.DelileTiklandi(rectTransform);
            }
        }
    }

    // İŞTE SİHİRLİ MATEMATİK FONKSİYONU
    private void SinirlarinIcindeTut()
    {
        if (tahtaRect == null) return;

        // Resmin merkez (pivot) noktasının mevcut lokal pozisyonunu al
        Vector3 pozisyon = rectTransform.localPosition;

        // Resmin genişliğini ve yüksekliğini hesaba katarak sınırları belirle
        // (Resmin yarısı kadar pay bırakıyoruz ki çerçevenin yarısı dışarı taşmasın)
        float minX = tahtaRect.rect.xMin + (rectTransform.rect.width * rectTransform.pivot.x);
        float maxX = tahtaRect.rect.xMax - (rectTransform.rect.width * (1 - rectTransform.pivot.x));

        float minY = tahtaRect.rect.yMin + (rectTransform.rect.height * rectTransform.pivot.y);
        float maxY = tahtaRect.rect.yMax - (rectTransform.rect.height * (1 - rectTransform.pivot.y));

        // Pozisyonu bu dört duvar arasına "Kelepçele" (Clamp)
        pozisyon.x = Mathf.Clamp(pozisyon.x, minX, maxX);
        pozisyon.y = Mathf.Clamp(pozisyon.y, minY, maxY);

        // Kelepçelenmiş yeni pozisyonu resme uygula
        rectTransform.localPosition = pozisyon;
    }
}