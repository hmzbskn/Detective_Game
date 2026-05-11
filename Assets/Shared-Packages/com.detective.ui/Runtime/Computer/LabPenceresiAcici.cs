using UnityEngine;

public class LabPenceresiAcici : MonoBehaviour
{
    [Header("Pencere")]
    [SerializeField] private BilgisayarPenceresi labPenceresi;

    [Header("LAB Icerigi")]
    [SerializeField] private RectTransform uygulamaHavuzu;
    [SerializeField] private RectTransform dnaSystem;

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
    }
}