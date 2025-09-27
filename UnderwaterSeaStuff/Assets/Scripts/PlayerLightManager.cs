using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerLightManager : MonoBehaviour
{
    [SerializeField] private Transform m_SpriteMask;
    [SerializeField] private float m_ScalingRate;

    private bool m_Lit = false;
    private float m_TargetScale;
    private void Update()
    {
        float thisFrameScale = (m_TargetScale - m_SpriteMask.localScale.x) * m_ScalingRate * Time.deltaTime;
        m_SpriteMask.localScale += new Vector3(thisFrameScale, thisFrameScale, 0);
    }

    public bool Lit() { return m_Lit; }

    public void Lighten(float scale)
    {
        m_TargetScale = scale;
        m_Lit = true;
    }

    public void Darken()
    { 
        m_TargetScale = 0;
        m_Lit = false;
    }
}
