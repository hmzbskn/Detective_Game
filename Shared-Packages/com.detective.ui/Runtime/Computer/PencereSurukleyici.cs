using UnityEngine;
using UnityEngine.EventSystems;

public class PencereSurukleyici : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform anaPencere;
    [SerializeField] private Canvas anaCanvas;
    [SerializeField] private BilgisayarPenceresi pencereScripti;

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

        anaPencere.anchoredPosition += eventData.delta / anaCanvas.scaleFactor;
    }
}