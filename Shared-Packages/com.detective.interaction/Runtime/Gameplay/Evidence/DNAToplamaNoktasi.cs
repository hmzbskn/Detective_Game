using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DNAToplamaNoktasi : MonoBehaviour, IHighlightable
{
    [Header("DNA Noktası")]
    [SerializeField] private string noktaAdi = "DNA Noktası";

    [Header("Toplama Sonucu Oluşacak Delil")]
    [SerializeField] private ItemData dnaDeliliItemData;

    [Header("Laboratuvarda Kullanılacak DNA Verisi")]
    [SerializeField] private DNAData dnaData;

    [Header("Toplama Ayarları")]
    [SerializeField] private bool tekSeferlik = true;
    [SerializeField] private bool toplandiktanSonraGizle = true;

    [Header("Highlight Ayarları")]
    [SerializeField] private Color highlightRengi = Color.yellow;

    private bool toplandiMi = false;
    private bool highlightAcikMi = false;

    private Collider hedefCollider;
    private Renderer[] rendererlar;
    private Color[] orijinalRenkler;

    public string NoktaAdi => noktaAdi;
    public ItemData DNADeliliItemData => dnaDeliliItemData;
    public DNAData DNAData => dnaData;
    public bool ToplandiMi => toplandiMi;

    private void Awake()
    {
        hedefCollider = GetComponent<Collider>();

        if (hedefCollider != null)
            hedefCollider.isTrigger = true;

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

        if (dnaData == null)
            return false;

        return true;
    }

    public void ToplandiOlarakIsaretle()
    {
        if (!ToplanabilirMi())
            return;

        toplandiMi = true;

        HighlightKapat();

        if (hedefCollider != null)
            hedefCollider.enabled = false;

        if (toplandiktanSonraGizle)
        {
            for (int i = 0; i < rendererlar.Length; i++)
            {
                if (rendererlar[i] != null)
                    rendererlar[i].enabled = false;
            }
        }

        Debug.Log($"{noktaAdi} noktasından DNA alındı. Delil: {dnaDeliliItemData.ItemAdi} | DNA: {dnaData.dnaID}");
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