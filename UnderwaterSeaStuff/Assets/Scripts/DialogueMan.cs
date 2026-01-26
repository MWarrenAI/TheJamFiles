using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMan : MonoBehaviour
{
    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartTalky (Talky talky)
    {
        Debug.Log("Starting Conversation " + talky.name);
    }
}
