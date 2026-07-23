using UnityEngine;

public class FotografBaskiObjesi : MonoBehaviour
{
    [Header("Fotoğrafın Basılacağı Renderer")]
    [SerializeField] private Renderer onYuzRenderer;

    [Header("Material Ayarı")]
    [SerializeField] private int materialIndex = 0;

    [Header("Debug")]
    [SerializeField] private bool debugMesajlari = true;

    public FotografKaydi Kayit { get; private set; }

    private void Awake()
    {
        if (onYuzRenderer == null)
        {
            Transform photoSurface = transform.Find("PhotoSurface");

            if (photoSurface != null)
                onYuzRenderer = photoSurface.GetComponent<Renderer>();

            if (onYuzRenderer == null)
                onYuzRenderer = GetComponentInChildren<Renderer>();
        }
    }

    public void FotografAta(FotografKaydi yeniKayit)
    {
        Kayit = yeniKayit;

        if (Kayit == null)
        {
            if (debugMesajlari)
                Debug.LogWarning("FotografBaskiObjesi: Kayit boş geldi.");

            return;
        }

        if (Kayit.FotografTexture == null)
        {
            if (debugMesajlari)
                Debug.LogWarning("FotografBaskiObjesi: FotografTexture boş geldi.");

            return;
        }

        GorseliUygula();
    }

    private void GorseliUygula()
    {
        if (onYuzRenderer == null)
        {
            Debug.LogWarning("FotografBaskiObjesi: On Yuz Renderer atanmamış.");
            return;
        }

        Material hedefMateryal = MateryalGetirVeyaOlustur();

        if (hedefMateryal == null)
        {
            Debug.LogWarning("FotografBaskiObjesi: Material oluşturulamadı.");
            return;
        }

        Texture2D texture = Kayit.FotografTexture;

        if (hedefMateryal.HasProperty("_BaseMap"))
            hedefMateryal.SetTexture("_BaseMap", texture);

        if (hedefMateryal.HasProperty("_MainTex"))
            hedefMateryal.SetTexture("_MainTex", texture);

        hedefMateryal.mainTexture = texture;

        onYuzRenderer.enabled = true;

        if (debugMesajlari)
        {
            Debug.Log(
                "Fotoğraf yüzeye basıldı. Renderer: " +
                onYuzRenderer.name +
                " | Texture: " +
                texture.name +
                " | Boyut: " +
                texture.width +
                "x" +
                texture.height
            );
        }
    }

    private Material MateryalGetirVeyaOlustur()
    {
        Material[] materyaller = onYuzRenderer.materials;

        if (materyaller == null || materyaller.Length == 0)
        {
            Material yeniMateryal = YeniUnlitMateryalOlustur();
            onYuzRenderer.material = yeniMateryal;
            return yeniMateryal;
        }

        if (materialIndex < 0 || materialIndex >= materyaller.Length)
            materialIndex = 0;

        Material hedefMateryal = materyaller[materialIndex];

        if (hedefMateryal == null)
        {
            hedefMateryal = YeniUnlitMateryalOlustur();
            materyaller[materialIndex] = hedefMateryal;
            onYuzRenderer.materials = materyaller;
        }

        return hedefMateryal;
    }

    private Material YeniUnlitMateryalOlustur()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");

        if (shader == null)
            shader = Shader.Find("Unlit/Texture");

        if (shader == null)
            shader = Shader.Find("Standard");

        if (shader == null)
        {
            Debug.LogWarning("FotografBaskiObjesi: Uygun shader bulunamadı.");
            return null;
        }

        Material mat = new Material(shader);
        mat.name = "M_Runtime_BasiliFotograf";

        return mat;
    }
}