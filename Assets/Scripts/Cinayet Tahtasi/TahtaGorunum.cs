using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TahtaGorunum : MonoBehaviour
{
    [Header("Zoom Ayarları")]
    [SerializeField] private float zoomHizi = 0.15f;
    [SerializeField] private float minZoom = 1.0f;
    [SerializeField] private float maxZoom = 3.0f;

    [Header("Kaydırma Ayarları")]
    [SerializeField] private bool ortaTuslaKaydir = true;

    private RectTransform rectTransform;
    private Vector2 sonFarePozisyonu;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        // InputField'e yazı yazarken zoom/pan çalışmasın.
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != null &&
            EventSystem.current.currentSelectedGameObject.GetComponentInParent<TMP_InputField>() != null)
        {
            return;
        }

        Vector2 farePozisyonu = Mouse.current.position.ReadValue();

        ZoomKontrol(farePozisyonu);
        PanKontrol(farePozisyonu);
    }

    private void ZoomKontrol(Vector2 farePozisyonu)
    {
        float scrollHam = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scrollHam) < 0.01f)
            return;

        // Bazı sistemlerde scroll 120 gelir, bazılarında 1 gelir.
        // Bu satır ikisini de aynı mantığa çeker.
        float scroll = Mathf.Abs(scrollHam) > 10f ? scrollHam / 120f : scrollHam;

        Vector3 eskiScale = rectTransform.localScale;

        float hedefScale = eskiScale.x + scroll * zoomHizi;
        float sinirliScale = Mathf.Clamp(hedefScale, minZoom, maxZoom);

        Vector3 yeniScale = new Vector3(sinirliScale, sinirliScale, 1f);

        if (Mathf.Approximately(eskiScale.x, yeniScale.x))
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            farePozisyonu,
            null,
            out Vector2 fareYerelPozisyon
        );

        rectTransform.localScale = yeniScale;

        Vector3 kaymaMiktari = new Vector3(
            fareYerelPozisyon.x * (yeniScale.x - eskiScale.x),
            fareYerelPozisyon.y * (yeniScale.y - eskiScale.y),
            0f
        );

        rectTransform.localPosition -= kaymaMiktari;

        if (sinirliScale <= minZoom + 0.001f)
        {
            rectTransform.localPosition = Vector3.zero;
        }

        Debug.Log("Tahta zoom: " + sinirliScale);
    }

    private void PanKontrol(Vector2 farePozisyonu)
    {
        if (!ortaTuslaKaydir)
            return;

        if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            sonFarePozisyonu = farePozisyonu;
            return;
        }

        if (!Mouse.current.middleButton.isPressed)
            return;

        if (rectTransform.localScale.x <= minZoom)
        {
            sonFarePozisyonu = farePozisyonu;
            return;
        }

        Vector2 delta = farePozisyonu - sonFarePozisyonu;
        rectTransform.position += new Vector3(delta.x, delta.y, 0f);

        sonFarePozisyonu = farePozisyonu;
    }
}