using UnityEngine;
using UnityEngine.UI; // UI InputField'ı kullanmak için
using TMPro; // TextMeshPro kullanmak için
using System.Collections.Generic;

public class IpYoneticisi : MonoBehaviour
{
    public static IpYoneticisi Instance;

    [Header("İp Ayarları")]
    public GameObject ipPrefab;
    public Transform ipKonteyneri;

    [Header("UI Ayarları")]
    // YENİ: Eklediğimiz InputField'ı buraya sürükleyeceğiz
    public TMP_InputField baglantiInputField;

    private RectTransform baslangicNoktasi;

    // Geçici hafıza: İpin başını ve sonunu, yazı yazılana kadar aklımızda tutalım
    private RectTransform geciciBas, geciciBitis;

    private List<KirmiziIp> cekilenIpler = new List<KirmiziIp>();

    void Awake()
    {
        Instance = this;
    }

    public void DelileTiklandi(RectTransform tiklananDelil)
    {
        if (baslangicNoktasi == null)
        {
            baslangicNoktasi = tiklananDelil;
            Debug.Log("İpin ucu bağlandı: " + tiklananDelil.gameObject.name);
        }
        else
        {
            if (baslangicNoktasi != tiklananDelil)
            {
                if (!IpZatenVarMi(baslangicNoktasi, tiklananDelil))
                {
                    // YENİ MANTIK: İpi hemen çekme! Önce yazıyı iste.

                    // İpin uçlarını geçici hafızaya al
                    geciciBas = baslangicNoktasi;
                    geciciBitis = tiklananDelil;

                    // 1. Yazı giriş kutusunu (InputField) aktif hale getir
                    if (baglantiInputField != null)
                    {
                        baglantiInputField.gameObject.SetActive(true);
                        baglantiInputField.text = ""; // Eski yazıyı temizle
                        baglantiInputField.ActivateInputField(); // Odağı buraya al
                        baglantiInputField.onEndEdit.RemoveAllListeners(); // Eski olayları temizle
                        baglantiInputField.onEndEdit.AddListener(OnInputDone); // Yazı bitince OnInputDone çalışsın
                    }
                }
                else
                {
                    Debug.LogWarning("Bu iki delil zaten birbirine bağlı!");
                }
            }
            baslangicNoktasi = null;
        }
    }

    // YENİ: Oyuncu Enter'a bastığında çalışacak fonksiyon
    private void OnInputDone(string yazi)
    {
        // Eğer bir şey yazdıysa ve uçlar hafızadaysa ipi çek!
        if (!string.IsNullOrEmpty(yazi) && geciciBas != null && geciciBitis != null)
        {
            IpCek(geciciBas, geciciBitis, yazi);
        }
        else
        {
            Debug.LogWarning("Yazı yazılmadı veya uçlar kayboldu, ip çekilemedi.");
        }

        // InputField'ı gizle ve uçları temizle
        baglantiInputField.gameObject.SetActive(false);
        geciciBas = null;
        geciciBitis = null;
    }

    private bool IpZatenVarMi(RectTransform a, RectTransform b)
    {
        foreach (KirmiziIp ip in cekilenIpler)
        {
            if (ip == null) continue;
            if ((ip.Baslangic == a && ip.Bitis == b) || (ip.Baslangic == b && ip.Bitis == a))
            {
                return true;
            }
        }
        return false;
    }

    // YENİ: Artık bu fonksiyon bir de 'yazi' parametresi alıyor
    private void IpCek(RectTransform baslangic, RectTransform bitis, string yazi)
    {
        GameObject yeniIp = Instantiate(ipPrefab, ipKonteyneri);
        KirmiziIp ipScripti = yeniIp.GetComponent<KirmiziIp>();

        if (ipScripti != null)
        {
            ipScripti.UclariBagla(baslangic, bitis);
            // İpin üzerine yazıyı yaz!
            ipScripti.YaziyiSetEt(yazi);
            cekilenIpler.Add(ipScripti);
        }

        yeniIp.transform.SetAsFirstSibling();
    }
}