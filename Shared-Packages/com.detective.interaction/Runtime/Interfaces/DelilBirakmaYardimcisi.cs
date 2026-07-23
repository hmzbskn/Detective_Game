using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Bir sürükleme işleminin bittiği ekran pozisyonunda IDelilBirakmaHedefi uygulayan bir UI hedefini
/// bulmak için kullanılan ortak yardımcı. HotbarSlotUI ve EnvanterSlotUI arasında aynı arama
/// mantığının tekrarlanmasını önler. Sürükleme sırasında görsel önizleme (DragIkon) genelde
/// raycastTarget=false olduğundan Unity'nin normal OnDrop mekanizması hedefe ulaşmaz; bu yüzden
/// EventSystem'in kendi UI raycaster'ı üzerinden bırakma noktasındaki gerçek hedefler taranır.
/// </summary>
public static class DelilBirakmaYardimcisi
{
    public static IDelilBirakmaHedefi EkranPozisyonundakiHedefiBul(PointerEventData eventData)
    {
        if (EventSystem.current == null)
            return null;

        List<RaycastResult> sonuclar = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, sonuclar);

        for (int i = 0; i < sonuclar.Count; i++)
        {
            GameObject hedefObje = sonuclar[i].gameObject;

            if (hedefObje == null)
                continue;

            IDelilBirakmaHedefi hedef = hedefObje.GetComponentInParent<IDelilBirakmaHedefi>();

            if (hedef != null)
                return hedef;
        }

        return null;
    }
}
