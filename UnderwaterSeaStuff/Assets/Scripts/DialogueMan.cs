using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueMan : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private Queue<string> sentences;

    public static DialogueMan Instance;

    void Awake()
    {
        Instance = this;
        sentences = new Queue<string>();
    }

    public void StartTalky(Talky talky)
    {
        if (talky == null)
        {
            Debug.LogWarning("Talky asset is missing!");
            return;
        }


        Debug.Log("Starting Conversation " + talky.name);
        nameText.text = talky.name;
        sentences.Clear();

        foreach (string sentence in talky.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNext();
    }

    public void DisplayNext()
    {
        if (sentences.Count == 0)
        {
            EndTalk();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }
    void EndTalk()
    {
        Debug.Log("End of Talk");
    }
}
