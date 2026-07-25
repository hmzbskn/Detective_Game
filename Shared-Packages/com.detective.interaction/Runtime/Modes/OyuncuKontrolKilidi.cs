using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Oyuncu kontrolünü (hareket/bakış/etkileşim scriptleri + cursor durumu) kilitleyen, referans
/// sayaçlı merkezi servis. Bilgisayar modu, cinayet tahtası, inceleme sistemi, fotoğraf modu ve
/// NPC diyaloğu gibi birbirinden habersiz "mod" sistemlerinin her biri kendi script-devre-dışı-
/// bırakma/cursor mantığını tekrar tekrar yazmak yerine bunu kullanır. Referans sayacı sayesinde
/// iç içe modlar (örn. cinayet tahtası açıkken bilgisayarı açmak) birbirinin durumunu ezmez: sadece
/// 0'dan 1'e geçişte scriptler kapatılır/cursor kaydedilir, sadece 1'den 0'a geçişte geri açılır.
/// </summary>
public class OyuncuKontrolKilidi : MonoBehaviour
{
    public static OyuncuKontrolKilidi Instance { get; private set; }

    private static int kilitSayisi = 0;
    public static bool KilitliMi => kilitSayisi > 0;

    [Header("Kilitlenecek/Açılacak Kontrol Scriptleri")]
    [Tooltip("Oyuncu hareket, bakış, etkileşim gibi scriptleri buraya ekle. Bir mod kilit istediğinde hepsi devre dışı bırakılır; kilit tamamen kalkınca her biri kendi önceki 'enabled' durumuna geri döner.")]
    [SerializeField] private MonoBehaviour[] kontrolScriptleri;

    private readonly Dictionary<MonoBehaviour, bool> oncekiScriptDurumlari = new Dictionary<MonoBehaviour, bool>();
    private CursorLockMode oncekiCursorLockState;
    private bool oncekiCursorVisible;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Kilidi bir seviye artırır. Sadece ilk çağrıda (0'dan 1'e geçişte) scriptleri kapatır ve
    /// cursor durumunu kaydeder. <paramref name="imleciSerbestBirak"/>: bazı modlar (örn. bilgisayar,
    /// cinayet tahtası, inceleme) cursor'ı serbest bırakıp görünür yapmak ister; bazıları (örn.
    /// fotoğraf modu) nişan alma tabanlı olduğu için cursor'ın kilitli kalmasını ister — bu yüzden
    /// sabit bir alan değil, çağıran tarafın belirlediği bir parametredir. <paramref name="haricTutulacaklar"/>:
    /// listede olsa bile bu çağrı için devre dışı bırakılmayacak scriptler (örn. fotoğraf modu, nişan
    /// alırken kameranın dönebilmesi için FirstPersonController'ı burada hariç tutar).
    /// </summary>
    public static void Kilitle(bool imleciSerbestBirak = true, MonoBehaviour[] haricTutulacaklar = null)
    {
        if (kilitSayisi == 0 && Instance != null)
            Instance.IlkKilidiUygula(imleciSerbestBirak, haricTutulacaklar);

        kilitSayisi++;
    }

    /// <summary>
    /// Kilidi bir seviye azaltır. Sadece son çağrıda (1'den 0'a geçişte) scriptleri ve cursor'ı
    /// kilitlenmeden önceki durumlarına geri yükler.
    /// </summary>
    public static void KilidiKaldir()
    {
        if (kilitSayisi <= 0)
        {
            kilitSayisi = 0;
            return;
        }

        kilitSayisi--;

        if (kilitSayisi == 0 && Instance != null)
            Instance.SonKilidiKaldir();
    }

    /// <summary>
    /// Sayaca bakmaksızın kilidi tamamen sıfırlar (örn. sahne geçişlerinde güvenlik amaçlı).
    /// </summary>
    public static void Sifirla()
    {
        bool aktifti = kilitSayisi > 0;
        kilitSayisi = 0;

        if (aktifti && Instance != null)
            Instance.SonKilidiKaldir();
    }

    private void IlkKilidiUygula(bool imleciSerbestBirak, MonoBehaviour[] haricTutulacaklar)
    {
        oncekiCursorLockState = Cursor.lockState;
        oncekiCursorVisible = Cursor.visible;

        oncekiScriptDurumlari.Clear();

        if (kontrolScriptleri != null)
        {
            for (int i = 0; i < kontrolScriptleri.Length; i++)
            {
                MonoBehaviour script = kontrolScriptleri[i];

                if (script == null)
                    continue;

                if (haricTutulacaklar != null && System.Array.IndexOf(haricTutulacaklar, script) >= 0)
                    continue;

                if (!oncekiScriptDurumlari.ContainsKey(script))
                    oncekiScriptDurumlari.Add(script, script.enabled);

                script.enabled = false;
            }
        }

        if (imleciSerbestBirak)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void SonKilidiKaldir()
    {
        foreach (KeyValuePair<MonoBehaviour, bool> kayit in oncekiScriptDurumlari)
        {
            if (kayit.Key != null)
                kayit.Key.enabled = kayit.Value;
        }

        oncekiScriptDurumlari.Clear();

        Cursor.lockState = oncekiCursorLockState;
        Cursor.visible = oncekiCursorVisible;
    }
}
