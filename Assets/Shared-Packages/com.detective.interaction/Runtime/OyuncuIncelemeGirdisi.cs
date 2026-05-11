using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OyuncuIncelemeGirdisi : MonoBehaviour
{
    [SerializeField] private HotbarSistemi hotbarSistemi;
    [SerializeField] private OyuncuEsyaTutucu oyuncuEsyaTutucu;
    [SerializeField] private IncelemeSistemi incelemeSistemi;

    private void Update()
    {
        if (incelemeSistemi == null || oyuncuEsyaTutucu == null)
            return;

        // UI açıkken blokla
        if (hotbarSistemi != null && hotbarSistemi.EnvanterAcikMi())
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // İnceleme açıkken blokla
        if (incelemeSistemi.IncelemeAktifMi)
            return;

        // El boşsa inceleme yok
        if (oyuncuEsyaTutucu.EliBosMu())
            return;

        // 🔥 F TUŞU
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            GameObject eldekiObje = oyuncuEsyaTutucu.AktifEldeTutulanObjeGetir();

            if (eldekiObje == null)
                return;

            var incelenebilir = eldekiObje.GetComponent<IIncelenebilir>();

            if (incelenebilir == null)
                return;

            if (!incelenebilir.IncelenebilirMi())
                return;

            incelemeSistemi.IncelemeyiBaslat(eldekiObje);
        }
    }
}