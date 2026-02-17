using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//This script is intended for the tutorial portion of dialogue

public class Dialogue : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    private NPCDialogue currentDialogue;
    public NPCDialogue dialogueData;      // First interaction data
    public NPCDialogue ErnieBallData;     // Second interaction data
    public NPCDialogue reminderErnie;
    public NPCDialogue ErniePlay;
    public NPCDialogue noPlay;
    public NPCDialogue noChoiceData;

    private int activeIndex = 0;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    private bool tutorialFinished = false;

    private bool playerInRange;
    private bool isTyping, isDialogueActive;

    [Header("UI References")]

    public GameObject choiceContainer; //This contains the Prefab
    public GameObject ChoicesPrefab; //This contains the buttons
    public GameObject e_1; //ernie's e prompt
    public GameObject e_5; //erbie's e prompt
    public GameObject tut1; //The barrier blocking the player
    public GameObject tut2; //The barrier blocking the player
    public GameObject tut3; //The barrier blocking the player
    public GameObject tut4; //The barrier blocking the player
    public GameObject[] movementPrompts; //I believe these are the button prompts that appear to instruct the player
    public GameObject Panel; //This is the choices panel (not to be confused with the dialogue panel)
    public GameObject keyboardChoiceUI; //I believe this was also something I got confused with logic wise, it may not be needed but would have taken too long to change
    public TMP_Text TextE; //What the e button does
    public TMP_Text TextU; //What the w/up button does
    public TMP_Text TextD; //What the s/down button does
    public TMP_Text TextL; //What the a/left button does
    public TMP_Text TextR; //What the d/right button does
    public TMP_Text TextSpa; //What the space button does
    // This list will appear in the NPC Inspector in Unity
    public DialogueChoice[] choices; //this stores all the player choices have in an array

    public GameObject tut_Panel; //tutorial panel
    public GameObject erbie; //ernie's twin!

    [System.Serializable]
    public class DialogueChoice
    {
        public int dialogueIndex;
        public string[] choices; // Labels like "Yes", "No"
        public int[] nextDialogueIndex; // Where they jump to
    }

    //hides the various prompts and keeps their twin hidden for now
    void Start()
    {
        TogglePrompts(true);
        tut_Panel.SetActive(false);
        e_5.SetActive(false);
        if (e_1 != null) e_1.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(false);
        playerInRange = false;
        erbie.SetActive(false);
        isDialogueActive = false;

        //Triggers the tutorial for flavour text on script load.
        if (GameState.shouldStartTutorial && !GameState.tutorialFinished)
        {
            TutorialStart();
        }
    }

    //handles the logic for changing the gamestate and deactivating the walls so the player can freeroam
    private void TriggerChoice(int choiceIndex)
    {
        foreach (var choice in choices)
        {
            if (choice.dialogueIndex == activeIndex)
            {
                if (choiceIndex == 1 && noChoiceData != null)
                {
                    currentDialogue = noChoiceData;
                    GameState.noPlay = true;
                    activeIndex = 0;
                    tut1.SetActive(false);
                    tut2.SetActive(false);
                    tut3.SetActive(false);
                    tut4.SetActive(false);

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

                //Choice dialogue logic
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
        //handles interaction logic and keydown for e
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (choiceContainer.transform.childCount > 0)
            {
                return;
            }

            if (isDialogueActive || playerInRange)
            {
                Interact();
            }
        }

        //handles yes no options for y n keybindings
        if (isDialogueActive && GameState.tutorialFinished && choiceContainer.transform.childCount > 0)
        {
            if (Input.GetKeyDown(KeyCode.Y)) TriggerChoice(0);
            if (Input.GetKeyDown(KeyCode.N)) TriggerChoice(1);
        }
    }

    public bool CanInteract() => !isDialogueActive;

    //ensures the interaction logic flows smoothly - so animation can be played, text can be skipped to allow users to interact better with the speed of the dialog
    //next line of the dialog follows, etc.
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

    //Ensures the right command is triggered
    public void TutorialStart()
    {
        StartDialogue();
    }

    //handles some of the tutorial logic and includes a debug that doesn't appear in-game so it's fine. Gamestates can switch and different dialogue objects can be loaded.
    void StartDialogue()
    {
        Debug.Log($"Tutorial: {GameState.tutorialFinished}, HasBall: {GameState.hasBall}, LostBall: {GameState.lostBall}");
        if (!GameState.tutorialFinished)
        {
            currentDialogue = dialogueData;
        }
        else if (GameState.hasBall)
        {
            currentDialogue = ReturnBallAndReset();
        }
        else if (!GameState.lostBall)
        {
            currentDialogue = SetLostBallState();
        }
        else
        {
            currentDialogue = reminderErnie;
        }


        if (currentDialogue == null) return;
        ExecuteStart();
    }

    //sets the ball be lost state
    private NPCDialogue SetLostBallState()
    {
        GameState.lostBall = true;
        return ErnieBallData;
    }
    public GameObject ballObject;

    //this returns the ball to it's previous state and makes it visible again
    private NPCDialogue ReturnBallAndReset()
    {
        GameState.hasBall = false;
        GameState.lostBall = false;
        NPCDialogue result = ErniePlay;
        if (ballObject != null) ballObject.SetActive(true);
        return result;
    }

    //this handles some of the dialog logic hiding e promps, showing panels, and ensuring the animation works for the dialog
    private void ExecuteStart()
    {
        isDialogueActive = true;
        if (e_1 != null) e_1.SetActive(false);
        activeIndex = 0;

        nameText.SetText(currentDialogue.npcName);
        dialoguePanel.SetActive(true);
        Panel.SetActive(true);

        if (PlayerController.Instance != null)
            PlayerController.Instance.canMove = false;

        StartCoroutine(TypeLine());
    }

    //this handles the dialog animation itself
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

    //this stops the dialog animation by skipping to complete the dialog text generated
    void CompleteLine()
    {
        StopAllCoroutines();
        dialogueText.text = currentDialogue.dialogueLines[activeIndex];
        isTyping = false;
        CheckForChoices();
    }

    //moves onto the next sequence in the dialog script
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

    //creates choices for user prompts
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

    //Create choices logic, hides various objects, makes various objects visible and instantiates the button so that it can be displayed
    public void CreateChoices(DialogueChoice choiceData)
    {
        ClearChoices();

        if (choiceContainer == null || ChoicesPrefab == null)
        {
            Debug.LogError("Dialogue Error: Choice Container or Prefab is missing in the Inspector!");
            return;
        }

        choiceContainer.SetActive(true);
        if (Panel != null) Panel.SetActive(true);

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

    //hides everything that need not be there and after they are done with the tutorial erbie appears.
    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        Panel.SetActive(false);
        TogglePrompts(false);
        ClearChoices();


        if (currentDialogue == dialogueData)
        {
            GameState.tutorialFinished = true;
        }

        if (currentDialogue == noPlay)
        {
            erbie.SetActive(true);
        }

        if (PlayerController.Instance != null) PlayerController.Instance.canMove = true;
        {
            dialogueText.text = "";
            nameText.text = "";
            TextE.text = "";
            TextU.text = "";
            TextD.text = "";
            TextL.text = "";
            TextR.text = "";
            TextSpa.text = "";
        }


        if (playerInRange)
        {
            if (e_1 != null) e_1.SetActive(true);
        }

    }

    //clears the choices from the prefab
    void ClearChoices()
    {
        if (choiceContainer == null)
        {
            return;
        }
        foreach (Transform child in choiceContainer.transform)
        {
            Destroy(child.gameObject);
        }
        if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(false);
    }

    //hides the prompt images (not the text)
    void TogglePrompts(bool hide)
    {
        foreach (GameObject prompt in movementPrompts)
        {
            if (prompt != null)
            {
                prompt.SetActive(hide);
            }
        }
    }

    //shows e prompt when inside the collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        playerInRange = true;

        if (e_1 != null && !isDialogueActive)
        {
            e_1.SetActive(true);
        }
    }

    //hides e prompt when outside the collider
    private void OnTriggerExit2D(Collider2D other)
    {
        playerInRange = false;

        if (e_1 != null)
        {
            e_1.SetActive(false);
        }
        if (isDialogueActive)
        {
            EndDialogue();
        }

    }
}