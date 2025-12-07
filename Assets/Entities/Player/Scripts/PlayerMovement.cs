using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PortalTraveller {

    [Header("Health")]
    public HealthSystem healthSystem; // Reference to the Health Bar UI
    public int maxHealth = 100;

    [Header("Stamina Integration")]
    public StaminaSystem staminaSystem; // Reference to the Stamina Bar UI
    public float sprintCostPerSecond = 10f;

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float smoothMoveTime = 0.1f;
    public float jumpForce;
    public float gravity;

    [Header("Look")]
    public float mouseSensitivity;
    public Vector2 pitchMinMax = new Vector2 (-40, 85);
    public float rotationSmoothTime = 0.1f;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    public float startYScale;
    private float targetYScale;
    public float crouchTransition;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl; 

    public Animator animator;

    private CharacterController controller;
    Camera cam;
    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;

    float yawSmoothV;
    float pitchSmoothV;
    float verticalVelocity;
    Vector3 velocity;
    Vector3 smoothV;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    public bool jumping;
    public bool isCrouching;
    float lastGroundedTime;

    private bool isPaused = false;
    private Vector3 currentVelocity;

    // -- STAMINA VAR --
    private bool isSprinting; // Track sprinting status for movement and stamina logic

    void Start () {
        cam = Camera.main;

        controller = GetComponent<CharacterController>();

        controller.height = startYScale;
        targetYScale = startYScale;

        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize Health
        if (healthSystem != null)
        {
            healthSystem.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogError("HealthSystem is not assigned in the Inspector!", this.gameObject);
        }

        // Initialize Stamina
        if (staminaSystem != null)
        {
            staminaSystem.SetMaxStamina(100f); 
        }
    }

    void Update() {
        
        if (Input.GetKeyDown(KeyCode.Escape) && !IsJumping())
        {
            TogglePause();
        }

        if (isPaused) return;

        // --- HEALTH TEST KEYS ---
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (healthSystem != null) healthSystem.TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (healthSystem != null) healthSystem.Heal(5);
        }

        // --- INPUT CALCULATION ---
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir);

        // Check if the player is actively pressing a movement key (magnitude > 0)
        // Required for Stamina logic so we don't drain stamina while standing still
        bool isAttemptingMovement = inputDir.magnitude > 0.1f;

        // --- STAMINA SPRINTING LOGIC (Integrated) ---
        // We set isSprinting to true only if the key is held initially
        isSprinting = Input.GetKey(sprintKey);
        
        if (staminaSystem != null)
        {
            // CONDITION TO DRAIN STAMINA: 
            if (isSprinting && !isCrouching && isAttemptingMovement && staminaSystem.HasStamina(0.1f)) 
            {
                staminaSystem.DrainStaminaOverTime(sprintCostPerSecond);
            }
            // CONDITION TO STOP DRAINING/START REGEN: 
            else 
            {
                staminaSystem.StopStaminaDrain(); 
                
                // Set isSprinting to false if the stamina is depleted
                // This prevents the player from moving at sprint speed if they have no stamina
                if (!staminaSystem.HasStamina(0.1f)) 
                {
                    isSprinting = false; 
                }
            }
        }

        // --- MOVEMENT SPEED APPLICATION ---
        // Modified to use the 'isSprinting' bool controlled by the stamina logic
        float currentSpeed = isCrouching ? crouchSpeed : (isSprinting && !isCrouching) ? sprintSpeed : walkSpeed;
        
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, smoothMoveTime);

        verticalVelocity -= gravity * Time.deltaTime;
        velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);

        var flags = controller.Move(velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below)
        {
            jumping = false;
            lastGroundedTime = Time.time;
            verticalVelocity = 0;
        }

        if (Input.GetKeyDown(jumpKey)) 
        {
            float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
            if (!jumping && !isCrouching && timeSinceLastTouchedGround < 0.15f)
            {
                jumping = true;
                verticalVelocity = jumpForce;
            }
        }

        if (Input.GetKeyDown(crouchKey)) {
            if (!jumping) {
                isCrouching = true;
                targetYScale = crouchYScale;
            }
        }
        if (Input.GetKeyUp(crouchKey))
        {
            if (CanStand())
            {
                isCrouching = false;
                targetYScale = startYScale;
            }
        }

        smoothHeightTransition();

        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        float mMag = Mathf.Sqrt(mX * mX + mY * mY);
        if (mMag > 5)
        {
            mX = 0;
            mY = 0;
        }

        yaw += mX * mouseSensitivity;
        pitch -= mY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

        transform.eulerAngles = Vector3.up * smoothYaw;
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;

        bool isMoving = inputDir.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving && !isSprinting);
        animator.SetBool("isSprinting", isMoving && isSprinting);

    }

    void smoothHeightTransition() {
        float newHeight = Mathf.MoveTowards(controller.height, targetYScale, Time.deltaTime * crouchTransition);

        float centerAdjust = (controller.height - newHeight) / 2f;
        transform.position -= new Vector3(0, centerAdjust, 0);

        controller.height = newHeight;
    }
    
    bool CanStand() {
        Vector3 start = transform.position + Vector3.up * (controller.height / 2f);
        float checkDistance = startYScale - controller.height;
        return !Physics.Raycast(start, Vector3.up, checkDistance + 0.05f);
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle(smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;
        velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));
        Physics.SyncTransforms();
    }

    public bool IsJumping()
    {
        return jumping;
    }
    
    void TogglePause() {
        isPaused = !isPaused;

        SetInventoryState(isPaused);
    }

    public void SetInventoryState(bool open)
    {
        isPaused = open;

        if (open)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}