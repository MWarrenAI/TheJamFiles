using NUnit.Framework;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerLightManager))]
[RequireComponent(typeof(InsanityController))]

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D m_RigidBody;
    private PlayerLightManager m_PlayerLightManager;
    private InsanityController m_InsanityController;


    [Header("Movement")]
    [SerializeField] private float m_Acceleration;
    [SerializeField] private float m_Deceleration;
    [SerializeField] private float m_MovementSpeed;
    [SerializeField] private float m_JumpForce;
    [SerializeField] private float m_JumpCoolDown;
    [SerializeField] private float m_DownwardsJumpForce;
    [SerializeField] private float m_FloorCheckRange;
    [SerializeField] private Transform m_FloorCheckTransform;
    [SerializeField] private Transform[] m_PlayerSprites;

    [Header("Lighting Management")]
    [SerializeField] private float m_SwitchLightCooldown;
    [SerializeField] private float m_LightScale;

    [Header("Camera Movement")]
    [SerializeField] private Vector3 m_CameraOffset;
    [SerializeField] private float m_CameraFollowSmoothness;

    [Header("Insane Movement")]
    [SerializeField] private float m_SlowestMovementSpeed;
    [SerializeField] private float m_SlowestAcceleration;
    [SerializeField] private float m_WeakestJump;
    [SerializeField] private float m_RateOfSlowJump;

    [Header("Interactions")]
    [SerializeField] private float m_InteractionRange;
    [SerializeField] private Transform m_InteractionCheck;


    private InputAction m_MoveAction;
    private InputAction m_JumpAction;
    private InputAction m_SwitchAction;
    private InputAction m_InteractionAction;
    
    private bool m_HoldingDown = false;
    private bool m_NeedToJump = false;

    private float m_LastJumpTime = 0;
    private float m_LastSwitchTime = 0;

    private Vector2 m_DefaultSpriteScale = Vector2.zero;


    
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_PlayerLightManager = GetComponent<PlayerLightManager>();
        m_InsanityController = GetComponent<InsanityController>();

        m_DefaultSpriteScale = m_PlayerSprites[0].localScale;

        m_MoveAction = InputSystem.actions.FindAction("Move");
        m_JumpAction = InputSystem.actions.FindAction("Jump");
        m_SwitchAction = InputSystem.actions.FindAction("Interact");
        m_InteractionAction = InputSystem.actions.FindAction("Attack");
    }

    private void Update()
    {
        DetectJump();
        HandleLightSystem();
        HandleInteractions();
    }

    void FixedUpdate()
    {
        HandleLeftRight();
        HandleJump();
        HandleDown();
        CameraFollowPlayer();
    }

    void HandleLeftRight()
    {
        m_RigidBody.linearVelocityX += m_MoveAction.ReadValue<Vector2>().x * Mathf.Lerp(m_Acceleration, m_SlowestAcceleration, m_InsanityController.MappedInsanity());
        m_RigidBody.linearVelocityX = Mathf.Clamp(m_RigidBody.linearVelocityX, -Mathf.Lerp(m_MovementSpeed, m_SlowestMovementSpeed, m_InsanityController.MappedInsanity()) , Mathf.Lerp(m_MovementSpeed, m_SlowestMovementSpeed, m_InsanityController.MappedInsanity()));

        if (m_MoveAction.ReadValue<Vector2>().x == 0)
            m_RigidBody.linearVelocityX *= m_Deceleration;

        for(int i = 0; i < m_PlayerSprites.Length; i++)
        {
            if (m_RigidBody.linearVelocity.x > 0.1)
                m_PlayerSprites[i].localScale = m_DefaultSpriteScale;
            else if (m_RigidBody.linearVelocity.x < -0.1)
                m_PlayerSprites[i].localScale = new Vector2(-m_DefaultSpriteScale.x, m_DefaultSpriteScale.y);
        }
    }

    void DetectJump()
    {
        if (m_JumpAction.IsPressed() && m_NeedToJump == false && Time.time > m_LastJumpTime + Mathf.Lerp(m_JumpCoolDown, m_RateOfSlowJump, m_InsanityController.MappedInsanity()))
            m_NeedToJump = true;
        m_HoldingDown = m_MoveAction.ReadValue<Vector2>().y < 0;
    }
    void HandleJump()
    {
        if(m_NeedToJump)
        {
            m_RigidBody.linearVelocityY = Mathf.Lerp(m_JumpForce, m_WeakestJump, m_InsanityController.MappedInsanity());
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
        Vector3 targetPosition = transform.position + m_CameraOffset;
        Vector3 newCameraPosition = (targetPosition - camera.position) * m_CameraFollowSmoothness;
        camera.position += new Vector3(newCameraPosition.x, newCameraPosition.y, newCameraPosition.z);
    }

    void HandleInteractions()
    {
        if(m_InteractionAction.WasPressedThisFrame())
        {
            Debug.Log("Interaction Attempt");
            Collider2D coll = Physics2D.OverlapCircle(m_InteractionCheck.position, m_InteractionRange, LayerMask.GetMask("InteractableNPC"));
            if (coll != null)
            {

                Debug.Log("Collider Attempt");
                InteractableNPC interactable = coll.GetComponent<InteractableNPC>();
                if (interactable != null)
                {

                    Debug.Log("Interact Attempt");
                    interactable.Interact();
                }
            }
        }
    }
}
