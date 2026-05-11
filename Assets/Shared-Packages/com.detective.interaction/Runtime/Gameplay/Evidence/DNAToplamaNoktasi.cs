using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DNAToplamaNoktasi : MonoBehaviour, IHighlightable
{
    [Header("DNA Noktası")]
    [SerializeField] private string noktaAdi = "DNA Noktası";

    [Header("Toplama Sonucu Oluşacak Delil")]
    [SerializeField] private ItemData dnaDeliliItemData;

    [Header("Toplama Ayarları")]
    [SerializeField] private bool tekSeferlik = true;
    [SerializeField] private bool toplandiktanSonraKapat = true;

    [Header("Highlight Ayarları")]
    [SerializeField] private Color highlightRengi = Color.yellow;

    private bool toplandiMi = false;
    private bool highlightAcikMi = false;

    private Renderer[] rendererlar;
    private Color[] orijinalRenkler;

    public string NoktaAdi => noktaAdi;
    public ItemData DNADeliliItemData => dnaDeliliItemData;
    public bool ToplandiMi => toplandiMi;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        rendererlar = GetComponentsInChildren<Renderer>(true);
        orijinalRenkler = new Color[rendererlar.Length];

        for (int i = 0; i < rendererlar.Length; i++)
        {
            if (rendererlar[i] != null && rendererlar[i].material.HasProperty("_Color"))
            {
                orijinalRenkler[i] = rendererlar[i].material.color;
            }
        }
    }

    public bool ToplanabilirMi()
    {
        if (tekSeferlik && toplandiMi)
            return false;

        if (dnaDeliliItemData == null)
            return false;

        return true;
    }

    public void ToplandiOlarakIsaretle()
    {
        if (!ToplanabilirMi())
            return;

        toplandiMi = true;

        HighlightKapat();

        Debug.Log($"{noktaAdi} noktasından DNA alındı. Oluşan delil: {dnaDeliliItemData.ItemAdi}");

        if (toplandiktanSonraKapat)
            gameObject.SetActive(false);
    }

    public void HighlightAc()
    {
        if (highlightAcikMi)
            return;

        if (!ToplanabilirMi())
            return;

        highlightAcikMi = true;

        for (int i = 0; i < rendererlar.Length; i++)
        {
            if (rendererlar[i] != null && rendererlar[i].material.HasProperty("_Color"))
            {
                rendererlar[i].material.color = highlightRengi;
            }
        }
    }

    public void HighlightKapat()
    {
        if (!highlightAcikMi)
            return;

        highlightAcikMi = false;

        for (int i = 0; i < rendererlar.Length; i++)
        {
            if (rendererlar[i] != null && rendererlar[i].material.HasProperty("_Color"))
            {
                rendererlar[i].material.color = orijinalRenkler[i];
            }
        }
    }
}