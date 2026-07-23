using UnityEngine;

public class NPCInteraction : EtkilesebilirObje
{
    [SerializeField] private NPCController npcController;

    private void Reset()
    {
        if (npcController == null)
            npcController = GetComponentInParent<NPCController>();
    }

    private void Awake()
    {
        if (npcController == null)
            npcController = GetComponentInParent<NPCController>();
    }

    public override void Etkilesim()
    {
        if (npcController == null)
        {
            Debug.LogWarning($"{gameObject.name} üzerinde NPCController bulunamadı.");
            return;
        }

        npcController.KonusmayiBaslat();
    }

    public override string EtkilesimMetniGetir()
    {
        if (npcController == null)
            return "Etkileşim";

        if (!npcController.KonusabilirMi)
            return string.Empty;

        return "Konuş";
    }
}