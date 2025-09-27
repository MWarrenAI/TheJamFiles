using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(PlayerLightManager))]

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D m_RigidBody;
    private PlayerLightManager m_PlayerLightManager;


    [Header("Movement")]
    [SerializeField] private float m_Acceleration;
    [SerializeField] private float m_Deceleration;
    [SerializeField] private float m_MovementSpeed;
    [SerializeField] private float m_JumpForce;
    [SerializeField] private float m_JumpCoolDown;
    [SerializeField] private float m_DownwardsJumpForce;
    [SerializeField] private float m_FloorCheckRange;
    [SerializeField] private Transform m_FloorCheckTransform;

    [Header("Lighting Management")]
    [SerializeField] private float m_SwitchLightCooldown;
    [SerializeField] private float m_LightScale;

    [Header("Camera Movement")]
    [SerializeField] private float m_CameraFollowSmoothness;

    

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;
    private InputAction m_SwitchAction;
    
    private bool m_HoldingDown = false;
    private bool m_NeedToJump = false;

    private float m_LastJumpTime = 0;
    private float m_LastSwitchTime = 0;

    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_PlayerLightManager = GetComponent<PlayerLightManager>();

        m_MoveAction = InputSystem.actions.FindAction("Move");
        m_JumpAction = InputSystem.actions.FindAction("Jump");
        m_SwitchAction = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        DetectJump();
        HandleLightSystem();
        CameraFollowPlayer();
    }

    void FixedUpdate()
    {
        HandleLeftRight();
        HandleJump();
        HandleDown();
    }

    void HandleLeftRight()
    {
        m_RigidBody.linearVelocityX += m_MoveAction.ReadValue<Vector2>().x * m_Acceleration;
        m_RigidBody.linearVelocityX = Mathf.Clamp(m_RigidBody.linearVelocityX, -m_MovementSpeed, m_MovementSpeed);
        if (m_MoveAction.ReadValue<Vector2>().x == 0)
            m_RigidBody.linearVelocityX *= m_Deceleration;
        m_RigidBody.SetRotation(m_RigidBody.linearVelocityX);
    }

    void DetectJump()
    {
        if (m_JumpAction.IsPressed() && m_NeedToJump == false && Time.time > m_LastJumpTime + m_JumpCoolDown)
            m_NeedToJump = true;
        m_HoldingDown = m_MoveAction.ReadValue<Vector2>().y < 0;
    }
    void HandleJump()
    {
        if(m_NeedToJump)
        {
            m_RigidBody.linearVelocityY = m_JumpForce;
            m_NeedToJump = false;
            m_LastJumpTime = Time.time;
        }
    }

    void HandleDown()
    {
        if (m_HoldingDown)
        {
            Collider2D coll = Physics2D.OverlapCircle(m_FloorCheckTransform.position, m_FloorCheckRange, LayerMask.GetMask("Floor"));
            if (coll == null)
            {
                m_RigidBody.linearVelocityY -= m_DownwardsJumpForce;
            }
        }
    }

    void HandleLightSystem()
    {
        if (m_SwitchAction.IsPressed() && Time.time > m_LastSwitchTime + m_SwitchLightCooldown)
        {
            if (m_PlayerLightManager.Lit())
                m_PlayerLightManager.Darken();
            else
                m_PlayerLightManager.Lighten(m_LightScale);
            m_LastSwitchTime = Time.time;
        }
    }

    void CameraFollowPlayer()
    {
        Transform camera = Camera.main.transform;
        Vector2 newCameraPosition = new Vector2(transform.position.x - camera.position.x,
            transform.position.y - camera.position.y) * m_CameraFollowSmoothness;
        camera.position += new Vector3(newCameraPosition.x, newCameraPosition.y, 0);
    }
}
