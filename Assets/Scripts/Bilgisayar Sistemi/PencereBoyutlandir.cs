using UnityEngine;
using UnityEngine.EventSystems;

public class PencereBoyutlandir : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Header("Ayarlar")]
    public RectTransform anaPencere;
    public Vector2 minimumBoyut = new Vector2(400, 300); // Pencere bundan daha küçük olamaz

    private Canvas anaCanvas;

    void Start()
    {
        if (anaPencere == null) anaPencere = transform.parent.GetComponent<RectTransform>();
        anaCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Köşeye tıklandığı an pencereyi en öne çıkar
        if (anaPencere != null) anaPencere.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (anaPencere == null || anaCanvas == null) return;

        // Farenin sağa/sola ve aşağı/yukarı hareketini al (Canvas boyutuna göre oranla)
        Vector2 boyutDegisimi = new Vector2(eventData.delta.x, -eventData.delta.y) / anaCanvas.scaleFactor;
        Vector2 yeniBoyut = anaPencere.sizeDelta + boyutDegisimi;

        // Minimum sınırların altına inmesini engelle
        yeniBoyut.x = Mathf.Max(yeniBoyut.x, minimumBoyut.x);
        yeniBoyut.y = Mathf.Max(yeniBoyut.y, minimumBoyut.y);

        // Pencerenin merkez noktası ortadaysa, boyutu büyürken sol üst köşesi sabit kalsın diye
        // pozisyonunu da hafifçe kaydırmamız gerekir (matematik hilesi)
        Vector2 gercekDegisim = yeniBoyut - anaPencere.sizeDelta;
        anaPencere.sizeDelta = yeniBoyut;
        anaPencere.anchoredPosition += new Vector2(gercekDegisim.x / 2f, -gercekDegisim.y / 2f);
    }
}