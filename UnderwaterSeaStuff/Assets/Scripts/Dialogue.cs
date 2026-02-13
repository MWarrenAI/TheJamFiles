using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//This script is intended for the tutorial portion of dialogue

public class Dialogue : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    private NPCDialogue currentDialogue;
    public NPCDialogue dialogueData;  // First interaction data
    public NPCDialogue ErnieBallData; // Second interaction data
    public NPCDialogue reminderErnie; // This gets triggered after they promise to get them Ernie's ball
    public NPCDialogue ErniePlay;     // This triggers Ernie throwing the ball
    public NPCDialogue noPlay;        // This triggers Ernie resigning ball play  
    public NPCDialogue noChoiceData;  // This triggers the same dialogue as no play - I think I added this because I wasn't sure how the logic was flowing and I wanted to avoid mistakes

    private int activeIndex = 0; //This is what line they are on
    public GameObject dialoguePanel; //This contains where the dialogue goes
    public TMP_Text dialogueText, nameText; //Here's the text that appears on the dialogue panel
    private bool tutorialFinished = false; //This allows for freeroam after the tutorial is finished (the walls disappear)

    private bool playerInRange; //This bool is a variable that gets set to true when the player is in range of the object
    private bool isTyping, isDialogueActive; //These bools are required for the typing animations and checking if the dialogue window is active

    [Header("UI References")]
    public GameObject choiceContainer; //This contains the Prefab
    public GameObject ChoicesPrefab; //This contains the buttons
    public GameObject e_1; //the e prompt
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

    [System.Serializable]
    public class DialogueChoice
    {
        public int dialogueIndex; // This is which line they are on currently.
        public string[] choices; // The yes/no or 1, 2, 3 options.
        public int[] nextDialogueIndex; // Where the script jumps to (line 1, line 2, line 3...)
    }

    void Start()
    {
        //This shows the keyboard instructions when they initially start the conversation (this serves as a instructions tutorial)
        //This also hides the e prompt and checks and makes sure the dialogue panel and choices panel are both inactive
        //We also want to make sure the player is by default not in range as ernie is not nearby
        TogglePrompts(true);
        tut_Panel.SetActive(false);
        if (e_1 != null) e_1.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(false);
        playerInRange = false;

        //This then triggers the tutorial only after they start this script - this therefore makes sure it only triggers once which is great for tutorials.
        if (GameState.shouldStartTutorial && !GameState.tutorialFinished)
        {
            TutorialStart();
        }
    }

    private void TriggerChoice(int choiceIndex)
    {
        //this handles are logic for when they pick the no choice - removing all the walls, we do want them to repeat it as it is both amusing and teaches them about interaction with objects.
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
                    //This is a null handle just in case I forget to add an index
                    if (choice.nextDialogueIndex == null || choice.nextDialogueIndex.Length <= choiceIndex)
                    {
                        Debug.LogError("DIALOGUE ERROR: 'Next Dialogue Index' is missing an entry for choice " + choiceIndex);
                        EndDialogue();
                        return;
                    }
                    activeIndex = choice.nextDialogueIndex[choiceIndex];
                }

                //This clears the choices after they are made
                if (activeIndex >= 0 && activeIndex < currentDialogue.dialogueLines.Length)
                {
                    if (keyboardChoiceUI != null) keyboardChoiceUI.SetActive(false);
                    ClearChoices();
                    StartCoroutine(TypeLine());
                }
                //Self explanatory - ends the dialogue
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
        //This handles the logic for when the user presses e 
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (choiceContainer.transform.childCount > 0) return;

            //if the dialogue is active and the player is in range they can therefore interact
            if (isDialogueActive || playerInRange)
            {
                Interact();

            }
        }
        if (isDialogueActive && choiceContainer.transform.childCount > 0)
        {
            if (Input.GetKeyDown(KeyCode.Y)) TriggerChoice(0);
            if (Input.GetKeyDown(KeyCode.N)) TriggerChoice(1);
        }
    }
    

    public bool CanInteract() => !isDialogueActive;

    public void Interact()
    {
        //This triggers the dialogue and keeps the dialogue animated whilst it is being generated, if there is no more text it goes onto the next line.
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

    //this is what is triggered only after the tutorial is started.
    public void TutorialStart()
    {
        StartDialogue();
    }

    //because there was a lot that could go wrong here I included a debug that tells me if the player either has the ball or ernie has lost the ball.
    //certain states trigger certain dialogues - this is how all my script logic flows perfectly.
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

    //this sets state to lostball and returns the dialogue object ErnieBallData
    private NPCDialogue SetLostBallState()
    {
        GameState.lostBall = true;
        return ErnieBallData;
    }
    public GameObject ballObject;
    
    //this resets states and returns the dialogue object ErniePlay therefore restoring the ball to it's original position (troll face)
    private NPCDialogue ReturnBallAndReset()
    {
        GameState.hasBall = false;
        GameState.lostBall = false;
        NPCDialogue result = ErniePlay;
        if (ballObject != null) ballObject.SetActive(true);
        return result;
    }

    //this prevents the player from moving whilst the dialogue is active it handles simple logic like
    //whether the e prompt is active (usually it's not during dialogue) and activates the panel and dialogue panel for interaction
    //this also starts the typing animation
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

    //This types each letter of every line for dialogue like a typewriter would "appearing animated"
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

    //This handles the logic after the text is finished - whether or not there are choices
    void CompleteLine()
    {
        StopAllCoroutines();
        dialogueText.text = currentDialogue.dialogueLines[activeIndex];
        isTyping = false;
        CheckForChoices();
    }

    //This checks if there is more dialogue to generate
    void NextLine()
    {
        activeIndex++;
        if (activeIndex < currentDialogue.dialogueLines.Length)
            StartCoroutine(TypeLine());
        else
            EndDialogue();
    }

    //This checks for choices
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

    //this generates the options for dialogue
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
            if (btnText != null)
            {
                    btnText.text = choiceData.choices[i];
            }

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

    //this ensures no unnecessary panels/text remain after dialogue and executes the revised script only after the noplay dialogue
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

        if (PlayerController.Instance != null) PlayerController.Instance.canMove = true;
        dialogueText.text = "";
        nameText.text = "";
        TextE.text = "";
        TextU.text = "";
        TextD.text = "";
        TextL.text = "";
        TextR.text = "";
        TextSpa.text = "";

        if (playerInRange)
        {
            if (e_1 != null) e_1.SetActive(true);
        }

    }

    //this clears the choices from the prompt menu
    void ClearChoices()
    {
        if (choiceContainer == null) return;
        foreach (Transform child in choiceContainer.transform) Destroy(child.gameObject);
    }

    //This shows the buttons for the instructions (there were a lot of them so I put them in an array)
    void TogglePrompts(bool hide)
    {
        foreach (GameObject prompt in movementPrompts)
            if (prompt != null) prompt.SetActive(hide);
    }

    //this shows the e button above ernie!
    private void OnTriggerEnter2D(Collider2D other)
    {
            playerInRange = true;
            if (e_1 != null && !isDialogueActive) e_1.SetActive(true);
    }

    //this hides the e button above ernie!
    private void OnTriggerExit2D(Collider2D other)
    {
            playerInRange = false;
            if (e_1 != null) e_1.SetActive(false);
            if (isDialogueActive) EndDialogue();
        
    }
}