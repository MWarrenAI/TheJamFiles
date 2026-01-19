using UnityEngine;
using TMPro;
using System.Collections;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float txtSpeed;
    private int index;
    private void Start()
    {
        textComponent.text = string.Empty;
        StartDialog();
    }
    private void Update()
    {
        
    }

    void StartDialog()
    {
        index = 0;
        StartCoroutine(TypeLine());

    }    
    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(txtSpeed);
        }
    }
}
