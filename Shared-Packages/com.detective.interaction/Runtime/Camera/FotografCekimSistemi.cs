using UnityEngine;

public class FotografCekimSistemi : MonoBehaviour
{
    [Header("Kamera")]
    [SerializeField] private Camera kaynakKamera;

    [Header("Fotoğraf Çözünürlüğü")]
    [SerializeField] private int genislik = 1280;
    [SerializeField] private int yukseklik = 720;

    [Header("Debug")]
    [SerializeField] private bool debugMesajlari = true;

    private void Awake()
    {
        if (kaynakKamera == null)
            kaynakKamera = Camera.main;
    }

    public FotografKaydi FotografCek()
    {
        if (kaynakKamera == null)
        {
            Debug.LogWarning("Fotoğraf çekilemedi. Kaynak kamera atanmamış.");
            return null;
        }

        Texture2D texture = KameraGoruntusunuTextureAl();

        if (texture == null)
        {
            Debug.LogWarning("Fotoğraf çekilemedi. Texture oluşturulamadı.");
            return null;
        }

        FotografKaydi kayit = new FotografKaydi(
            texture,
            kaynakKamera.transform.position,
            kaynakKamera.transform.rotation
        );

        if (debugMesajlari)
            Debug.Log("Fotoğraf çekildi: " + kayit.FotografId);

        return kayit;
    }

    private Texture2D KameraGoruntusunuTextureAl()
    {
        RenderTexture oncekiTargetTexture = kaynakKamera.targetTexture;
        RenderTexture oncekiAktifTexture = RenderTexture.active;

        RenderTexture renderTexture = RenderTexture.GetTemporary(
            genislik,
            yukseklik,
            24,
            RenderTextureFormat.ARGB32
        );

        Texture2D texture = new Texture2D(
            genislik,
            yukseklik,
            TextureFormat.RGB24,
            false
        );

        kaynakKamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;

        kaynakKamera.Render();

        texture.ReadPixels(new Rect(0, 0, genislik, yukseklik), 0, 0);
        texture.Apply();

        kaynakKamera.targetTexture = oncekiTargetTexture;
        RenderTexture.active = oncekiAktifTexture;

        RenderTexture.ReleaseTemporary(renderTexture);

        texture.name = "Cekilen_Fotograf_" + System.DateTime.Now.ToString("HHmmss");

        return texture;
    }
}