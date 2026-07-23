using System.Collections.Generic;
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

    private readonly DiyalogAkisi akis = new DiyalogAkisi();

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

        akis.Baslat(npc, baslangicNode);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (npcAdiText != null)
            npcAdiText.text = npc.Data != null ? npc.Data.npcAdi : string.Empty;

        NodeGoster();
        OyunKontrolleriniKilitle(true);
    }

    private void NodeGoster()
    {
        if (akis.AktifNode == null)
            return;

        if (dialogueText != null)
            dialogueText.text = akis.AktifNode.npcText;

        if (akis.AktifNode.secenekler == null || akis.AktifNode.secenekler.Length == 0)
        {
            if (choicePanel != null)
                choicePanel.SetActive(false);

            return;
        }

        SecenekleriGoster();
    }

    private void SecenekleriGoster()
    {
        List<DialogueChoice> uygunSecenekler =
            akis.UygunSecenekleriGetir(item => hotbarSistemi != null && hotbarSistemi.ItemVarMi(item));

        if (choicePanel != null)
            choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] == null)
                continue;

            if (i >= uygunSecenekler.Count)
            {
                choiceButtons[i].gameObject.SetActive(false);
                continue;
            }

            DialogueChoice secim = uygunSecenekler[i];

            choiceButtons[i].gameObject.SetActive(true);

            TextMeshProUGUI butonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (butonText != null)
                butonText.text = secim.secenekMetni;

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => SecimYap(secim));
        }
    }

    private void SecimYap(DialogueChoice secim)
    {
        bool bittiMi = akis.SecimYap(secim);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (bittiMi)
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

        if (akis.AktifNPC != null)
            akis.AktifNPC.KonusmayiBitir();

        akis.Bitir();

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
