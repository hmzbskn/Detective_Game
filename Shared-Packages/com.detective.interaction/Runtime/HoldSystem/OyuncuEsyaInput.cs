using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OyuncuEsyaInput : MonoBehaviour
{
    [SerializeField] private HotbarSistemi hotbarSistemi;
    [SerializeField] private OyuncuEsyaTutucu facade;
    [SerializeField] private IncelemeSistemi incelemeSistemi;
    private void Update()
    {
        // 1. UI Bloklamaları
        if (hotbarSistemi != null && hotbarSistemi.EnvanterAcikMi()) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        if (incelemeSistemi != null && incelemeSistemi.IncelemeAktifMi) return;
        // 2. Sistemi her koşulda güncelle (Preview takılmalarını önler)
        facade.SistemiGuncelle();

        // 3. Elimiz boşsa aksiyonu kes
        if (facade.EliBosMu()) return;

        // 4. Aksiyon (Tıklama ile Yerleştirme)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            facade.YerlestirmeyiDene();
        }
    }
}