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

    public NPCDialogue Path_1a_Data; // "W-wait!! At least, before you go..."
    public NPCDialogue EfIntro; //"...I'm Ef..."
    public NPCDialogue Path_1a_Part2; //"Slow Down!"
    public NPCDialogue LightTut; // "I've been in the darkness before..."
    public NPCDialogue ErnieCompliment; //"Pretty"
    public NPCDialogue Path_2a_Data; //"Yes the trenches..."
    public NPCDialogue EfAnnoyed; //"If you don't know..."
    public NPCDialogue Path_1b_Data; // "There's a side road..."
    
    public GameObject tut_Panel; //tutorial panel

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public GameObject choiceContainer;
    public GameObject choicePrefab;
    public GameObject interactionPrompt;

    [Header("Settings")]
    public DialogueChoice[] choices;

    [System.Serializable]
    public class DialogueChoice
    {
        public int dialogueIndex;
        public string[] choices;
        public int[] nextDialogueIndex; 
    }

    private int activeIndex = 0;
    private bool isTyping, isDialogueActive, playerInRange;

    public void TriggerPath()
    {
        StartDialogue();
    }
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
            if (choiceContainer.transform.childCount > 0) return; //stops player pressing e to progress


            if (isDialogueActive || playerInRange)
            {
                Interact();
            }
        }

        if (isDialogueActive && choiceContainer.transform.childCount > 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) TriggerChoice(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) TriggerChoice(1);
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

    public void TutorialStart()
    {
        StartDialogue();
    }

    void StartDialogue()
    {
        currentDialogue = dialogueData;
        if (currentDialogue == null) return;

        isDialogueActive = true;
        activeIndex = 0;

        if (interactionPrompt) interactionPrompt.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(true);
        if (nameText) nameText.text = currentDialogue.npcName;

        if (PlayerController.Instance != null) PlayerController.Instance.canMove = false;

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
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
        ClearChoices();
        if (choiceContainer.transform.childCount > 0) return;

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
    private void TriggerChoice(int choiceIndex)
    {
        foreach (var choice in choices)
        {
            if (choice.dialogueIndex == activeIndex)
            {
                if (choiceIndex == 1 && noResponse != null)
                {
                    currentDialogue = noResponse;
                    activeIndex = 0;
                    ClearChoices();
                    StartCoroutine(TypeLine());
                }
                else
                {
                    int targetIndex = choice.nextDialogueIndex[choiceIndex];

                    if (targetIndex < 0 || targetIndex >= currentDialogue.dialogueLines.Length)
                    {
                        EndDialogue();
                        return;
                    }

                    activeIndex = targetIndex;
                    ClearChoices();
                    StartCoroutine(TypeLine());
                }
                break;
            }
        }
    }

    

    void CheckForChoices()
    {
        ClearChoices();
        Debug.Log("Checking choices for index: " + activeIndex);

        if (currentDialogue != dialogueData) return; //only checks for choices in main dialogue

        foreach (var choice in choices)
        {
            if (choice.dialogueIndex == activeIndex)
            {
                Debug.Log("Found choice for index " + activeIndex + "! Creating buttons...");
                CreateChoices(choice);
                break;
            }
        }
    }

    public void CreateChoices(DialogueChoice choiceData)
    {
        ClearChoices();

        if (choiceContainer.transform.parent != null)
        {
            choiceContainer.transform.parent.gameObject.SetActive(true);
        }

        choiceContainer.SetActive(true);

        Debug.Log($"Attempting to spawn {choiceData.choices.Length} buttons into {choiceContainer.name}");

        if (choiceContainer == null || choicePrefab == null) return;

        for (int i = 0; i < choiceData.choices.Length; i++)
        {
            GameObject btn = Instantiate(choicePrefab, choiceContainer.transform);
            btn.name = "CHOICE_BUTTON_" + i;
            btn.SetActive(true);
            TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
            if (btnText != null) btnText.text = choiceData.choices[i];

            int choiceIndex = i;
            Button but = btn.GetComponent<Button>();
            if (btn != null)
            {
                but.onClick.AddListener(() => TriggerChoice(choiceIndex));
            }
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        ClearChoices();

        if (currentDialogue == dialogueData)
        {
            GameState.tutorialFinished = true;
        }

        if (PlayerController.Instance != null) PlayerController.Instance.canMove = true;
        dialogueText.text = "";
        nameText.text = "";
        

        if (playerInRange)
        {
            if (interactionPrompt != null) interactionPrompt.SetActive(true);
        }

    }

    void ClearChoices()
    {
        if (choiceContainer == null) return;
        foreach (Transform child in choiceContainer.transform) Destroy(child.gameObject);

        if (choiceContainer.transform.parent != null)
        {
            choiceContainer.transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerInRange = true;
        if (interactionPrompt != null && !isDialogueActive) interactionPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerInRange = false;
        if (interactionPrompt != null) interactionPrompt.SetActive(false);

    }
}