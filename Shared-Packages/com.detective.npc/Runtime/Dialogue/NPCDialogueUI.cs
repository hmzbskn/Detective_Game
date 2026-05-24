using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogueUI : MonoBehaviour
{
    public static NPCDialogueUI Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] private GameObject dialoguePanel;

    [Header("UI Referansları")]
    [SerializeField] private TextMeshProUGUI npcAdiText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Seçenekler")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;

    [Header("Kontrol Kilidi")]
    [SerializeField] private NPCKonusmaKilidi npcKonusmaKilidi;

    [Header("Envanter Kontrolü")]
    [SerializeField] private HotbarSistemi hotbarSistemi;

    private NPCController aktifNPC;
    private DialogueNode aktifNode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);
    }

    public void DiyalogBaslat(NPCController npc, DialogueNode baslangicNode)
    {
        if (npc == null || baslangicNode == null)
            return;

        aktifNPC = npc;
        aktifNode = baslangicNode;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (npcAdiText != null)
            npcAdiText.text = npc.Data != null ? npc.Data.npcAdi : string.Empty;

        NodeGoster();
        OyunKontrolleriniKilitle(true);
    }

    private void NodeGoster()
    {
        if (aktifNode == null)
            return;

        if (dialogueText != null)
            dialogueText.text = aktifNode.npcText;

        if (aktifNode.secenekler == null || aktifNode.secenekler.Length == 0)
        {
            if (choicePanel != null)
                choicePanel.SetActive(false);

            return;
        }

        SecenekleriGoster();
    }

    private void SecenekleriGoster()
    {
        if (choicePanel != null)
            choicePanel.SetActive(true);

        int aktifButonIndex = 0;

        for (int i = 0; i < aktifNode.secenekler.Length; i++)
        {
            DialogueChoice secim = aktifNode.secenekler[i];

            if (secim == null)
                continue;

            if (secim.sadeceBirKezSorulsun && secim.secildiMi)
                continue;

            if (secim.gerekliDelil != null)
            {
                if (hotbarSistemi == null || !hotbarSistemi.ItemVarMi(secim.gerekliDelil))
                    continue;
            }

            if (aktifButonIndex >= choiceButtons.Length)
                break;

            Button buton = choiceButtons[aktifButonIndex];

            if (buton == null)
            {
                aktifButonIndex++;
                continue;
            }

            buton.gameObject.SetActive(true);

            TextMeshProUGUI butonText = buton.GetComponentInChildren<TextMeshProUGUI>();
            if (butonText != null)
                butonText.text = secim.secenekMetni;

            int index = i;
            buton.onClick.RemoveAllListeners();
            buton.onClick.AddListener(() => SecimYap(index));

            aktifButonIndex++;
        }

        for (int i = aktifButonIndex; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].gameObject.SetActive(false);
        }
    }

    private void SecimYap(int index)
    {
        if (aktifNode == null || aktifNode.secenekler == null || index < 0 || index >= aktifNode.secenekler.Length)
            return;

        DialogueChoice secim = aktifNode.secenekler[index];
        if (secim == null)
            return;

        secim.secildiMi = true;

        if (secim.sadeceBirKezSorulsun && aktifNPC != null)
            aktifNPC.SoruKaydet(secim.secenekMetni);

        // 🔥 BURASI YENİ
        if (secim.gerekliDelil != null)
        {
            if (secim.dogruDelilMi)
                aktifNode = secim.nextNode;
            else
                aktifNode = secim.yanlisDelilNode;
        }
        else
        {
            aktifNode = secim.nextNode;
        }

        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (aktifNode == null)
        {
            DiyalogBitir();
            return;
        }

        NodeGoster();
    }

    public void DiyalogBitir()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (npcAdiText != null)
            npcAdiText.text = string.Empty;

        if (dialogueText != null)
            dialogueText.text = string.Empty;

        if (aktifNPC != null)
            aktifNPC.KonusmayiBitir();

        aktifNPC = null;
        aktifNode = null;

        OyunKontrolleriniKilitle(false);
    }

    private void OyunKontrolleriniKilitle(bool kilitliMi)
    {
        if (npcKonusmaKilidi == null)
            return;

        if (kilitliMi)
            npcKonusmaKilidi.KonusmaKilidiniAc();
        else
            npcKonusmaKilidi.KonusmaKilidiniKapat();
    }
}