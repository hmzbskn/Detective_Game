using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class IpYoneticisi : MonoBehaviour
{
    public static IpYoneticisi Instance { get; private set; }

    [Header("İp Ayarları")]
    [SerializeField] private GameObject ipPrefab;
    [SerializeField] private Transform ipKonteyneri;

    [Header("UI Ayarları")]
    [SerializeField] private TMP_InputField baglantiInputField;

    private RectTransform baslangicNoktasi;
    private RectTransform geciciBas;
    private RectTransform geciciBitis;

    private readonly List<KirmiziIp> cekilenIpler = new List<KirmiziIp>();

    private void Awake()
    {
        Instance = this;

        if (baglantiInputField != null)
            baglantiInputField.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SecimiIptalEt();
        }
    }

    public void DelileTiklandi(RectTransform tiklananDelil)
    {
        if (tiklananDelil == null)
            return;

        if (baslangicNoktasi == null)
        {
            baslangicNoktasi = tiklananDelil;
            Debug.Log("İpin başlangıç noktası seçildi: " + tiklananDelil.gameObject.name);
            return;
        }

        if (baslangicNoktasi == tiklananDelil)
        {
            Debug.LogWarning("Bir delil kendisine bağlanamaz.");
            baslangicNoktasi = null;
            return;
        }

        if (IpZatenVarMi(baslangicNoktasi, tiklananDelil))
        {
            Debug.LogWarning("Bu iki delil zaten birbirine bağlı.");
            baslangicNoktasi = null;
            return;
        }

        geciciBas = baslangicNoktasi;
        geciciBitis = tiklananDelil;
        baslangicNoktasi = null;

        BaglantiYazisiIste();
    }

    private void BaglantiYazisiIste()
    {
        if (baglantiInputField == null)
        {
            IpCek(geciciBas, geciciBitis, "");
            geciciBas = null;
            geciciBitis = null;
            return;
        }

        baglantiInputField.gameObject.SetActive(true);
        baglantiInputField.text = "";
        baglantiInputField.ActivateInputField();

        baglantiInputField.onEndEdit.RemoveAllListeners();
        baglantiInputField.onEndEdit.AddListener(OnInputDone);
    }

    private void OnInputDone(string yazi)
    {
        if (geciciBas != null && geciciBitis != null)
        {
            IpCek(geciciBas, geciciBitis, yazi);
        }

        if (baglantiInputField != null)
        {
            baglantiInputField.onEndEdit.RemoveListener(OnInputDone);
            baglantiInputField.gameObject.SetActive(false);
        }

        geciciBas = null;
        geciciBitis = null;
    }

    private void IpCek(RectTransform baslangic, RectTransform bitis, string yazi)
    {
        if (ipPrefab == null)
        {
            Debug.LogError("IpYoneticisi üzerinde ipPrefab atanmamış.");
            return;
        }

        if (ipKonteyneri == null)
        {
            Debug.LogError("IpYoneticisi üzerinde ipKonteyneri atanmamış.");
            return;
        }

        GameObject yeniIp = Instantiate(ipPrefab, ipKonteyneri);

        KirmiziIp ipScripti = yeniIp.GetComponent<KirmiziIp>();

        if (ipScripti == null)
        {
            Debug.LogError("ipPrefab üzerinde KirmiziIp scripti yok.");
            Destroy(yeniIp);
            return;
        }

        ipScripti.UclariBagla(baslangic, bitis);
        ipScripti.YaziyiSetEt(yazi);

        cekilenIpler.Add(ipScripti);

        yeniIp.transform.SetAsFirstSibling();
    }

    private bool IpZatenVarMi(RectTransform a, RectTransform b)
    {
        foreach (KirmiziIp ip in cekilenIpler)
        {
            if (ip == null)
                continue;

            bool ayniYon = ip.Baslangic == a && ip.Bitis == b;
            bool tersYon = ip.Baslangic == b && ip.Bitis == a;

            if (ayniYon || tersYon)
                return true;
        }

        return false;
    }

    private void SecimiIptalEt()
    {
        baslangicNoktasi = null;
        geciciBas = null;
        geciciBitis = null;

        if (baglantiInputField != null && baglantiInputField.gameObject.activeSelf)
        {
            baglantiInputField.onEndEdit.RemoveListener(OnInputDone);
            baglantiInputField.gameObject.SetActive(false);
        }
    }
}