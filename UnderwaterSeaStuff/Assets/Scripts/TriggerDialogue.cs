using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialog : MonoBehaviour
{
    public Talky talky;

    void Start() {
        [System.Obsolete]
        void TriggerTalk ()
            {
                FindObjectOfType<DialogueMan>().StartTalky(talky);
            }
    }       
}