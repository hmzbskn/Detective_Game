using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI Referansları")]
    [SerializeField] private Image slotArkaplan;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI adetText;

    [Header("Renkler")]
    [SerializeField] private Color normalRenk = Color.white;
    [SerializeField] private Color seciliRenk = new Color(1f, 0.85f, 0.25f, 1f);

    private HotbarSistemi hotbarSistemi;
    private int hotbarIndex = -1;
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
        hotbarIndex = index;
    }

    public void GuncelleUI(Sprite ikon, bool seciliMi, int adet)
    {
        if (slotArkaplan != null)
            slotArkaplan.color = seciliMi ? seciliRenk : normalRenk;

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
        if (hotbarSistemi == null)
            return;

        bool suruklemeyeIzinVar =
            hotbarSistemi.EnvanterAcikMi() ||
            Cursor.lockState == CursorLockMode.None;

        if (!suruklemeyeIzinVar)
            return;

        if (!hotbarSistemi.HotbarSlottaEsyaVarMi(hotbarIndex))
            return;

        dragIkonObjesi = new GameObject("DragIkon");
        dragIkonObjesi.transform.SetParent(anaCanvas.transform, false);
        dragIkonObjesi.transform.SetAsLastSibling();

        dragIkonImage = dragIkonObjesi.AddComponent<Image>();
        dragIkonImage.raycastTarget = false;
        dragIkonImage.sprite = hotbarSistemi.HotbarSlottakiIkonuGetir(hotbarIndex);
        dragIkonImage.preserveAspect = true;
        dragIkonImage.color = new Color(1f, 1f, 1f, 0.85f);

        RectTransform rt = dragIkonObjesi.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80f, 80f);

        DragPozisyonunuGuncelle(eventData);

        hotbarSistemi.SuruklemeBaslat(100 + hotbarIndex);

        Debug.Log("Hotbar sürükleme başladı. Kaynak index: " + (100 + hotbarIndex));
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIkonObjesi == null)
            return;

        DragPozisyonunuGuncelle(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("GUNCEL HOTBARSLOTUI ONENDDRAG CALISTI");

        if (dragIkonObjesi != null)
        {
            Destroy(dragIkonObjesi);
            dragIkonObjesi = null;
        }

        if (hotbarSistemi == null)
            return;

        int kaynakIndex = hotbarSistemi.AktifSuruklenenSlotIndex();

        Debug.Log("OnEndDrag kaynak index: " + kaynakIndex);

        if (kaynakIndex < 0)
        {
            hotbarSistemi.SuruklemeBitir();
            return;
        }

        bool slotUzerineBirakildi = BirSlotUIUzerineBirakildi(eventData.pointerEnter);

        Debug.Log("Pointer Enter: " + (eventData.pointerEnter != null ? eventData.pointerEnter.name : "YOK"));
        Debug.Log("Slot üstüne bırakıldı mı: " + slotUzerineBirakildi);

        if (!slotUzerineBirakildi)
        {
            MonoBehaviour tahtaZemini = EkranPozisyonundakiTahtaZeminiBul(
                eventData.position,
                eventData.pressEventCamera
            );

            if (tahtaZemini != null)
            {
                Sprite ikon = hotbarSistemi.HotbarSlottakiIkonuGetir(hotbarIndex);

                bool tahtayaAsildi = TahtaZeminineHotbarIkonuGonder(
                    tahtaZemini,
                    ikon,
                    eventData.position,
                    eventData.pressEventCamera
                );

                hotbarSistemi.SuruklemeBitir();

                if (tahtayaAsildi)
                {
                    HotbarSlotunuBosaltmayaCalis(kaynakIndex);
                    return;
                }
            }
            else
            {
                Debug.LogWarning("Bırakılan ekran pozisyonunda TahtaZemini bulunamadı.");
            }
        }

        if (!slotUzerineBirakildi)
        {
            hotbarSistemi.GlobalSlottakiEsyayiDunyayaAt(kaynakIndex);
        }

        hotbarSistemi.SuruklemeBitir();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (hotbarSistemi == null)
            return;

        if (!hotbarSistemi.EnvanterAcikMi())
            return;

        int kaynakIndex = hotbarSistemi.AktifSuruklenenSlotIndex();

        if (kaynakIndex < 0)
            return;

        int hedefGlobalIndex = 100 + hotbarIndex;

        if (kaynakIndex == hedefGlobalIndex)
            return;

        hotbarSistemi.SlotlariYerDegistir(kaynakIndex, hedefGlobalIndex);
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

    private MonoBehaviour EkranPozisyonundakiTahtaZeminiBul(Vector2 ekranPozisyonu, Camera eventCamera)
    {
        MonoBehaviour[] tumComponentler = Object.FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (MonoBehaviour component in tumComponentler)
        {
            if (component == null)
                continue;

            if (component.GetType().Name != "TahtaZemini")
                continue;

            RectTransform rectTransform = component.GetComponent<RectTransform>();

            if (rectTransform == null)
                continue;

            bool icindeMi = RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform,
                ekranPozisyonu,
                eventCamera
            );

            if (icindeMi)
            {
                Debug.Log("TahtaZemini ekran pozisyonundan bulundu: " + component.gameObject.name);
                return component;
            }
        }

        return null;
    }

    private bool TahtaZeminineHotbarIkonuGonder(
        MonoBehaviour tahtaZemini,
        Sprite ikon,
        Vector2 ekranPozisyonu,
        Camera eventCamera)
    {
        if (tahtaZemini == null)
            return false;

        MethodInfo method = tahtaZemini.GetType().GetMethod(
            "HotbarIkonunuTahtayaAs",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (method == null)
        {
            Debug.LogWarning(
                "TahtaZemini bulundu ama içinde HotbarIkonunuTahtayaAs(Sprite, Vector2, Camera) metodu yok."
            );

            return false;
        }

        object sonuc = method.Invoke(
            tahtaZemini,
            new object[] { ikon, ekranPozisyonu, eventCamera }
        );

        if (sonuc is bool boolSonuc)
            return boolSonuc;

        return false;
    }

    private void HotbarSlotunuBosaltmayaCalis(int globalSlotIndex)
    {
        MethodInfo method = hotbarSistemi.GetType().GetMethod(
            "GlobalSlotuBosalt",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (method != null)
        {
            method.Invoke(hotbarSistemi, new object[] { globalSlotIndex });
            return;
        }

        Debug.LogWarning(
            "Tahtaya referans oluşturuldu fakat HotbarSistemi içinde GlobalSlotuBosalt(int) metodu bulunamadı. " +
            "Bu yüzden hotbar slotu boşaltılamadı."
        );
    }
}