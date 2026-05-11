using UnityEngine;

[CreateAssetMenu(fileName = "YeniDialogueNode", menuName = "Dedektif Oyunu/NPC/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    [TextArea(2, 5)]
    public string npcText;

    public DialogueChoice[] secenekler;
}