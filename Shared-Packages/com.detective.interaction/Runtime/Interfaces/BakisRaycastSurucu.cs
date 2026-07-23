using UnityEngine;

/// <summary>
/// "Kameranın baktığı yöne ışın gönder, mesafe/katman/tetikleyici ayarlarına göre en yakın çarpışmayı
/// bul" işini tek yerde toplar. Önceden OyuncuEtkilesim ve OyuncuDNAToplamaKontrolcusu bu raycast
/// mantığını neredeyse birebir kopyalayarak kendi Update() metotlarında ayrı ayrı uyguluyordu.
/// </summary>
public class BakisRaycastSurucu
{
    private readonly float mesafe;
    private readonly LayerMask katman;
    private readonly QueryTriggerInteraction triggerModu;

    public BakisRaycastSurucu(float mesafe, LayerMask katman, QueryTriggerInteraction triggerModu = QueryTriggerInteraction.Ignore)
    {
        this.mesafe = mesafe;
        this.katman = katman;
        this.triggerModu = triggerModu;
    }

    public float Mesafe => mesafe;

    public bool Raycast(Camera kamera, out RaycastHit hit)
    {
        if (kamera == null)
        {
            hit = default;
            return false;
        }

        Ray ray = new Ray(kamera.transform.position, kamera.transform.forward);
        return Physics.Raycast(ray, out hit, mesafe, katman, triggerModu);
    }
}
