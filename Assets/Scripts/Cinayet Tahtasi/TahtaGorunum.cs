using UnityEngine;

public class TahtaGorunum : MonoBehaviour
{
    [Header("Zoom Ayarları")]
    public float zoomHizi = 0.1f;
    public float minZoom = 1.0f;  // Senin ayarladığın gibi 1 yaptık
    public float maxZoom = 3.0f;

    private RectTransform rectTransform;
    private Vector3 sonFarePozisyonu;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // 1. ZOOM (Fare Tekerleği İle)
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 eskiScale = rectTransform.localScale;
            float hedefScale = eskiScale.x + scroll * zoomHizi;
            float sinirliScale = Mathf.Clamp(hedefScale, minZoom, maxZoom);
            Vector3 yeniScale = new Vector3(sinirliScale, sinirliScale, 1f);

            if (eskiScale.x != yeniScale.x)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out Vector2 fareYerelPozisyon);
                rectTransform.localScale = yeniScale;

                Vector3 kaymaMiktari = new Vector3(
                    fareYerelPozisyon.x * (yeniScale.x - eskiScale.x),
                    fareYerelPozisyon.y * (yeniScale.y - eskiScale.y),
                    0f
                );

                rectTransform.localPosition -= kaymaMiktari;

                // BÜYÜK ÇÖZÜM 1: Zoom en geri çekildiğinde tahtayı şaak diye merkeze oturt!
                if (sinirliScale <= minZoom)
                {
                    rectTransform.localPosition = Vector3.zero;
                }
            }
        }

        // 2. KAYDIRMA (Pan) - Farenin ORTA TUŞUNA (Tekerleğe) Basılı Tutarak
        if (Input.GetMouseButtonDown(2))
        {
            sonFarePozisyonu = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2))
        {
            // BÜYÜK ÇÖZÜM 2: Sadece tahta zoomlanmışsa (büyütülmüşse) kaydırmaya izin ver!
            // Böylece 1x modundayken yanlışlıkla tahtayı kenara çekip arkasını göremezsin.
            if (rectTransform.localScale.x > minZoom)
            {
                Vector3 delta = Input.mousePosition - sonFarePozisyonu;
                rectTransform.position += delta;
            }

            sonFarePozisyonu = Input.mousePosition;
        }
    }
}