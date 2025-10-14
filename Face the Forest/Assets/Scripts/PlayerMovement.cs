using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    float walkSpeed, crouchSpeed, sprintSpeed;
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
    public Transform groundCheck;
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
    GameManager gameManagerScript;

    [Header("Audio")]
    public PlayerAudio playerAudio;

    private void Start()
    {
        walkSpeed = moveSpeed;
        crouchSpeed = moveSpeed / 2;
        sprintSpeed = moveSpeed * sprintMultiplier;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentStamina = maxStamina;
        gameManagerScript = GameManager.instance;

        if (playerAudio == null)
        {
            playerAudio = GetComponent<PlayerAudio>();
        }
    }

    private void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        HandleStamina();

        if (playerAudio != null)
        {
            bool isMoving = horizontalInput != 0 || verticalInput != 0;
            playerAudio.PlayFootsteps(isMoving, isSprinting, grounded, isCrouching);
            playerAudio.CheckLanding(grounded);
        }

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
            gameManagerScript.SetCrouching();

            if (isCrouching)
            {
                moveSpeed = crouchSpeed;
            }
            else
            {
                moveSpeed = walkSpeed;
            }
        }
    }

    private void HandleStamina()
    {
        bool isMoving = horizontalInput != 0 || verticalInput != 0;
        bool wantsToSprint = Input.GetKey(sprintKey);

        if (wantsToSprint && isMoving && currentStamina > 0 && !isCrouching)
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
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        float maxSpeed = isSprinting ? sprintSpeed : moveSpeed;

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

        if (playerAudio != null)
        {
            playerAudio.PlayJumpSound();
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public void Die()
    {
        this.enabled = false;

        rb.isKinematic = true;
    }

    public void HinderedCheck(bool hindered)
    {
        if (hindered)
        {
            moveSpeed = isCrouching ? crouchSpeed - 2 : isSprinting ? sprintSpeed - 2 : walkSpeed - 2; //lol
        }
    }
}