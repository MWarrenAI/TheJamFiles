using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InsanityController : MonoBehaviour
{
    private float m_Insanity = 0;

    bool m_SanityDirection = false;
    [SerializeField] private float m_MaxInsanity;
    [SerializeField] private float m_InsanityIncreaseRate;
    [SerializeField] private float m_InsanityDecreaseRate;

    [SerializeField] private Volume m_Volume;

    public float Insanity() { return m_Insanity; }

    // state -  true = going insane 
    //          false = sane
    public void ToggleInsanity(bool state)
    {
        m_SanityDirection = state;
    }

    private void Update()
    {
        if (m_SanityDirection)
            m_Insanity += m_InsanityIncreaseRate * Time.deltaTime;
        else
            m_Insanity -= m_InsanityDecreaseRate * Time.deltaTime;
        m_Insanity = Mathf.Clamp(m_Insanity, 0, m_MaxInsanity);

        Debug.Log(m_Insanity);

        if (m_Volume.profile.TryGet<Vignette>(out var vignette))
        {
            // Scale intensity between 0 and 1 based on insanity
            vignette.intensity.value = Mathf.Clamp01(m_Insanity / m_MaxInsanity);
        }
    }
}
