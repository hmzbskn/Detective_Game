using UnityEngine;
using UnityEngine.EventSystems;

public class PencereBoyutlayici : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform anaPencere;
    [SerializeField] private Canvas anaCanvas;
    [SerializeField] private BilgisayarPenceresi pencereScripti;
    [SerializeField] private Vector2 minimumBoyut = new Vector2(500, 350);
    [SerializeField] private Vector2 maksimumBoyut = new Vector2(1200, 800);

    private void Awake()
    {
        if (anaPencere == null)
            anaPencere = GetComponentInParent<BilgisayarPenceresi>().GetComponent<RectTransform>();

        if (anaCanvas == null)
            anaCanvas = GetComponentInParent<Canvas>();

        if (pencereScripti == null)
            pencereScripti = GetComponentInParent<BilgisayarPenceresi>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pencereScripti != null)
            pencereScripti.ONeGetir();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (anaPencere == null || anaCanvas == null)
            return;


        Vector2 delta = eventData.delta / anaCanvas.scaleFactor;

        Vector2 yeniBoyut = anaPencere.sizeDelta + new Vector2(delta.x, -delta.y);
        // minimum sınır
        yeniBoyut.x = Mathf.Max(yeniBoyut.x, minimumBoyut.x);
        yeniBoyut.y = Mathf.Max(yeniBoyut.y, minimumBoyut.y);

        // maksimum sınır
        yeniBoyut.x = Mathf.Min(yeniBoyut.x, maksimumBoyut.x);
        yeniBoyut.y = Mathf.Min(yeniBoyut.y, maksimumBoyut.y);

        anaPencere.sizeDelta = yeniBoyut;
    }
}