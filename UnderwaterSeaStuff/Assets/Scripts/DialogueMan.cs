using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMan : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public GameObject dialogueBox;
    public GameObject Player;
    public Transform choiceContainer;
    public GameObject ChoicesPrefab;

    public TextMeshProUGUI TextSpa;
    public TextMeshProUGUI TextU;
    public TextMeshProUGUI TextD;
    public TextMeshProUGUI TextL;
    public TextMeshProUGUI TextR;
    public TextMeshProUGUI TextE;
    public GameObject a_0;
    public GameObject s_0;
    public GameObject d_0;
    public GameObject w_0;
    public GameObject e_0;
    public GameObject spa_0;

    private Queue<string> sentences;

    public static DialogueMan Instance { get; private set;}

    private void Start()
    {
        if (Instance = null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        a_0.SetActive(true);
        s_0.SetActive(true);
        d_0.SetActive(true);
        w_0.SetActive(true);
        e_0.SetActive(true);
        spa_0.SetActive(true); 
    }

    void Awake()
    {
        Instance = this;
        sentences = new Queue<string>();
    }

    public void StartTalky(Talky talky)
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.canMove = false;
        }

        if (talky == null)
        {
            Debug.LogWarning("Talky asset is missing!");
            return;
        }

        dialogueBox.SetActive(true);
        Debug.Log("Starting Conversation " + talky.name);
        nameText.text = talky.name;
        sentences.Clear();

        foreach (string sentence in talky.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNext();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DisplayNext();
        }
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

    public void HideControls()
    {
        a_0.SetActive(false);
        s_0.SetActive(false);
        d_0.SetActive(false);
        w_0.SetActive(false);
        e_0.SetActive(false);
        spa_0.SetActive(false);
        TextSpa.text = "";
        TextU.text = "";
        TextD.text = "";
        TextL.text = "";
        TextR.text = "";
        TextE.text = "";

    }

    void EndTalk()
    {
        Debug.Log("End of Talk");
        dialogueBox.SetActive(false);
        dialogueText.text = "";
        nameText.text = "";
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.canMove = true;
        }
        HideControls();
    }

    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);
    }

    public void CreateChoices(string choiceText)
    {
        GameObject Choices = Instantiate(ChoicesPrefab, choiceContainer);
        Choices.GetComponentInChildren<TMP_Text>().text = choiceText;
    }
}
