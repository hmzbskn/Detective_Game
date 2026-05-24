using UnityEngine;

[CreateAssetMenu(fileName = "YeniNPCData", menuName = "Dedektif Oyunu/NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    [Header("Kimlik")]
    public string npcId;
    public string npcAdi;

    [TextArea(2, 5)]
    public string kisaAciklama;

    [Header("Görsel")]
    public Sprite portrait;

    [Header("Başlangıç Diyaloğu")]
    public DialogueNode baslangicNode;
}