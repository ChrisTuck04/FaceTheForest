using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float sprintMultiplier = 2.0f;

    public float groundDrag;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 30f;
    public float staminaRegenRate = 15f;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    bool isSprinting;
    bool isCrouching;
    public FollowPlayer cameraScript; 
    public HidePlayer hidePlayerScript;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        HandleStamina();

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey) && grounded)
        {
            isCrouching = !isCrouching;
            cameraScript.SetCrouching();
            hidePlayerScript.SetCrouching();

            if (isCrouching)
            {
                moveSpeed /= 2;
            }
            else
            {
                moveSpeed *= 2;
            }
        }
    }

    private void HandleStamina()
    {
        bool isMoving = horizontalInput != 0 || verticalInput != 0;
        bool wantsToSprint = Input.GetKey(sprintKey);

        if (wantsToSprint && isMoving && currentStamina > 0)
        {
            isSprinting = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0);
        }
        else
        {
            isSprinting = false;
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        if (grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        float maxSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}