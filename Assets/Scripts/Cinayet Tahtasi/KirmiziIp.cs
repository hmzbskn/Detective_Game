using UnityEngine;
using TMPro;

public class KirmiziIp : MonoBehaviour
{
    public RectTransform Baslangic { get; private set; }
    public RectTransform Bitis { get; private set; }

    private RectTransform ipRect;
    private TextMeshProUGUI baglantiYazisi;

    private void Awake()
    {
        ipRect = GetComponent<RectTransform>();
        baglantiYazisi = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UclariBagla(RectTransform baslangic, RectTransform bitis)
    {
        Baslangic = baslangic;
        Bitis = bitis;
    }

    public void YaziyiSetEt(string yazi)
    {
        if (baglantiYazisi != null)
        {
            baglantiYazisi.text = yazi;
        }
    }

    private void Update()
    {
        if (Baslangic == null || Bitis == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 basNoktasi = BaglantiNoktasiniBul(Baslangic);
        Vector3 bitisNoktasi = BaglantiNoktasiniBul(Bitis);

        ipRect.position = basNoktasi;

        Vector3 yerelBas = ipRect.parent.InverseTransformPoint(basNoktasi);
        Vector3 yerelBitis = ipRect.parent.InverseTransformPoint(bitisNoktasi);

        float yerelMesafe = Vector3.Distance(yerelBas, yerelBitis);

        ipRect.sizeDelta = new Vector2(yerelMesafe, ipRect.sizeDelta.y);

        Vector3 yon = bitisNoktasi - basNoktasi;
        float aci = Mathf.Atan2(yon.y, yon.x) * Mathf.Rad2Deg;

        ipRect.rotation = Quaternion.Euler(0f, 0f, aci);

        if (baglantiYazisi != null)
        {
            Vector3 ortaNokta = Vector3.Lerp(basNoktasi, bitisNoktasi, 0.5f);
            baglantiYazisi.rectTransform.position = ortaNokta;
            baglantiYazisi.rectTransform.rotation = ipRect.rotation;

            if (bitisNoktasi.x < basNoktasi.x)
            {
                baglantiYazisi.rectTransform.Rotate(0f, 0f, 180f);
            }

            float yaziGenisligi = Mathf.Max(0f, yerelMesafe - 40f);
            baglantiYazisi.rectTransform.sizeDelta = new Vector2(
                yaziGenisligi,
                baglantiYazisi.rectTransform.sizeDelta.y
            );
        }
    }

    private Vector3 BaglantiNoktasiniBul(RectTransform rectT)
    {
        float xNoktasi = rectT.rect.center.x;
        float ceyrekBoy = rectT.rect.height * 0.25f;
        float yNoktasi = rectT.rect.yMax - ceyrekBoy;

        Vector3 yerelNokta = new Vector3(xNoktasi, yNoktasi, 0f);

        return rectT.TransformPoint(yerelNokta);
    }
}