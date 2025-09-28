using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class InteractableNPC : MonoBehaviour
{
    [SerializeField] private GameObject m_TextBox;
    [SerializeField] private float m_InteractionDuration;
    [SerializeField] private float m_interactionCooldown;

    private bool m_Interacted = false;
    private float m_LastInteractionTime;
    public void Update()
    {
        if(m_Interacted && Time.time > m_LastInteractionTime + m_InteractionDuration)
        {
            m_TextBox.SetActive(false);
            m_Interacted = false;
        }
    }

    public void Interact()
    {
        if (m_Interacted == false && Time.time > m_LastInteractionTime + m_interactionCooldown )
        {
            m_LastInteractionTime = Time.time;
            m_TextBox.SetActive(true);
            m_Interacted = true;
        }
    }
}
