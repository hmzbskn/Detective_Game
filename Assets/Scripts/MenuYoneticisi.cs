using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuYoneticisi : MonoBehaviour
{
    [System.Serializable]
    public class GizlenecekUI
    {
        [Tooltip("ESC ile duraklatma menüsü açılınca gizlenecek UI objesi.")]
        public GameObject uiObjesi;

        [Tooltip("Oyuna devam edilince bu UI eski aktiflik durumuna geri dönsün mü?")]
        public bool devamEdinceEskiHalineDonsun = false;

        [HideInInspector] public bool oncekiAktiflikDurumu;
    }

    [Header("UI Ayarları")]
    [SerializeField] private GameObject duraklatmaMenusu;

    [Header("ESC Basınca Gizlenecek UI Objeleri")]
    [SerializeField] private List<GizlenecekUI> escBasincaGizlenecekler = new List<GizlenecekUI>();

    [Header("Karakter Ayarları")]
    [Tooltip("Starter Assets karakterini buraya sürükle. Üzerinde PlayerInput olan obje.")]
    [SerializeField] private PlayerInput oyuncuInput;

    private bool oyunDurduMu = false;

    private void Start()
    {
        if (duraklatmaMenusu != null)
            duraklatmaMenusu.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (oyunDurduMu)
                OyunaDevamEt();
            else
                OyunuDuraklat();
        }
    }

    public void OyunuDuraklat()
    {
        oyunDurduMu = true;

        GizlenecekUIObjeleriniKapat();

        if (duraklatmaMenusu != null)
            duraklatmaMenusu.SetActive(true);

        Time.timeScale = 0f;

        if (oyuncuInput != null)
            oyuncuInput.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OyunaDevamEt()
    {
        oyunDurduMu = false;

        if (duraklatmaMenusu != null)
            duraklatmaMenusu.SetActive(false);

        GizlenenUIObjeleriniGeriYukle();

        Time.timeScale = 1f;

        if (oyuncuInput != null)
            oyuncuInput.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OyundanCik()
    {
        Debug.Log("Oyundan çıkılıyor...");

        Time.timeScale = 1f;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void GizlenecekUIObjeleriniKapat()
    {
        foreach (GizlenecekUI kayit in escBasincaGizlenecekler)
        {
            if (kayit == null || kayit.uiObjesi == null)
                continue;

            kayit.oncekiAktiflikDurumu = kayit.uiObjesi.activeSelf;
            kayit.uiObjesi.SetActive(false);
        }
    }

    private void GizlenenUIObjeleriniGeriYukle()
    {
        foreach (GizlenecekUI kayit in escBasincaGizlenecekler)
        {
            if (kayit == null || kayit.uiObjesi == null)
                continue;

            if (kayit.devamEdinceEskiHalineDonsun)
                kayit.uiObjesi.SetActive(kayit.oncekiAktiflikDurumu);
        }
    }

    public void GizlenecekUIEkle(GameObject uiObjesi, bool devamEdinceEskiHalineDonsun = false)
    {
        if (uiObjesi == null)
            return;

        foreach (GizlenecekUI kayit in escBasincaGizlenecekler)
        {
            if (kayit.uiObjesi == uiObjesi)
                return;
        }

        GizlenecekUI yeniKayit = new GizlenecekUI
        {
            uiObjesi = uiObjesi,
            devamEdinceEskiHalineDonsun = devamEdinceEskiHalineDonsun
        };

        escBasincaGizlenecekler.Add(yeniKayit);
    }
}