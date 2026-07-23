using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class YerlestirmeGirdiDenetleyici
{
    [Header("Döndürme Ayarları")]
    [SerializeField] private float scrollBasinaDonusAcisi = 15f;

    [Header("Yükseklik Ayarları")]
    [SerializeField] private float scrollBasinaYukseklikAdimi = 0.05f;
    [SerializeField] private float minimumEkYukseklik = 0f;
    [SerializeField] private float maksimumEkYukseklik = 2f;

    public float BirikmisYRotasyonu { get; private set; }
    public float EkYukseklik { get; private set; }

    public void GirdiyiIsle()
    {
        if (Mouse.current == null)
            return;

        float scrollDegeri = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scrollDegeri) < 0.01f)
            return;

        float yon = Mathf.Sign(scrollDegeri);

        bool shiftBasili =
            (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed) ||
            (Keyboard.current != null && Keyboard.current.rightShiftKey.isPressed);

        if (shiftBasili)
        {
            EkYukseklik += yon * scrollBasinaYukseklikAdimi;
            EkYukseklik = Mathf.Clamp(EkYukseklik, minimumEkYukseklik, maksimumEkYukseklik);
        }
        else
        {
            BirikmisYRotasyonu += yon * scrollBasinaDonusAcisi;
        }
    }

    public void Sifirla()
    {
        BirikmisYRotasyonu = 0f;
        EkYukseklik = 0f;
    }
}