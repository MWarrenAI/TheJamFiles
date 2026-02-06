using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

[System.Serializable]
public class Talky
{
<<<<<<< HEAD
    [Header("Dialogue Data")]
    private NPCDialogue currentDialogue;
    public NPCDialogue dialogueData;      // Greeting
    public NPCDialogue ErnieBallData;     // Player tasked
    public NPCDialogue BallData;          // Ball Interaction/Get ball
    public NPCDialogue noChoiceData;      // Player skips tutorial

    private int activeIndex = 0;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    private bool tutorialFinished = false;

    private bool playerInRange;
    private bool isTyping, isDialogueActive;
    private bool isInteractingWithBall = false; // Moved up for clarity

    [Header("UI References")]
    public GameObject choiceContainer;
    public GameObject ChoicesPrefab;
    public GameObject e_1; // NPC Prompt
    public GameObject e_2; // Ball Prompt
    public GameObject[] movementPrompts;
    public GameObject Panel;
    public GameObject keyboardChoiceUI;

    public DialogueChoice[] choices;

    [System.Serializable]
    public class DialogueChoice
    {
        public int dialogueIndex;
        public string[] choices;
        public int[] nextDialogueIndex;
    }

    void Start()
    {
        TutorialStart();
        if (e_1 != null) e_1.SetActive(false);
        if (e_2 != null) e_2.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false); // Changed to false by default
        if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(false);
        playerInRange = false;
        TogglePrompts(false);
        if (GameState.shouldStartTutorial)
        {
            GameState.shouldStartTutorial = false;
            TutorialStart();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (keyboardChoiceUI != null && keyboardChoiceUI.activeSelf) return;

            if (isDialogueActive)
            {
                Interact();
            }
            else if (playerInRange)
            {
                Interact();
            }
        }

        if (isDialogueActive && keyboardChoiceUI != null && keyboardChoiceUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) TriggerChoice(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) TriggerChoice(1);
        }

        if (!isDialogueActive && playerInRange)
        {
            if (isInteractingWithBall && e_2 != null && !e_2.activeSelf) e_2.SetActive(true);
            if (!isInteractingWithBall && e_1 != null && !e_1.activeSelf) e_1.SetActive(true);
        }


    }

    public void Interact()
    {
        if (!isDialogueActive) StartDialogueSequence();
        else if (isTyping) CompleteLine();
        else NextLine();
    }



    private void StartDialogueSequence()
    {
        // Decide which data to use based on what we are currently touching
        if (isInteractingWithBall)
        {
            currentDialogue = BallData;
        }
        else
        {
            currentDialogue = tutorialFinished ? ErnieBallData : dialogueData;
        }

        if (currentDialogue == null) return;

        isDialogueActive = true;
        activeIndex = 0;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (Panel != null) Panel.SetActive(true);
        if (nameText != null) nameText.SetText(currentDialogue.npcName);

        if (e_1 != null) e_1.SetActive(false);
        if (e_2 != null) e_2.SetActive(false);

        TogglePrompts(true);

        if (PlayerController.Instance != null)
            PlayerController.Instance.canMove = false;

        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    // --- KEY TRIGGER FIXES START HERE ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        // We check 'other', which is the thing the Player just touched
        if (other.CompareTag("Ball"))
        {
            playerInRange = true;
            isInteractingWithBall = true;
            if (e_2 != null) e_2.SetActive(true);
            if (e_1 != null) e_1.SetActive(false);
        }
        else if (other.CompareTag("Ernie"))
        {
            playerInRange = true;
            isInteractingWithBall = false;
            if (e_1 != null) e_1.SetActive(true);
            if (e_2 != null) e_2.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Only reset if we are leaving the ball or Ernie
        if (other.CompareTag("Ball") || other.CompareTag("Ernie"))
        {
            playerInRange = false;
            isInteractingWithBall = false;
            if (e_1 != null) e_1.SetActive(false);
            if (e_2 != null) e_2.SetActive(false);
            if (isDialogueActive) EndDialogue();
        }
    }

    // --- (Rest of the typing/choice logic remains the same) ---

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
    public bool CanInteract()
    {
        return !isDialogueActive && playerInRange;
    }

    private void ExecuteDialogueSequence()
    {
        isDialogueActive = true;
        activeIndex = 0;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (Panel != null) Panel.SetActive(true);
        if (nameText != null) nameText.SetText(currentDialogue.npcName);

        TogglePrompts(true);

        if (PlayerController.Instance != null)
            PlayerController.Instance.canMove = false;

        StopAllCoroutines();
        StartCoroutine(TypeLine());
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
            StartCoroutine(TypeLine());
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        isTyping = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (Panel != null) Panel.SetActive(false);
        if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(false);
        ClearChoices();
        if (dialogueText != null) dialogueText.text = "";
        if (nameText != null) nameText.text = "";
        tutorialFinished = true;
        if (PlayerController.Instance != null) 
            PlayerController.Instance.canMove = true;
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    void TogglePrompts(bool hide)
    {
        foreach (GameObject prompt in movementPrompts)
        {
            if (prompt != null)
                prompt.SetActive(!hide);
        }
    }


    void ClearChoices()
    {
        if (choiceContainer == null) return;
        foreach (Transform child in choiceContainer.transform) Destroy(child.gameObject);
        if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(false);
    }

    void CheckForChoices()
    {
        ClearChoices();
        foreach (var choice in choices)
        {
            if (choice.dialogueIndex == activeIndex)
            {
                CreateChoices(choice);
                break;
            }
        }
    }

    public void CreateChoices(DialogueChoice choiceData)
    {
        ClearChoices();
        for (int i = 0; i < choiceData.choices.Length; i++)
        {
            GameObject btn = Instantiate(ChoicesPrefab, choiceContainer.transform);
            btn.GetComponentInChildren<TMP_Text>().text = choiceData.choices[i];
            int index = i;
            btn.GetComponent<Button>().onClick.AddListener(() => TriggerChoice(index));
        }
        if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(true);
    }

    private void TriggerChoice(int choiceIndex)
    {
        foreach (var choice in choices)
        {
            if (choice.dialogueIndex == activeIndex)
            {
                activeIndex = choice.nextDialogueIndex[choiceIndex];
                ClearChoices();
                StartCoroutine(TypeLine());
                break;
            }
        }
    }

    public void TutorialStart()
    {
        currentDialogue = dialogueData;

        if (currentDialogue != null)
        {
            ExecuteDialogueSequence();
        }
        else
        {
            Debug.LogError("Dialogue Error: dialogueData is missing in the Inspector!");
        }

    }
=======
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


>>>>>>> parent of 7ae1c72 (buggy)
}