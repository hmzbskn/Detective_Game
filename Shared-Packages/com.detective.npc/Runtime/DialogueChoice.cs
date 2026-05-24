using System;

[Serializable]
public class DialogueChoice
{
    public string secenekMetni;
    public DialogueNode nextNode;

    public bool sadeceBirKezSorulsun;
    public bool secildiMi;

    public ItemData gerekliDelil;

    public bool dogruDelilMi;
    public DialogueNode yanlisDelilNode;
}