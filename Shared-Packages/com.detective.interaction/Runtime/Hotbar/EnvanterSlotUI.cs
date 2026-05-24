using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnvanterSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Referanslar")]
    [SerializeField] private Image slotArkaplan;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI adetText;

    [Header("Slot Bilgisi")]
    [SerializeField] private int slotIndex;

    private HotbarSistemi hotbarSistemi;
    private Canvas anaCanvas;

    private GameObject dragIkonObjesi;
    private Image dragIkonImage;

    private void Awake()
    {
        if (slotArkaplan == null)
            slotArkaplan = GetComponent<Image>();

        if (iconImage == null)
        {
            Transform iconChild = transform.Find("Icon");
            if (iconChild != null)
                iconImage = iconChild.GetComponent<Image>();
        }

        if (adetText == null)
        {
            Transform adetChild = transform.Find("AdetText");
            if (adetChild != null)
                adetText = adetChild.GetComponent<TextMeshProUGUI>();
        }

        anaCanvas = GetComponentInParent<Canvas>();
    }

    public void Baslat(HotbarSistemi sistem, int index)
    {
        hotbarSistemi = sistem;
        slotIndex = index;
    }

    public void GuncelleUI(Sprite ikon, int adet)
    {
        if (iconImage != null)
        {
            iconImage.sprite = ikon;
            iconImage.enabled = ikon != null;
        }

        if (adetText != null)
        {
            bool goster = ikon != null && adet > 1;
            adetText.gameObject.SetActive(goster);

            if (goster)
                adetText.text = adet.ToString();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (hotbarSistemi == null) return;
        if (!hotbarSistemi.EnvanterAcikMi()) return;
        if (!hotbarSistemi.SlottaEsyaVarMi(slotIndex)) return;

        dragIkonObjesi = new GameObject("DragIkon");
        dragIkonObjesi.transform.SetParent(anaCanvas.transform, false);
        dragIkonObjesi.transform.SetAsLastSibling();

        dragIkonImage = dragIkonObjesi.AddComponent<Image>();
        dragIkonImage.raycastTarget = false;
        dragIkonImage.sprite = hotbarSistemi.SlottakiIkonuGetir(slotIndex);
        dragIkonImage.preserveAspect = true;
        dragIkonImage.color = new Color(1f, 1f, 1f, 0.85f);

        RectTransform rt = dragIkonObjesi.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80f, 80f);

        DragPozisyonunuGuncelle(eventData);
        hotbarSistemi.SuruklemeBaslat(slotIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIkonObjesi == null)
            return;

        DragPozisyonunuGuncelle(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIkonObjesi != null)
            Destroy(dragIkonObjesi);

        if (hotbarSistemi != null)
        {
            int kaynakIndex = hotbarSistemi.AktifSuruklenenSlotIndex();
            bool slotUzerineBirakildi = BirSlotUIUzerineBirakildi(eventData.pointerEnter);

            if (kaynakIndex >= 0 && !slotUzerineBirakildi)
            {
                hotbarSistemi.GlobalSlottakiEsyayiDunyayaAt(kaynakIndex);
            }

            hotbarSistemi.SuruklemeBitir();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (hotbarSistemi == null) return;
        if (!hotbarSistemi.EnvanterAcikMi()) return;

        int kaynakIndex = hotbarSistemi.AktifSuruklenenSlotIndex();
        if (kaynakIndex < 0) return;
        if (kaynakIndex == slotIndex) return;

        hotbarSistemi.SlotlariYerDegistir(kaynakIndex, slotIndex);
    }

    private void DragPozisyonunuGuncelle(PointerEventData eventData)
    {
        if (dragIkonObjesi == null || anaCanvas == null)
            return;

        RectTransform canvasRect = anaCanvas.transform as RectTransform;
        RectTransform dragRect = dragIkonObjesi.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPos))
        {
            dragRect.localPosition = localPos;
        }
    }

    private bool BirSlotUIUzerineBirakildi(GameObject hedef)
    {
        if (hedef == null)
            return false;

        if (hedef.GetComponent<EnvanterSlotUI>() != null)
            return true;

        if (hedef.GetComponent<HotbarSlotUI>() != null)
            return true;

        return hedef.GetComponentInParent<EnvanterSlotUI>() != null ||
               hedef.GetComponentInParent<HotbarSlotUI>() != null;
    }
}