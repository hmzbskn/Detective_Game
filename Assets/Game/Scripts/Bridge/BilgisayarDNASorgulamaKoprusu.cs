using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BilgisayarDNASorgulamaKoprusu : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private HotbarSistemi hotbarSistemi;
    [SerializeField] private GameObject dnaSystem;
    [SerializeField] private DNAMiniGameManager dnaMiniGameManager;

    [Header("UI")]
    [SerializeField] private GameObject baslatButonu;
    [SerializeField] private TextMeshProUGUI durumText;

    [Header("Ayarlar")]
    [SerializeField] private bool debugMesajlari = true;

    private void Awake()
    {
        ReferanslariTamamla();
    }

    private void OnEnable()
    {
        ReferanslariTamamla();
        EkraniSifirla();
    }

    private void ReferanslariTamamla()
    {
        if (hotbarSistemi == null)
            hotbarSistemi = FindFirstObjectByType<HotbarSistemi>();

        if (dnaMiniGameManager == null && dnaSystem != null)
            dnaMiniGameManager = dnaSystem.GetComponentInChildren<DNAMiniGameManager>(true);
    }

    public void EkraniSifirla()
    {
        if (baslatButonu != null)
            baslatButonu.SetActive(true);

        if (durumText != null)
            durumText.text = "";

        if (dnaMiniGameManager != null)
            dnaMiniGameManager.AnaliziSifirla();
    }

    public void SorgulaVeKarsilastir()
    {
        if (debugMesajlari)
            Debug.Log("DNA sorgulama başlatıldı.");

        if (hotbarSistemi == null)
        {
            DurumYaz("Hotbar sistemi bulunamadı.");
            return;
        }

        if (dnaMiniGameManager == null)
        {
            DurumYaz("DNAMiniGameManager bulunamadı.");
            return;
        }

        List<ItemInstanceData> dnaDelilInstancelari =
            hotbarSistemi.HotbarVeEnvanterdekiDNADelilInstancelariniGetir();

        if (dnaDelilInstancelari == null || dnaDelilInstancelari.Count == 0)
        {
            DurumYaz("Hotbar veya envanterde DNA örneği içeren delil kulak çöpü bulunamadı.");
            return;
        }

        DNAData olayYeriDNA = null;
        List<DNAData> supheliDNAlar = new List<DNAData>();

        for (int i = 0; i < dnaDelilInstancelari.Count; i++)
        {
            ItemInstanceData instance = dnaDelilInstancelari[i];

            if (instance == null || instance.DNAData == null)
                continue;

            DNAData dna = instance.DNAData;

            if (dna.ornekTuru == DNAOrnekTuru.Ceset)
            {
                if (olayYeriDNA == null)
                {
                    olayYeriDNA = dna;

                    if (debugMesajlari)
                        Debug.Log("Olay yeri / ceset DNA bulundu: " + dna.sahibiAdi);
                }
            }
            else if (dna.ornekTuru == DNAOrnekTuru.Supheli)
            {
                supheliDNAlar.Add(dna);

                if (debugMesajlari)
                    Debug.Log("Şüpheli DNA bulundu: " + dna.sahibiAdi);
            }
        }

        if (olayYeriDNA == null)
        {
            DurumYaz("Olay yerinden / cesetten alınmış DNA örneği bulunamadı.");
            return;
        }

        if (supheliDNAlar.Count == 0)
        {
            DurumYaz("Şüphelilerden alınmış DNA örneği bulunamadı.");
            return;
        }

        if (baslatButonu != null)
            baslatButonu.SetActive(false);

        if (dnaSystem != null)
            dnaSystem.SetActive(true);

        DurumYaz($"DNA analizi başladı. 1 olay yeri DNA'sı ve {supheliDNAlar.Count} şüpheli DNA'sı bulundu.");

        dnaMiniGameManager.MiniGameBaslat(
            olayYeriDNA,
            supheliDNAlar.ToArray()
        );
    }

    private void DurumYaz(string mesaj)
    {
        if (durumText != null)
            durumText.text = mesaj;

        if (debugMesajlari)
            Debug.Log(mesaj);
    }
}