using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//This is more for characters with flavour text - made to make scripting easier.

public class Regular : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    public NPCDialogue dialogueData;
    private NPCDialogue currentDialogue;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public GameObject interactionPrompt;

    private int activeIndex = 0;
    private bool isTyping, isDialogueActive, playerInRange;

    void Start()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (interactionPrompt) interactionPrompt.SetActive(false);
        playerInRange = false;
    }

    void Update()
    {
        // Trigger with E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
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
            CompleteLine(); //Skips typing animation
        }
        else
        {
            NextLine(); //Next part of the dialogue
        }
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
    }

    void CompleteLine()
    {
        StopAllCoroutines();
        dialogueText.text = currentDialogue.dialogueLines[activeIndex];
        isTyping = false;
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

    public void EndDialogue()
    {
        isDialogueActive = false;
        if (dialoguePanel) dialoguePanel.SetActive(false);

        if (PlayerController.Instance != null) PlayerController.Instance.canMove = true;

        dialogueText.text = "";
        nameText.text = "";

        if (playerInRange && interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null && !isDialogueActive)
                interactionPrompt.SetActive(true);
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