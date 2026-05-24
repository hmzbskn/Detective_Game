using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TahtaDelili : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("UI Referansları")]
    [SerializeField] private Image delilResmi;
    [SerializeField] private TMP_InputField etiketInput;

    [Header("Etiket Ayarları")]
    [SerializeField] private int maksimumKarakter = 30;

    public EsyaVerisi DelilVerisi { get; private set; }

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform tahtaRect;

    private bool inputUzerindenSuruklemeBasladi = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (transform.parent != null)
            tahtaRect = transform.parent.GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (delilResmi == null)
        {
            Transform resimChild = transform.Find("Resim");

            if (resimChild != null)
                delilResmi = resimChild.GetComponent<Image>();
        }

        if (etiketInput == null)
        {
            Transform inputChild = transform.Find("EtiketInput");

            if (inputChild != null)
                etiketInput = inputChild.GetComponent<TMP_InputField>();
        }

        if (etiketInput != null)
        {
            etiketInput.characterLimit = maksimumKarakter;
            etiketInput.lineType = TMP_InputField.LineType.SingleLine;
        }
    }

    public void DelilVerisiniAta(EsyaVerisi esyaVerisi)
    {
        DelilVerisi = esyaVerisi;
    }

    public void IcerigiAyarla(Sprite ikon, string baslangicMetni = "")
    {
        if (delilResmi != null)
        {
            delilResmi.sprite = ikon;
            delilResmi.preserveAspect = true;
            delilResmi.enabled = ikon != null;
        }

        if (etiketInput != null)
        {
            etiketInput.characterLimit = maksimumKarakter;
            etiketInput.text = baslangicMetni;
        }
    }

    public string EtiketMetniniGetir()
    {
        if (etiketInput == null)
            return "";

        return etiketInput.text.Trim();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        inputUzerindenSuruklemeBasladi = InputAlaninaTiklandiMi(eventData);

        if (inputUzerindenSuruklemeBasladi)
            return;

        rectTransform.SetAsLastSibling();
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inputUzerindenSuruklemeBasladi)
            return;

        if (canvas == null)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        SinirlarinIcindeTut();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (inputUzerindenSuruklemeBasladi)
        {
            inputUzerindenSuruklemeBasladi = false;
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.SetAsLastSibling();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InputAlaninaTiklandiMi(eventData))
            return;

        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (IpYoneticisi.Instance != null)
        {
            IpYoneticisi.Instance.DelileTiklandi(rectTransform);
        }
    }

    private bool InputAlaninaTiklandiMi(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerEnter == null)
            return false;

        return eventData.pointerEnter.GetComponentInParent<TMP_InputField>() != null;
    }

    private void SinirlarinIcindeTut()
    {
        if (tahtaRect == null)
            return;

        Vector3 pozisyon = rectTransform.localPosition;

        float minX = tahtaRect.rect.xMin + rectTransform.rect.width * rectTransform.pivot.x;
        float maxX = tahtaRect.rect.xMax - rectTransform.rect.width * (1f - rectTransform.pivot.x);

        float minY = tahtaRect.rect.yMin + rectTransform.rect.height * rectTransform.pivot.y;
        float maxY = tahtaRect.rect.yMax - rectTransform.rect.height * (1f - rectTransform.pivot.y);

        pozisyon.x = Mathf.Clamp(pozisyon.x, minX, maxX);
        pozisyon.y = Mathf.Clamp(pozisyon.y, minY, maxY);

        rectTransform.localPosition = pozisyon;
    }
}