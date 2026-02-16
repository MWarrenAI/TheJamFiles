using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePath : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    private NPCDialogue currentDialogue;
    public NPCDialogue dialogueData;
    public NPCDialogue noResponse;

    public NPCDialogue Path_1a_Data;
    public NPCDialogue EfIntro;
    public NPCDialogue Path_1a_Part2;
    public NPCDialogue LightTut;
    public NPCDialogue ErnieCompliment;
    public NPCDialogue Path_2a_Data;
    public NPCDialogue EfAnnoyed;
    public NPCDialogue Path_1b_Data;

    public GameObject tut_Panel;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public GameObject choiceContainer;
    public GameObject choicePrefab;
    public GameObject interactionPrompt;
    public GameObject erbie;

    [Header("Settings")]
    public DialogueChoice[] choices;

    [System.Serializable]
    public class DialogueChoice
    {
        public NPCDialogue belongingToDialogue;
        public int dialogueIndex;
        public string[] choices;
        public int[] nextDialogueIndex;
        public NPCDialogue[] nextDialogueSO;
    }

    private int activeIndex = 0;
    private bool isTyping, isDialogueActive, playerInRange;

    public void Start()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (interactionPrompt) interactionPrompt.SetActive(false);
        playerInRange = false;
        isDialogueActive = false;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Stops text progress when e appears
            if (choiceContainer.activeInHierarchy && choiceContainer.transform.childCount > 0) return;

            Interact();
        }

        // 1 or 2 for choices
        if (isDialogueActive && choiceContainer.activeInHierarchy && choiceContainer.transform.childCount > 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) KeyBinding(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) KeyBinding(1);
        }
    }

    public bool CanInteract() => !isDialogueActive;

    public void Interact()
    {
        if (!isDialogueActive)
        {
            StartDialogue();
        }
        else if (isTyping)
        {
            CompleteLine();
        }
        else
        {
            NextLine();
        }
    }

    void StartDialogue()
    {
        if (!GameState.hello)
        {
            currentDialogue = dialogueData;
        }
        else if (GameState.efintro)
        {
            currentDialogue = EfIntro;//EfIntro; //"...I'm Ef..."
        }
        else if (!GameState.efannoyed)
        {
            currentDialogue = EfAnnoyed;// EfAnnoyed; //"If you don't know..."
        }
        else if (!GameState.efcomplimented)
        {
            currentDialogue = ErnieCompliment;//ErnieCompliment; //"Pretty"
        }
        else if (!GameState.dejected)
        {
            currentDialogue = noResponse;
        }
        else if (!GameState.lighttut)
        {
            currentDialogue = LightTut;// LightTut;// "I've been in the darkness before..."
        }
        else if (!GameState.efimpatient)
        {
            currentDialogue = Path_2a_Data; //Path_1a_Part2; //"Slow Down!"
        }
        else if (!GameState.efimpatient2)
        {
            currentDialogue = Path_1a_Data; //Path_1a_Data; // "W-wait!! At least, before you go..."
        }
        else if (!GameState.trenches)
        {
            currentDialogue = Path_2a_Data; // Path_2a_Data; //"Yes the trenches..."
        }
        else if (!GameState.fin)
        {
            currentDialogue = Path_1b_Data; // Path_1b_Data; // "There's a side road..."
        }

        if (currentDialogue == null) return;
        ExecuteStart();
    }

    private void ExecuteStart()
    {
        isDialogueActive = true;
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
        activeIndex = 0;

        nameText.SetText(currentDialogue.npcName);
        dialoguePanel.SetActive(true);

        if (PlayerController.Instance != null) PlayerController.Instance.canMove = false;

        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in currentDialogue.dialogueLines[activeIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(currentDialogue.typingSpeed);
        }
        isTyping = false;
        CheckForChoices();
    }

    void CompleteLine()
    {
        StopAllCoroutines();
        dialogueText.text = currentDialogue.dialogueLines[activeIndex];
        isTyping = false;
        CheckForChoices();
    }

    void NextLine()
    {
        activeIndex++;
        if (activeIndex < currentDialogue.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void CheckForChoices()
    {
        ClearChoices();
        foreach (var choiceData in choices)
        {
            if (choiceData.belongingToDialogue == currentDialogue && choiceData.dialogueIndex == activeIndex)
            {
                CreateChoices(choiceData);
                break;
            }
        }
    }

    public void CreateChoices(DialogueChoice choiceData)
    {
        ClearChoices();
        choiceContainer.SetActive(true);

        // ensure the container is active
        if (choiceContainer.transform.parent != null)
            choiceContainer.transform.parent.gameObject.SetActive(true);

        for (int i = 0; i < choiceData.choices.Length; i++)
        {
            GameObject btn = Instantiate(choicePrefab, choiceContainer.transform);
            btn.GetComponentInChildren<TMP_Text>().text = choiceData.choices[i];

            int index = i;
            btn.GetComponent<Button>().onClick.AddListener(() => TriggerChoice(index, choiceData));
        }
    }

    private void KeyBinding(int index)
    {
        foreach (var c in choices)
        {
            if (c.belongingToDialogue == currentDialogue && c.dialogueIndex == activeIndex)
            {
                if (index < c.choices.Length) TriggerChoice(index, c);
                break;
            }
        }
    }

    private void TriggerChoice(int choiceIndex, DialogueChoice choiceData)
    {
        UpdateGameState(currentDialogue);
        ClearChoices();

        // Switch Scripts!
        if (choiceData.nextDialogueSO != null && choiceIndex < choiceData.nextDialogueSO.Length && choiceData.nextDialogueSO[choiceIndex] != null)
        {
            currentDialogue = choiceData.nextDialogueSO[choiceIndex];
            activeIndex = 0;
        }
        //Jump to a line
        else if (choiceData.nextDialogueIndex != null && choiceIndex < choiceData.nextDialogueIndex.Length)
        {
            activeIndex = choiceData.nextDialogueIndex[choiceIndex];
        }
        else
        {
            EndDialogue();
            return;
        }

        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        isTyping = false;
        if (dialoguePanel) dialoguePanel.SetActive(false);
        ClearChoices();

        if (PlayerController.Instance != null) PlayerController.Instance.canMove = true;
        if (playerInRange && interactionPrompt) interactionPrompt.SetActive(true);
    }

    private void UpdateGameState(NPCDialogue finishedSO)
    {
        if (finishedSO == dialogueData) GameState.hello = true;
        else if (finishedSO == EfAnnoyed) GameState.efannoyed = true;
        else if (finishedSO == ErnieCompliment) GameState.efcomplimented = true;
        else if (finishedSO == noResponse) GameState.dejected = true;
        else if (finishedSO == LightTut) GameState.lighttut = true;
        else if (finishedSO == Path_2a_Data) GameState.efimpatient = true;
        else if (finishedSO == Path_1a_Data) GameState.efimpatient2 = true;
    }

    void ClearChoices()
    {
        if (choiceContainer == null) return;
        foreach (Transform child in choiceContainer.transform) Destroy(child.gameObject);

        choiceContainer.SetActive(false);
        if (choiceContainer.transform.parent != null && choiceContainer.transform.parent.gameObject != dialoguePanel)
        {
            choiceContainer.transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null && !isDialogueActive) interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
            if (isDialogueActive) EndDialogue();
        }
    }
}