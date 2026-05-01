using UnityEngine;
using UnityEngine.InputSystem;

public class FPController : MonoBehaviour
{
    #region General Variables
    [Header("Movement & Look")]
    [SerializeField] GameObject camHolder;
    [SerializeField] float speed = 5f;
    [SerializeField] float crouchSpeed = 3f;
    [SerializeField] float sprintSpeed = 8f;
    [SerializeField] float maxForce = 1f;
    [SerializeField] float sensitivity = 0.1f;

    [Header("Jump & GroundCheck")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.3f;
    [SerializeField] LayerMask groundLayer;

    [Header("Player State Bools")]
    [SerializeField] bool isSprinting;
    [SerializeField] bool isCrouching;
    #endregion

    #region Sprint Stamina
    [Header("Sprint Stamina")]
    [SerializeField] float maxStamina = 5f;
    [SerializeField] float currentStamina;
    [SerializeField] float staminaDrain = 1f;
    [SerializeField] float staminaRecovery = 1.5f;
    [SerializeField] float sprintCooldown = 1f;

    bool canSprint = true;
    float sprintCooldownTimer;
    #endregion

    #region Aim Settings
    [Header("Aim Settings")]
    [SerializeField] Camera playerCamera;
    [SerializeField] float normalFOV = 60f;
    [SerializeField] float aimFOV = 40f;
    [SerializeField] float aimSpeed = 10f;

    bool isAiming;
    #endregion

    Rigidbody rb;
    Animator anim;

    Vector2 moveInput;
    Vector2 lookInput;
    float lookRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = maxStamina;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        HandleStamina();
        HandleAim();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    void CameraLook()
    {
        transform.Rotate(Vector3.up * lookInput.x * sensitivity);

        lookRotation += (-lookInput.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);

        camHolder.transform.localEulerAngles = new Vector3(lookRotation, 0f, 0f);
    }

    void Movement()
    {
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 targetVelocity = new Vector3(moveInput.x, 0, moveInput.y);

        targetVelocity *= isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : speed;
        targetVelocity = transform.TransformDirection(targetVelocity);

        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);
        velocityChange = Vector3.ClampMagnitude(velocityChange, maxForce);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Jump()
    {
        if (isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void HandleStamina()
    {
        if (isSprinting && moveInput.magnitude > 0.1f && canSprint)
        {
            currentStamina -= staminaDrain * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isSprinting = false;
                canSprint = false;
                sprintCooldownTimer = sprintCooldown;
            }
        }
        else
        {
            currentStamina += staminaRecovery * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        if (!canSprint)
        {
            sprintCooldownTimer -= Time.deltaTime;
            if (sprintCooldownTimer <= 0)
            {
                canSprint = true;
            }
        }
    }

    void HandleAim()
    {
        float targetFOV = isAiming ? aimFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * aimSpeed);
    }

    #region Input Methods
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            Jump();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = !isCrouching;
            anim.SetBool("isCrouching", isCrouching);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && !isCrouching && canSprint && currentStamina > 0)
            isSprinting = true;

        if (context.canceled)
            isSprinting = false;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
            isAiming = true;

        if (context.canceled)
            isAiming = false;
    }
    #endregion
}
