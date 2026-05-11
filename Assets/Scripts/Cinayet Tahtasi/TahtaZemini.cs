using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// DİKKAT: Artık tıklamayı (IPointerClickHandler) değil, bırakmayı (IDropHandler) dinliyoruz!
public class TahtaZemini : MonoBehaviour, IDropHandler
{
    [Header("Ayarlar")]
    [Tooltip("Tahtaya asıldığında oluşacak o fotoğraf prefabı")]
    public GameObject delilUIPrefab;

    public void OnDrop(PointerEventData eventData)
    {
        // Fareyle tutup panonun üzerine bıraktığımız orijinal objeyi (Slotu) bul
        if (eventData.pointerDrag != null)
        {
            EnvanterSlot gelenSlot = eventData.pointerDrag.GetComponent<EnvanterSlot>();

            // Eğer üzerimize bırakılan şey gerçekten dolu bir envanter slotuysa...
            if (gelenSlot != null && !gelenSlot.BosMu())
            {
                EsyaVerisi gelenEsya = gelenSlot.EsyaGetir();

                // Eşyanın delil kutucuğu işaretli mi diye kontrol et
                if (gelenEsya.delilMi)
                {
                    // 1. Yeni fotoğrafı fareyi tam bıraktığımız noktada (eventData.position) yarat
                    GameObject yeniDelil = Instantiate(delilUIPrefab, transform);
                    yeniDelil.transform.position = eventData.position;

                    // 2. Fotoğrafın resmini, envanterden gelen eşyanın ikonuyla değiştir
                    Image resim = yeniDelil.GetComponent<Image>();
                    if (resim != null && gelenEsya.esyaIkonu != null)
                    {
                        resim.sprite = gelenEsya.esyaIkonu;
                    }

                    // 3. Eşyayı envanterdeki o slottan tamamen SİL
                    gelenSlot.SlotuBosalt();

                    // 4. Eğer oyuncu az önce tahtaya astığı eşyayı o an elinde de tutuyorsa, 3D dünyadaki elini de boşalt!
                    EsyaKusanma oyuncu = FindFirstObjectByType<EsyaKusanma>();
                    if (oyuncu != null)
                    {
                        oyuncu.EldekiNesneyiYenile();
                    }

                    Debug.Log(gelenEsya.esyaAdi + " tahtaya başarıyla sürüklendi!");
                }
                else
                {
                    Debug.LogWarning("Bu eşya tahtaya asılamaz! (Sadece deliller asılabilir)");
                }
            }
        }
    }
}