using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private int activeIndex = 0;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;

    private bool isTyping, isDialogueActive;

    public GameObject choiceContainer;
    public GameObject ChoicesPrefab;
    public GameObject e_1;
    public GameObject[] movementPrompts;

    void Start()
    {
        if (GameState.shouldStartTutorial)
        {
            GameState.shouldStartTutorial = false;
            TutorialStart();
        }
    }

    public bool CanInteract() => !isDialogueActive;

    [System.Obsolete]
    public void Interact()
    {
        Debug.Log("Interact called on: " + gameObject.name); // ADD THIS LINE

        Dialogue tutorialScript = FindObjectOfType<Dialogue>();
        if (tutorialScript != null && tutorialScript.isActiveAndEnabled)
        {
            tutorialScript.Interact();
        }

        if (!isDialogueActive)
        {
            StartDialogue();
            return;
        }

        if (isTyping)
        {
            CompleteLine();
        }
        // FIX: Check if the container is null OR if it has 0 children (no buttons)
        else if (choiceContainer == null || choiceContainer.transform.childCount == 0)
        {
            NextLine();
        }
    }

    public void TutorialStart()
    {
        if (dialogueData == null) return;

        isDialogueActive = true;
        activeIndex = 0;
        nameText.SetText(dialogueData.npcName);
        dialoguePanel.SetActive(true);

        // Hide WASD prompts to focus on dialogue
        TogglePrompts(false);

        if (PlayerController.Instance != null)
            PlayerController.Instance.canMove = false;

        StartCoroutine(TypeLine());
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        activeIndex = 0;
        nameText.SetText(dialogueData.npcName);
        dialoguePanel.SetActive(true);

        if (PlayerController.Instance != null)
            PlayerController.Instance.canMove = false;

        TogglePrompts(false);
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";
        ClearChoices();

        foreach (char letter in dialogueData.dialogueLines[activeIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;
        CheckForChoices();
    }

    void CompleteLine()
    {
        StopAllCoroutines();
        dialogueText.text = dialogueData.dialogueLines[activeIndex];
        isTyping = false;
        CheckForChoices();
    }

    void NextLine()
    {
        activeIndex++;
        if (activeIndex < dialogueData.dialogueLines.Length)
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
        // FIX: Safety check to prevent line 139 crash
        if (choiceContainer == null || dialogueData.choices == null || dialogueData.choices.Length == 0) return;

        ClearChoices();
        foreach (var choice in dialogueData.choices)
        {
            if (choice.dialogueIndex == activeIndex)
            {
                CreateChoices(choice);
            }
        }
    }

    public void CreateChoices(DialogueChoice choiceData)
    {
        for (int i = 0; i < choiceData.choiceLabels.Length; i++)
        {
            GameObject btn = Instantiate(ChoicesPrefab, choiceContainer.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choiceData.choiceLabels[i];
            int targetIndex = choiceData.nextDialogueIndexes[i];

            btn.GetComponent<Button>().onClick.AddListener(() => {
                activeIndex = targetIndex;
                StartCoroutine(TypeLine());
            });
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        if (PlayerController.Instance != null) PlayerController.Instance.canMove = true;
        TogglePrompts(true); // Bring prompts back when done
    }

    void ClearChoices()
    {
        if (choiceContainer == null) return;
        foreach (Transform child in choiceContainer.transform) Destroy(child.gameObject);
    }

    void TogglePrompts(bool show)
    {
        foreach (GameObject prompt in movementPrompts)
        {
            if (prompt != null) prompt.SetActive(show);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) e_1.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            e_1.SetActive(false);
            EndDialogue();
        }
    }
}