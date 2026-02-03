using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;

    [Header("Keyboard Choice UI")]
    public GameObject choicePanel; 
    public TMP_Text choiceText;   

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowKeyboardChoices(string text)
    {
        choicePanel.SetActive(true);
        if (choiceText != null) choiceText.text = text;
    }

    public void HideChoices()
    {
        choicePanel.SetActive(false);
    }
}