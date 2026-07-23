using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FotografMakinesiController : MonoBehaviour
{
    [Header("Sistemler")]
    [SerializeField] private HotbarSistemi hotbarSistemi;
    [SerializeField] private IncelemeSistemi incelemeSistemi;
    [SerializeField] private FotografModuSistemi fotografModuSistemi;
    [SerializeField] private FotografCekimSistemi fotografCekimSistemi;

    [Header("Item Data")]
    [SerializeField] private ItemData fotografMakinesiItem;
    [SerializeField] private ItemData bosFotografKagidiItem;
    [SerializeField] private ItemData basiliFotografItem;

    [Header("Tuşlar - New Input System")]
    [SerializeField] private Key fotografModuTusu = Key.Q;

    [Header("Çekim Ayarları")]
    [SerializeField] private float cekimBeklemeSuresi = 0.35f;
    [SerializeField] private bool kagitYoksaModdanCik = false;

    [Header("Efekt")]
    [SerializeField] private CanvasGroup flashCanvasGroup;
    [SerializeField] private float flashSuresi = 0.15f;

    [Header("Debug")]
    [SerializeField] private bool debugMesajlari = true;

    private bool cekimBeklemede;
    private Coroutine flashRoutine;

    private void Awake()
    {
        if (hotbarSistemi == null)
            hotbarSistemi = FindFirstObjectByType<HotbarSistemi>();

        if (incelemeSistemi == null)
            incelemeSistemi = FindFirstObjectByType<IncelemeSistemi>();

        if (fotografModuSistemi == null)
            fotografModuSistemi = FindFirstObjectByType<FotografModuSistemi>();

        if (fotografCekimSistemi == null)
            fotografCekimSistemi = FindFirstObjectByType<FotografCekimSistemi>();

        if (flashCanvasGroup != null)
            flashCanvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        if (TusBasildi(fotografModuTusu))
        {
            FotografModunuToggle();
        }

        if (fotografModuSistemi != null && fotografModuSistemi.ModAktifMi)
        {
            if (!FotografModuKullanilabilirMi())
            {
                FotografModunuKapat();
                return;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                FotografCekVeEnvantereEkle();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                FotografModunuKapat();
            }
        }
    }

    private bool TusBasildi(Key key)
    {
        if (Keyboard.current == null)
            return false;

        var tus = Keyboard.current[key];

        if (tus == null)
            return false;

        return tus.wasPressedThisFrame;
    }

    private void FotografModunuToggle()
    {
        if (fotografModuSistemi == null)
            return;

        if (fotografModuSistemi.ModAktifMi)
        {
            FotografModunuKapat();
            return;
        }

        if (!FotografModuKullanilabilirMi())
        {
            if (debugMesajlari)
                Debug.Log("Fotoğraf modu açılamadı. Fotoğraf makinesi seçili değil, envanter açık olabilir veya inceleme modu aktif olabilir.");

            return;
        }

        FotografModunuAc();
    }

    private void FotografModunuAc()
    {
        if (fotografModuSistemi == null)
            return;

        fotografModuSistemi.Ac();

        if (debugMesajlari)
            Debug.Log("Fotoğraf modu açıldı.");
    }

    private void FotografModunuKapat()
    {
        if (fotografModuSistemi == null)
            return;

        fotografModuSistemi.Kapat();

        if (debugMesajlari)
            Debug.Log("Fotoğraf modu kapatıldı.");
    }

    private bool FotografModuKullanilabilirMi()
    {
        if (hotbarSistemi == null)
            return false;

        bool baskaBirModAktif = OyuncuKontrolKilidi.KilitliMi &&
            (fotografModuSistemi == null || !fotografModuSistemi.ModAktifMi);

        if (baskaBirModAktif)
            return false;

        if (hotbarSistemi.EnvanterAcikMi())
            return false;

        if (incelemeSistemi != null && incelemeSistemi.IncelemeAktifMi)
            return false;

        if (fotografMakinesiItem == null)
            return false;

        if (!hotbarSistemi.AktifHotbarItemMi(fotografMakinesiItem))
            return false;

        return true;
    }

    private void FotografCekVeEnvantereEkle()
    {
        if (cekimBeklemede)
            return;

        if (hotbarSistemi == null || fotografCekimSistemi == null)
            return;

        if (bosFotografKagidiItem == null)
        {
            Debug.LogWarning("Boş fotoğraf kağıdı ItemData atanmamış.");
            return;
        }

        if (basiliFotografItem == null)
        {
            Debug.LogWarning("Basılı fotoğraf ItemData atanmamış.");
            return;
        }

        FotografCekimTransaksiyonu transaksiyon = new FotografCekimTransaksiyonu(
            hotbarSistemi,
            fotografCekimSistemi,
            bosFotografKagidiItem,
            basiliFotografItem
        );

        FotografCekimTransaksiyonu.Sonuc sonuc = transaksiyon.Uygula();

        switch (sonuc)
        {
            case FotografCekimTransaksiyonu.Sonuc.KagitYok:
                if (debugMesajlari)
                    Debug.Log("Fotoğraf çekilemedi. Envanterde boş fotoğraf kağıdı yok.");

                if (kagitYoksaModdanCik)
                    FotografModunuKapat();

                return;

            case FotografCekimTransaksiyonu.Sonuc.CekimBasarisiz:
                Debug.LogWarning("Fotoğraf çekilemedi. Fotoğraf kaydı oluşturulamadı.");
                return;

            case FotografCekimTransaksiyonu.Sonuc.KagitTuketilemedi:
                Debug.LogWarning("Fotoğraf çekildi ama fotoğraf kağıdı tüketilemedi. Basılı fotoğraf oluşturulmadı.");
                return;

            case FotografCekimTransaksiyonu.Sonuc.EnvantereEklenemedi:
                Debug.LogWarning("Basılı fotoğraf envantere eklenemedi. Envanter dolu olabilir. Kağıt geri verilmeye çalışılıyor.");

                if (transaksiyon.KagitRollbackBasarisizMi)
                    Debug.LogWarning("Fotoğraf kağıdı geri verilemedi. Envanter sistemi kontrol edilmeli.");

                return;

            case FotografCekimTransaksiyonu.Sonuc.Basarili:
                FlashEfektiBaslat();

                if (debugMesajlari)
                    Debug.Log("Fotoğraf çekildi. 1 fotoğraf kağıdı tüketildi. Basılı fotoğraf envantere eklendi.");

                StartCoroutine(CekimBeklemeRutini());
                return;
        }
    }

    private IEnumerator CekimBeklemeRutini()
    {
        cekimBeklemede = true;

        yield return new WaitForSeconds(cekimBeklemeSuresi);

        cekimBeklemede = false;
    }

    private void FlashEfektiBaslat()
    {
        if (flashCanvasGroup == null)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashEfekti());
    }

    private IEnumerator FlashEfekti()
    {
        flashCanvasGroup.alpha = 1f;

        float zaman = 0f;

        while (zaman < flashSuresi)
        {
            zaman += Time.deltaTime;
            flashCanvasGroup.alpha = Mathf.Lerp(1f, 0f, zaman / flashSuresi);
            yield return null;
        }

        flashCanvasGroup.alpha = 0f;
    }
}