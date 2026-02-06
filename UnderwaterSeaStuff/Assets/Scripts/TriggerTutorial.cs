using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriggerTutorial : MonoBehaviour
{
    private DialogueMan man;
    public Talky talky;
    public GameObject Player;
    public GameObject e_1;
    public TextMeshProUGUI YesNo;
    public GameObject DialogueBox;

    private void Start()
    {
        man = DialogueMan.Instance;
        e_1.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log(other.name);
        if (other.gameObject == Player)
        {
            e_1.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                DialogueMan.Instance.StartTalky(talky);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log(other.name);
        if (other.gameObject == Player)
        {
            e_1.SetActive(false);
        }
    }
}