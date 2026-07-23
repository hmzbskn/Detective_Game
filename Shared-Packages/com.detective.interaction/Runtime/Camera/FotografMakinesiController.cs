using System.Collections;
using System.Collections.Generic;
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

    [Header("Bu mod acilinca kapatilacak scriptler")]
    [SerializeField] private List<MonoBehaviour> kapatilacakScriptler = new List<MonoBehaviour>();

    [Header("Efekt")]
    [SerializeField] private CanvasGroup flashCanvasGroup;
    [SerializeField] private float flashSuresi = 0.15f;

    [Header("Debug")]
    [SerializeField] private bool debugMesajlari = true;

    private bool cekimBeklemede;
    private Coroutine flashRoutine;
    private Dictionary<MonoBehaviour, bool> oncekiScriptDurumlari = new Dictionary<MonoBehaviour, bool>();

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
        ScriptleriKapat();

        if (debugMesajlari)
            Debug.Log("Fotoğraf modu açıldı.");
    }

    private void FotografModunuKapat()
    {
        if (fotografModuSistemi == null)
            return;

        fotografModuSistemi.Kapat();
        ScriptleriAc();

        if (debugMesajlari)
            Debug.Log("Fotoğraf modu kapatıldı.");
    }

    private bool FotografModuKullanilabilirMi()
    {
        if (hotbarSistemi == null)
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

        if (!hotbarSistemi.ItemVarMi(bosFotografKagidiItem))
        {
            if (debugMesajlari)
                Debug.Log("Fotoğraf çekilemedi. Envanterde boş fotoğraf kağıdı yok.");

            if (kagitYoksaModdanCik)
                FotografModunuKapat();

            return;
        }

        FotografKaydi kayit = fotografCekimSistemi.FotografCek();

        if (kayit == null)
        {
            Debug.LogWarning("Fotoğraf çekilemedi. Fotoğraf kaydı oluşturulamadı.");
            return;
        }

        bool kagitTuketildi = hotbarSistemi.ItemdenBirAdetAzalt(bosFotografKagidiItem);

        if (!kagitTuketildi)
        {
            Debug.LogWarning("Fotoğraf çekildi ama fotoğraf kağıdı tüketilemedi. Basılı fotoğraf oluşturulmadı.");
            return;
        }

        ItemInstanceData basiliFotografInstance = new ItemInstanceData(
            basiliFotografItem,
            kayit
        );

        bool fotografEklendi = hotbarSistemi.ItemInstanceEkle(basiliFotografInstance);

        if (!fotografEklendi)
        {
            Debug.LogWarning("Basılı fotoğraf envantere eklenemedi. Envanter dolu olabilir. Kağıt geri verilmeye çalışılıyor.");

            bool kagitGeriVerildi = hotbarSistemi.ItemEkle(bosFotografKagidiItem, 1);

            if (!kagitGeriVerildi)
                Debug.LogWarning("Fotoğraf kağıdı geri verilemedi. Envanter sistemi kontrol edilmeli.");

            return;
        }

        FlashEfektiBaslat();

        if (debugMesajlari)
            Debug.Log("Fotoğraf çekildi. 1 fotoğraf kağıdı tüketildi. Basılı fotoğraf envantere eklendi.");

        StartCoroutine(CekimBeklemeRutini());
    }

    private void ScriptleriKapat()
    {
        oncekiScriptDurumlari.Clear();

        for (int i = 0; i < kapatilacakScriptler.Count; i++)
        {
            MonoBehaviour script = kapatilacakScriptler[i];

            if (script == null)
                continue;

            if (script == this)
                continue;

            if (!oncekiScriptDurumlari.ContainsKey(script))
                oncekiScriptDurumlari.Add(script, script.enabled);

            script.enabled = false;
        }
    }

    private void ScriptleriAc()
    {
        foreach (KeyValuePair<MonoBehaviour, bool> kayit in oncekiScriptDurumlari)
        {
            if (kayit.Key == null)
                continue;

            kayit.Key.enabled = kayit.Value;
        }

        oncekiScriptDurumlari.Clear();
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