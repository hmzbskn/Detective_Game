using System.Collections.Generic;
using UnityEngine;

public class NPCKonusmaKilidi : MonoBehaviour
{
    [Header("Konuşma sırasında gizlenecek HUD objeleri")]
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private GameObject hotbarUI;

    [Header("Konuşma sırasında gizlenecek ekstra UI objeleri")]
    [SerializeField] private GameObject etkilesimPaneli;

    public bool KonusmaKilidiAktifMi { get; private set; }

    private readonly Dictionary<GameObject, bool> oncekiUIDurumlari = new Dictionary<GameObject, bool>();

    private void Start()
    {
        KonusmaKilidiAktifMi = false;
    }

    public void KonusmaKilidiniAc()
    {
        if (KonusmaKilidiAktifMi)
            return;

        KonusmaKilidiAktifMi = true;

        UIObjeleriniGizle();
        OyuncuKontrolKilidi.Kilitle();
    }

    public void KonusmaKilidiniKapat()
    {
        if (!KonusmaKilidiAktifMi)
            return;

        KonusmaKilidiAktifMi = false;

        UIObjeleriniGeriYukle();
        OyuncuKontrolKilidi.KilidiKaldir();
    }

    private void UIObjeleriniGizle()
    {
        oncekiUIDurumlari.Clear();

        UIObjesiniGizle(crosshairUI);
        UIObjesiniGizle(hotbarUI);
        UIObjesiniGizle(etkilesimPaneli);
    }

    private void UIObjesiniGizle(GameObject uiObjesi)
    {
        if (uiObjesi == null)
            return;

        if (!oncekiUIDurumlari.ContainsKey(uiObjesi))
            oncekiUIDurumlari.Add(uiObjesi, uiObjesi.activeSelf);

        uiObjesi.SetActive(false);
    }

    private void UIObjeleriniGeriYukle()
    {
        foreach (KeyValuePair<GameObject, bool> kayit in oncekiUIDurumlari)
        {
            if (kayit.Key == null)
                continue;

            kayit.Key.SetActive(kayit.Value);
        }

        oncekiUIDurumlari.Clear();
    }
}
