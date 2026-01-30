using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

[System.Serializable]
public class Talky
{
    public string[] lines;
    public bool[] autoProgressLines;
    public bool[] endlines;
    public string name;
    [TextArea(3, 10)]
    public string[] sentences;
    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public int choiceindex;
    public string[] choices;
    public int[] nextDialogueIndex;


}