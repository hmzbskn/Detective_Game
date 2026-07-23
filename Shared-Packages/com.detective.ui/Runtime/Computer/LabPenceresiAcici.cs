using System;
using UnityEngine;

public class LabPenceresiAcici : MonoBehaviour
{
    [Header("Pencere")]
    [SerializeField] private BilgisayarPenceresi labPenceresi;

    [Header("LAB Icerigi")]
    [SerializeField] private RectTransform uygulamaHavuzu;
    [SerializeField] private RectTransform dnaSystem;

    /// <summary>
    /// Lab penceresi her açıldığında tetiklenir. Detective.UI paketi Assembly-CSharp'taki Bridge
    /// sınıflarını (BilgisayarDNASorgulamaKoprusu gibi) derleme zamanında göremediği için, ekranı
    /// sıfırlama sorumluluğu artık SendMessage/tip-adı taraması yerine bu event üzerinden Bridge'e
    /// (zaten her iki tarafı da gören Assembly-CSharp katmanına) bırakılıyor.
    /// </summary>
    public event Action LabAcildi;

    public void LabAc()
    {
        if (labPenceresi != null)
            labPenceresi.Ac();

        if (dnaSystem == null || uygulamaHavuzu == null)
            return;

        dnaSystem.SetParent(uygulamaHavuzu, false);

        dnaSystem.anchorMin = Vector2.zero;
        dnaSystem.anchorMax = Vector2.one;
        dnaSystem.pivot = new Vector2(0.5f, 0.5f);

        dnaSystem.offsetMin = Vector2.zero;
        dnaSystem.offsetMax = Vector2.zero;
        dnaSystem.localScale = Vector3.one;

        dnaSystem.gameObject.SetActive(true);

        LabAcildi?.Invoke();
    }
}
