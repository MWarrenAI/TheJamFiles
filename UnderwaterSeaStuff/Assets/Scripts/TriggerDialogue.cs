using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialog : MonoBehaviour
{
    public Talky talky;

    private void Start()
    {
        StartTalky();
    }

    public void StartTalky()
    {
        if (DialogueMan.Instance == null)
        {
            Debug.LogWarning("Dialogue man asset is missing!");
            return;
        }

        if (talky == null)
        {
            Debug.LogWarning("Talky asset is missing!");
            return;
        }

        DialogueMan.Instance.StartTalky(talky);
    }
}