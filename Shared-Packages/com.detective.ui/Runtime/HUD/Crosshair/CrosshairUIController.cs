using UnityEngine;
using UnityEngine.UI;

public class CrosshairUIController : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Color normalRenk = Color.white;
    [SerializeField] private Color etkilesimRengi = Color.yellow;

    private void Awake()
    {
        NormalModaDon();
    }

    public void NormalModaDon()
    {
        if (crosshairImage != null)
            crosshairImage.color = normalRenk;
    }

    public void EtkilesimModunaGec()
    {
        if (crosshairImage != null)
            crosshairImage.color = etkilesimRengi;
    }
}