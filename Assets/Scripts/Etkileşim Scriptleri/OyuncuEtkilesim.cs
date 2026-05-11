using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class OyuncuEtkilesim : MonoBehaviour
{
    [Header("Etkileşim Ayarları")]
    public Transform kameraTransform;
    public float etkilesimMesafesi = 5f;

    [Header("UI Ayarları")]
    [Tooltip("Arkaplanı ve yazıyı barındıran ana UI objesi")]
    public GameObject etkilesimPaneli;
    [Tooltip("Yazıyı değiştireceğimiz Text objesi")]
    public TextMeshProUGUI etkilesimYazisi;

    void Update()
    {
        // 1. Her karenin başında paneli gizle
        etkilesimPaneli.SetActive(false);

        Ray isin = new Ray(kameraTransform.position, kameraTransform.forward);
        RaycastHit[] vuruslar = Physics.RaycastAll(isin, etkilesimMesafesi);

        foreach (RaycastHit vurus in vuruslar)
        {
            IEtkilesebilir hedef = vurus.collider.GetComponentInParent<IEtkilesebilir>();

            if (hedef != null)
            {
                // 2. Hedefi bulduk! Paneli görünür yap ve yazıyı bas.
                etkilesimPaneli.SetActive(true);
                etkilesimYazisi.text = "E : " + hedef.EtkilesimMetniGetir();

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    hedef.Etkilesim();
                }

                break;
            }
        }
    }
}
