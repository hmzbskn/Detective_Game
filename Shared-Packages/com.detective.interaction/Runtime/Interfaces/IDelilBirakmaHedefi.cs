using UnityEngine;

/// <summary>
/// Bir delil/eşya sürükle-bırak hedefinin uygulaması gereken sözleşme (örn. cinayet tahtası zemini).
/// Detective.Interaction paketi Assembly-CSharp'taki somut sınıfları (TahtaZemini gibi) derleme
/// zamanında göremediği için, bu ihtiyaç bir arayüzle çözülür: hedef sınıf (Assembly-CSharp'ta) bu
/// arayüzü uygular, sürükleyen taraf (Interaction paketinde) GetComponent&lt;IDelilBirakmaHedefi&gt;()
/// ile bulur — sahne taraması veya reflection gerekmez.
/// </summary>
public interface IDelilBirakmaHedefi
{
    /// <summary>
    /// Bir delili bu hedefe bırakmayı dener. Hedef, delili kabul edip etmeyeceğine kendi karar verir
    /// (örn. yalnızca DelilMi=true olan itemleri kabul etmek gibi).
    /// </summary>
    /// <param name="delil">Bırakılan eşyanın tam runtime instance verisi (DNA/fotoğraf verisi dahil).</param>
    /// <param name="ikon">Görsel için hazır ikon (delil.IkonGetir() ile aynı, çağıran tarafından hazırlanır).</param>
    /// <param name="ekranPozisyonu">Bırakma anındaki ekran koordinatı.</param>
    /// <param name="eventCamera">Sürükleme olayının kamerası (UI dönüşümü için).</param>
    /// <returns>Bırakma kabul edildiyse true; çağıran taraf bu durumda kaynağı (slot vb.) temizlemelidir.</returns>
    bool DeliliBirak(ItemInstanceData delil, Sprite ikon, Vector2 ekranPozisyonu, Camera eventCamera);
}
