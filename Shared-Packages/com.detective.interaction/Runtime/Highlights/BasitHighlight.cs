using UnityEngine;

public class BasitHighlight : MonoBehaviour, IHighlightable
{
    [SerializeField] private Renderer hedefRenderer;
    [SerializeField] private Color highlightRengi = Color.yellow;

    private Material materyal;
    private Color orijinalEmission;
    private bool hazirMi = false;

    private void Awake()
    {
        if (hedefRenderer == null)
            hedefRenderer = GetComponentInChildren<Renderer>();

        if (hedefRenderer != null)
        {
            materyal = hedefRenderer.material;
            materyal.EnableKeyword("_EMISSION");
            orijinalEmission = materyal.GetColor("_EmissionColor");
            hazirMi = true;
        }
    }

    public void HighlightAc()
    {
        if (!hazirMi) return;
        materyal.SetColor("_EmissionColor", highlightRengi);
    }

    public void HighlightKapat()
    {
        if (!hazirMi) return;
        materyal.SetColor("_EmissionColor", orijinalEmission);
    }
}