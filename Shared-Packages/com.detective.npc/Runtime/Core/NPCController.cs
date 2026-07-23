using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Veri")]
    [SerializeField] private NPCData npcData;

    [Header("Durum")]
    [SerializeField] private NPCState currentState = NPCState.Idle;

    private readonly HashSet<string> sorulanSorular = new HashSet<string>();

    // 🔥 EKLEDİK
    private NPCPatrol patrol;

    public NPCData Data => npcData;
    public NPCState CurrentState => currentState;

    public bool KonusabilirMi =>
        npcData != null &&
        currentState != NPCState.Disabled &&
        currentState != NPCState.Talking;

    private void Awake()
    {
        // 🔥 patrol referansı al
        patrol = GetComponent<NPCPatrol>();
    }

    public void KonusmayiBaslat()
    {
        if (!KonusabilirMi)
            return;

        if (npcData == null)
        {
            Debug.LogWarning($"{gameObject.name} için NPCData atanmamış.");
            return;
        }

        if (npcData.baslangicNode == null)
        {
            Debug.LogWarning($"{npcData.npcAdi} için başlangıç node atanmadı.");
            return;
        }

        if (NPCDialogueUI.Instance == null)
        {
            Debug.LogWarning("Sahnede NPCDialogueUI bulunamadı.");
            return;
        }

        // 🔥 STATE
        currentState = NPCState.Talking;

        // 🔥 NPC DURUR
        if (patrol != null)
            patrol.Dur();

        NPCDialogueUI.Instance.DiyalogBaslat(this, npcData.baslangicNode);
    }

    public void KonusmayiBitir()
    {
        if (currentState == NPCState.Disabled)
            return;

        currentState = NPCState.Idle;

        // 🔥 NPC DEVAM EDER
        if (patrol != null)
            patrol.Devam();
    }

    public void NPCPasifYap()
    {
        currentState = NPCState.Disabled;

        // 🔥 tamamen dursun
        if (patrol != null)
            patrol.Dur();
    }

    public bool SoruSorulduMu(string soruMetni)
    {
        return sorulanSorular.Contains(soruMetni);
    }

    public void SoruKaydet(string soruMetni)
    {
        if (string.IsNullOrWhiteSpace(soruMetni))
            return;

        sorulanSorular.Add(soruMetni);
    }
}