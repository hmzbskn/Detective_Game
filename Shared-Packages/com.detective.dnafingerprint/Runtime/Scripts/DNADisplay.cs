using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DNADisplay : MonoBehaviour
{
    [Header("Slotlar")]
    [SerializeField] private Image[] slotImages;

    [Header("Başlık")]
    [SerializeField] private TextMeshProUGUI baslikText;

    [Header("Başlık Ayarı")]
    [SerializeField] private bool sadeceSahipAdiniGoster = true;

    public void DNAGoster(DNAData dna)
    {
        if (dna == null)
        {
            Debug.LogWarning("DNA verisi boş!");
            Temizle();
            return;
        }

        if (baslikText != null)
        {
            if (sadeceSahipAdiniGoster)
                baslikText.text = dna.sahibiAdi;
            else
                baslikText.text = $"{dna.dnaID} - {dna.sahibiAdi}";
        }

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] == null)
                continue;

            int level = 0;

            if (dna.spektrum != null && i < dna.spektrum.Length)
                level = Mathf.Clamp(dna.spektrum[i], 0, 4);

            int alpha255 = Mathf.Clamp(level * 64, 0, 255);
            float alpha = alpha255 / 255f;

            Color c = slotImages[i].color;
            c.r = 0f;
            c.g = 0f;
            c.b = 0f;
            c.a = alpha;

            slotImages[i].color = c;
        }
    }

    public void Temizle()
    {
        if (baslikText != null)
            baslikText.text = "";

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] == null)
                continue;

            Color c = slotImages[i].color;
            c.a = 0f;
            slotImages[i].color = c;
        }
    }
}