using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    [Header("NPC Info")]
    public string npcName;
    public Sprite portrait;

    [Header("Dialogue Content")]
    [TextArea(3, 10)]
    public string[] dialogueLines;
    public DialogueChoice[] choices;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public bool[] autoProgressLines;
    public float autoProgressDelay = 1.5f;
}

public class DialogueChoice
{
    public int dialogueIndex; 
    public string[] choiceLabels;
    public int[] nextDialogueIndexes;
}