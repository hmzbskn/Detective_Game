using UnityEngine;
using TMPro; // TextMeshPro kullanmak için gerekli

public class KirmiziIp : MonoBehaviour
{
    public RectTransform Baslangic { get; private set; }
    public RectTransform Bitis { get; private set; }

    private RectTransform ipRect;
    // İpin üzerindeki metin bileşeni
    private TextMeshProUGUI baglantiYazisi;

    void Awake()
    {
        ipRect = GetComponent<RectTransform>();
        // Prefabın içindeki metni bulup hafızaya al
        baglantiYazisi = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UclariBagla(RectTransform baslangic, RectTransform bitis)
    {
        Baslangic = baslangic;
        Bitis = bitis;
    }

    // İp yöneticisi bu fonksiyonla yazıyı ayarlayacak
    public void YaziyiSetEt(string yazi)
    {
        if (baglantiYazisi != null)
        {
            baglantiYazisi.text = yazi;
        }
    }

    void Update()
    {
        if (Baslangic == null || Bitis == null) return;

        Vector3 basNoktasi = BaglantiNoktasiniBul(Baslangic);
        Vector3 bitisNoktasi = BaglantiNoktasiniBul(Bitis);

        ipRect.position = basNoktasi;

        Vector3 yerelBas = ipRect.parent.InverseTransformPoint(basNoktasi);
        Vector3 yerelBitis = ipRect.parent.InverseTransformPoint(bitisNoktasi);
        float yerelMesafe = Vector3.Distance(yerelBas, yerelBitis);

        ipRect.sizeDelta = new Vector2(yerelMesafe, ipRect.sizeDelta.y);

        Vector3 yon = bitisNoktasi - basNoktasi;
        float aci = Mathf.Atan2(yon.y, yon.x) * Mathf.Rad2Deg;
        ipRect.rotation = Quaternion.Euler(0, 0, aci);

        if (baglantiYazisi != null)
        {
            // 1. Yazıyı iki noktanın tam ortasına yerleştir
            Vector3 ortaNokta = Vector3.Lerp(basNoktasi, bitisNoktasi, 0.5f);
            baglantiYazisi.rectTransform.position = ortaNokta;

            // 2. Önce ipin rotasyonunu kopyala
            baglantiYazisi.rectTransform.rotation = ipRect.rotation;

            // 3. OKUNABİLİRLİK KONTROLÜ (Her Zaman Soldan Sağa):
            // Eğer bitiş noktası başlangıç noktasından daha soldaysa (X değeri küçükse),
            // yazı ters (sağdan sola) gidiyor demektir. 180 derece çevirerek düzeltiyoruz.
            if (bitisNoktasi.x < basNoktasi.x)
            {
                baglantiYazisi.rectTransform.Rotate(0, 0, 180);
            }

            // 4. Yazı kutusunun genişliğini (Auto-Size için) ayarla
            float yaziGenisligi = Mathf.Max(0, yerelMesafe - 40f);
            baglantiYazisi.rectTransform.sizeDelta = new Vector2(yaziGenisligi, baglantiYazisi.rectTransform.sizeDelta.y);
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