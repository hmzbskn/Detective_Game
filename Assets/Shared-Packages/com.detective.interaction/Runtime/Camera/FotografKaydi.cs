using System;
using UnityEngine;

[Serializable]
public class FotografKaydi
{
    [SerializeField] private string fotografId;
    [SerializeField] private Texture2D fotografTexture;
    [SerializeField] private Sprite fotografSprite;
    [SerializeField] private string cekimZamani;
    [SerializeField] private Vector3 cekimPozisyonu;
    [SerializeField] private Quaternion cekimRotasyonu;

    public string FotografId => fotografId;
    public Texture2D FotografTexture => fotografTexture;
    public Sprite FotografSprite => fotografSprite;
    public string CekimZamani => cekimZamani;
    public Vector3 CekimPozisyonu => cekimPozisyonu;
    public Quaternion CekimRotasyonu => cekimRotasyonu;

    public FotografKaydi(Texture2D texture, Vector3 pozisyon, Quaternion rotasyon)
    {
        fotografId = Guid.NewGuid().ToString();
        fotografTexture = texture;
        cekimPozisyonu = pozisyon;
        cekimRotasyonu = rotasyon;
        cekimZamani = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

        fotografSprite = SpriteOlustur(texture);
    }

    private Sprite SpriteOlustur(Texture2D texture)
    {
        if (texture == null)
            return null;

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }
}