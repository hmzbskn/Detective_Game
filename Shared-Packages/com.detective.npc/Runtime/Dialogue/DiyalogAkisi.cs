using System;
using System.Collections.Generic;

/// <summary>
/// Diyalog ağacındaki düğüm ilerletme ve delile-bağlı dallanma kurallarını NPCDialogueUI'den ayırır.
/// Bir düğümdeki hangi seçeneklerin oyuncuya gösterilebilir olduğuna (daha önce sorulmuş mu, gerekli
/// delil envanterde var mı) burada karar verilir; NPCDialogueUI sadece bu listeyi render eder.
/// </summary>
public class DiyalogAkisi
{
    public NPCController AktifNPC { get; private set; }
    public DialogueNode AktifNode { get; private set; }

    public void Baslat(NPCController npc, DialogueNode baslangicNode)
    {
        AktifNPC = npc;
        AktifNode = baslangicNode;
    }

    public List<DialogueChoice> UygunSecenekleriGetir(Func<ItemData, bool> itemVarMi)
    {
        List<DialogueChoice> sonuc = new List<DialogueChoice>();

        if (AktifNode == null || AktifNode.secenekler == null)
            return sonuc;

        for (int i = 0; i < AktifNode.secenekler.Length; i++)
        {
            DialogueChoice secim = AktifNode.secenekler[i];

            if (secim == null)
                continue;

            if (secim.sadeceBirKezSorulsun && secim.secildiMi)
                continue;

            if (secim.gerekliDelil != null)
            {
                if (itemVarMi == null || !itemVarMi(secim.gerekliDelil))
                    continue;
            }

            sonuc.Add(secim);
        }

        return sonuc;
    }

    /// <summary>
    /// Bir seçim yapıldığında düğümü ilerletir. Diyalog bittiyse (yeni düğüm yoksa) true döner.
    /// </summary>
    public bool SecimYap(DialogueChoice secim)
    {
        if (secim == null)
            return true;

        secim.secildiMi = true;

        if (secim.sadeceBirKezSorulsun && AktifNPC != null)
            AktifNPC.SoruKaydet(secim.secenekMetni);

        if (secim.gerekliDelil != null)
            AktifNode = secim.dogruDelilMi ? secim.nextNode : secim.yanlisDelilNode;
        else
            AktifNode = secim.nextNode;

        return AktifNode == null;
    }

    public void Bitir()
    {
        AktifNPC = null;
        AktifNode = null;
    }
}
