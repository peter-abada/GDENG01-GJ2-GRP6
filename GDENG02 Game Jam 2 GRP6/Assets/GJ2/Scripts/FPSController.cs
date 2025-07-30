using UnityEngine;
using UnityEngine.InputSystem;
using PlayerFPSControl; //  this is your generated namespace

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float topDownMoveSpeed = 2f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    
    public Transform mainCameraTransform;
    public Camera fpsCamera;
    public Camera topDownCamera;
    private bool isTopDown = false;
    private bool toggleCamPressed;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    // The generated input actions class
    //private PlayerFPSControl playerInput;

    private PlayerFPSControl.PlayerFPSControl playerInput;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private ParticleSystem afterburner;  // assign in inspector
    private float afterburnerTimer = 0f;

    private bool isDashing = false;
    private float dashTime = 0f;
    private float dashCooldownTimer = 0f;

    [Header("Footstep Settings")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip runningClip;
    [SerializeField] private AudioClip walkingClip;
    [SerializeField] private float footstepInterval = 0.4f;

    private float footstepTimer = 0f;

    [Header("Dash Sound")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip dashSound;


    [SerializeField]private Animator animator;

    private void Awake()
    {

        controller = GetComponent<CharacterController>();
        
        //playerInput = new PlayerFPSControl();
        playerInput = new PlayerFPSControl.PlayerFPSControl();
        // Hook up callbacks
        // Assuming in your .inputactions you have a "Player" action map with these actions:
        // Replace "Player/Move" with your real action names
        //playerInput.FindAction("Move").performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        //playerInput.FindAction("Move").canceled += ctx => moveInput = Vector2.zero;

        //playerInput.FindAction("Look").performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        //playerInput.FindAction("Look").canceled += ctx => lookInput = Vector2.zero;

        playerInput.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        playerInput.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerInput.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        playerInput.Player.ToggleCam.performed += ctx => toggleCamPressed = true;
        playerInput.Player.Dash.performed += ctx => TryDash();

        //animator = GetComponent<Animator>();
        //  playerInput.FindAction("Jump").performed += ctx => jumpPressed = true;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        fpsCamera.enabled = true;
        topDownCamera.enabled = false;
        controller = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        afterburner.Stop();
    }

    void Update()
    {
        Debug.Log("Move Input: " + moveInput);
        if (toggleCamPressed)
        {
            toggleCamPressed = false; // reset
            isTopDown = !isTopDown;
            ToggleCamera();
        }
        HandleMovement();
        HandleMouseLook();
        HandleFootsteps();

        if (afterburnerTimer > 0f)
        {
            afterburnerTimer -= Time.deltaTime;
            if (afterburnerTimer <= 0f && afterburner != null)
            {
                afterburner.Stop();
            }
        }
    }

    void HandleMovement()
    {
        float currentSpeed = isTopDown ? topDownMoveSpeed : moveSpeed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        //controller.Move(move * moveSpeed * Time.deltaTime);

        if (isDashing)
        {
            // Dash logic
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            dashTime -= Time.deltaTime;
            if (dashTime <= 0f)
            {
                isDashing = false;
            }
        }
        else
        {
            // Normal movement
            controller.Move(move * currentSpeed * Time.deltaTime);

            if (dashCooldownTimer > 0f)
                dashCooldownTimer -= Time.deltaTime;
        }

        if (velocity.y < 0)
            velocity.y = -2f;

        if (jumpPressed)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Blend Tree
        //Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        //bool isMoving = horizontalVelocity.magnitude > 0.1f;
        //bool isMoving = moveInput.sqrMagnitude > 0.01f;
        //Debug.Log("isMoving: " + isMoving);
        //animator.SetBool("isWalking", isMoving);

        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);
    }

    void TryDash()
    {
        if (!isDashing && dashCooldownTimer <= 0f)
        {
            isDashing = true;
            dashTime = dashDuration;
            dashCooldownTimer = dashCooldown; // start cooldown
            if (dashSound != null && sfxAudioSource != null)
            {
                sfxAudioSource.PlayOneShot(dashSound);
            }
            else
            {
                Debug.LogWarning("Dash sound or sfxAudioSource not assigned!");
            }
            if (afterburner != null)
            {
                afterburner.Play();
                afterburnerTimer = 0.5f;
            }
        }
    }

    void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        mainCameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void ToggleCamera()
    {
        if (isTopDown)
        {
            fpsCamera.enabled = false;
            topDownCamera.enabled = true;

            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            //isTopDown = false;
        }
        else
        {
            fpsCamera.enabled = true;
            topDownCamera.enabled = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //isTopDown = true;
        }
    }

    void HandleFootsteps()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f && controller.isGrounded && !isDashing;

        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                // Pick appropriate sound
                AudioClip clipToPlay = isTopDown ? walkingClip : runningClip;

                if (footstepAudioSource.clip != clipToPlay)
                {
                    footstepAudioSource.clip = clipToPlay;
                }

                footstepAudioSource.Play();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepAudioSource.Stop();
            footstepTimer = 0f;
        }
    }
}
