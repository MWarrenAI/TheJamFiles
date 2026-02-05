using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    private NPCDialogue currentDialogue;
    public NPCDialogue dialogueData;      // First interaction data
    public NPCDialogue ErnieBallData;     // Second interaction data
    public NPCDialogue noChoiceData;


    private int activeIndex = 0;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    private bool tutorialFinished = false;

    private bool playerInRange;
    private bool isTyping, isDialogueActive;

    [Header("UI References")]
    public GameObject choiceContainer;
    public GameObject ChoicesPrefab;
    public GameObject e_1;
    public GameObject[] movementPrompts;
    public GameObject Panel;
    public GameObject keyboardChoiceUI;
    public TMP_Text TextE;
    public TMP_Text TextU;
    public TMP_Text TextD;
    public TMP_Text TextL;
    public TMP_Text TextR;
    public TMP_Text TextSpa;
    // This list will appear in the NPC Inspector in Unity
    public DialogueChoice[] choices;

    [System.Serializable]
    public class DialogueChoice
    {
        public int dialogueIndex;
        public string[] choices; // Labels like "Yes", "No"
        public int[] nextDialogueIndex; // Where they jump to
    }

    void Start()
    {
        if (e_1 != null) e_1.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(false);
        playerInRange = false;

        if (GameState.shouldStartTutorial)
        {
            GameState.shouldStartTutorial = false;
            TutorialStart();
        }
    }

    private void TriggerChoice(int choiceIndex)
    {
        foreach (var choice in choices)
        {
            if (choice.dialogueIndex == activeIndex)
            {
                if (choiceIndex == 1 && noChoiceData != null)
                {
                    currentDialogue = noChoiceData; 
                    activeIndex = 0;               
                }
                else
                {
                    if (choice.nextDialogueIndex == null || choice.nextDialogueIndex.Length <= choiceIndex)
                    {
                        Debug.LogError("DIALOGUE ERROR: 'Next Dialogue Index' is missing an entry for choice " + choiceIndex);
                        EndDialogue();
                        return;
                    }
                    activeIndex = choice.nextDialogueIndex[choiceIndex];
                }

                if (activeIndex >= 0 && activeIndex < currentDialogue.dialogueLines.Length)
                {
                    if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(false);
                    ClearChoices();
                    StartCoroutine(TypeLine());
                }
                else
                {
                    EndDialogue(); 
                }
                break;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (choiceContainer.transform.childCount > 0) return;

            if (isDialogueActive || playerInRange)
            {
                Interact();
            }
        }

        if (isDialogueActive && tutorialFinished && choiceContainer.transform.childCount > 0)
        {
            if (Input.GetKeyDown(KeyCode.Y)) TriggerChoice(0);
            if (Input.GetKeyDown(KeyCode.N)) TriggerChoice(1);
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
        currentDialogue = tutorialFinished ? ErnieBallData : dialogueData;
        if (currentDialogue == null) return;

        ExecuteStart();
    }

    void StartDialogue()
    {
        currentDialogue = tutorialFinished ? ErnieBallData : dialogueData;
        if (currentDialogue == null) return;

        ExecuteStart();
    }

    private void ExecuteStart()
    {
        isDialogueActive = true;
        if (e_1 != null) e_1.SetActive(false);
        activeIndex = 0;

        nameText.SetText(currentDialogue.npcName);
        dialoguePanel.SetActive(true);
        Panel.SetActive(true);
        TogglePrompts(true);

        if (PlayerController.Instance != null)
            PlayerController.Instance.canMove = false;

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
        CheckForChoices(); // Trigger choices after typing finishes
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

    void CheckForChoices()
    {
        ClearChoices();

        if (!tutorialFinished) return;

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

        if (choiceContainer == null || ChoicesPrefab == null)
        {
            Debug.LogError("Dialogue Error: Choice Container or Prefab is missing in the Inspector!");
            return;
        }

        for (int i = 0; i < choiceData.choices.Length; i++)
        {
            GameObject btn = Instantiate(ChoicesPrefab, choiceContainer.transform);

            TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
            if (btnText != null) btnText.text = choiceData.choices[i];

            Button buttonComp = btn.GetComponent<Button>();
            if (buttonComp == null)
            {
                Debug.LogError("DIALOGUE ERROR: Your Choices Prefab is missing a 'Button' component!");
                continue;
            }

            int indexForThisButton = i;
            buttonComp.onClick.AddListener(() => TriggerChoice(indexForThisButton));
        }

        if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(true);
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        Panel.SetActive(false);
        ClearChoices();

        tutorialFinished = true;

        if (PlayerController.Instance != null) PlayerController.Instance.canMove = true;
        dialogueText.text = "";
        nameText.text = "";
        TextE.text = "";
        TextU.text = "";
        TextD.text = "";
        TextL.text = "";
        TextR.text = "";
        TextSpa.text = "";

        
    TogglePrompts(false);

    }

    void ClearChoices()
    {
        if (choiceContainer == null) return;
        foreach (Transform child in choiceContainer.transform) Destroy(child.gameObject);
    }

    void TogglePrompts(bool hide)
    {
        foreach (GameObject prompt in movementPrompts)
            if (prompt != null) prompt.SetActive(hide);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
            playerInRange = true;
            if (e_1 != null && !isDialogueActive) e_1.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
            playerInRange = false;
            if (e_1 != null) e_1.SetActive(false);
            if (isDialogueActive) EndDialogue();
        
    }
}