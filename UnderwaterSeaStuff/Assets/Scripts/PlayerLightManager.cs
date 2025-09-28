<<<<<<< Updated upstream
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class PlayerLightManager : MonoBehaviour
{
    [SerializeField] private Transform m_SpriteMask;
    [SerializeField] private float m_ScalingRate;
    [SerializeField] private float m_SinPatternRate;
    [SerializeField] private float m_SinPatternMagnitude;
    [SerializeField] private float m_RotationSpeed;
    [SerializeField] private Light2D m_Light;

    private bool m_Lit = false;
    private float m_OriginalLightIntensity;
    private float m_TargetLightIntensity;
    private float m_TargetScale;

    private void Start()
    {
        m_OriginalLightIntensity = m_Light.intensity;
        Darken();
    }

    private void Update()
    {

        float target = m_TargetScale;
        if(m_Lit)
            target = m_TargetScale + Mathf.Sin(Time.time * m_SinPatternRate) * m_SinPatternMagnitude; 
        float thisFrameScale = (target - m_SpriteMask.localScale.x) * m_ScalingRate * Time.deltaTime;
        m_SpriteMask.localScale += new Vector3(thisFrameScale, thisFrameScale, 0);

        m_SpriteMask.rotation = Quaternion.Euler(0, 0, Time.time * m_RotationSpeed);

        m_Light.intensity += (m_TargetLightIntensity - m_Light.intensity) * m_ScalingRate * Time.deltaTime;
    }

    public bool Lit() { return m_Lit; }

    public void Lighten(float scale)
    {
        m_TargetLightIntensity = 0;
        m_TargetScale = scale;
        m_Lit = true;
    }

    public void Darken()
    {
        m_TargetLightIntensity = m_OriginalLightIntensity;
        m_TargetScale = 0;
        m_Lit = false;
    }
}
=======
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class PlayerLightManager : MonoBehaviour
{
    [SerializeField] private Transform m_SpriteMask;
    [SerializeField] private float m_ScalingRate;
    [SerializeField] private float m_SinPatternRate;
    [SerializeField] private float m_SinPatternMagnitude;
    [SerializeField] private float m_RotationSpeed;
    [SerializeField] private Light2D m_Light;

    private bool m_Lit = false;
    private float m_OriginalLightIntensity;
    private float m_TargetLightIntensity;
    private float m_TargetScale;

    private void Start()
    {
        m_OriginalLightIntensity = m_Light.intensity;
        Darken();
    }

    private void Update()
    {

        float target = m_TargetScale;
        if(m_Lit)
            target = m_TargetScale + Mathf.Sin(Time.time * m_SinPatternRate) * m_SinPatternMagnitude; 
        float thisFrameScale = (target - m_SpriteMask.localScale.x) * m_ScalingRate * Time.deltaTime;
        m_SpriteMask.localScale += new Vector3(thisFrameScale, thisFrameScale, 0);

        m_SpriteMask.rotation = Quaternion.Euler(0, 0, Time.time * m_RotationSpeed);

        m_Light.intensity += (m_TargetLightIntensity - m_Light.intensity) * m_ScalingRate * Time.deltaTime;
    }

    public bool Lit() { return m_Lit; }

    public void Lighten(float scale)
    {
        m_TargetLightIntensity = 0;
        m_TargetScale = scale;
        m_Lit = true;
    }

    public void Darken()
    {
        m_TargetLightIntensity = m_OriginalLightIntensity;
        m_TargetScale = 0;
        m_Lit = false;
    }
}
>>>>>>> Stashed changes
