using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;
    public InputActionReference crouchAction;

    [Header("References")]
    public Transform cameraTransform;

    public bool allowMovement = true;
    public bool allowLook = true;

    [Header("Movement")]
    public float walkSpeed = 7f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 3.5f;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float gravity = -13f;
    public float jumpHeight = 1.6f;

    [Header("Look")]
    public float lookSensitivity = 1f;
    public float maxLookAngle = 80f;

    [Header("Cursor")]
    public bool lockCursor = true;

    [Header("Crouch")]
    [Tooltip("Multiplier applied to the CharacterController height when crouching (0-1).")]
    public float crouchHeightMultiplier = 0.5f;
    [Tooltip("How fast to interpolate height and camera when toggling crouch.")]
    public float crouchTransitionSpeed = 6f;

    private CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float cameraPitch;
    private float standHeight;
    private float crouchHeight;
    private float standCameraY;
    private float crouchCameraY;
    [Header("Camera Bob")]
    [Tooltip("Vertical amplitude of the head-bob in meters.")]
    public float bobAmplitude = 0.03f;
    [Tooltip("Base frequency of the head-bob (Hz-ish).")]
    public float bobFrequency = 8f;
    [Tooltip("Tilt amount applied to the camera while bobbing (degrees).")]
    public float bobSwayAngle = 1.2f;
    [Tooltip("How quickly the bob blends (higher = snappier).")]
    public float bobSmooth = 8f;
    [Tooltip("Minimum movement speed (m/s) to start bobbing.")]
    public float bobStartSpeed = 0.1f;
    [Tooltip("Amplitude multiplier applied while sprinting.")]
    public float bobSprintMultiplier = 1.6f;
    [Tooltip("Amplitude multiplier applied while crouching/sneaking.")]
    public float bobCrouchMultiplier = 0.45f;
    [Tooltip("Frequency multiplier applied while sprinting.")]
    public float bobSprintFreqMultiplier = 1.25f;

    private float bobTimer = 0f;
    private Vector3 cameraInitialLocalPos;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        if (cameraTransform != null) cameraInitialLocalPos = cameraTransform.localPosition;
        cursorLocked = lockCursor;
        SetCursorState(cursorLocked);
        // Initialize crouch heights
        standHeight = controller.height;
        crouchHeight = Mathf.Max(0.1f, standHeight * crouchHeightMultiplier);
        if (cameraTransform != null)
        {
            standCameraY = cameraTransform.localPosition.y;
            crouchCameraY = standCameraY * crouchHeightMultiplier;
        }
    }

    void OnEnable()
    {
        moveAction.action?.Enable();
        lookAction.action?.Enable();
        jumpAction.action?.Enable();
        sprintAction.action?.Enable();
        crouchAction.action?.Enable();
    }

    void OnDisable()
    {
        moveAction.action?.Disable();
        lookAction.action?.Disable();
        jumpAction.action?.Disable();
        sprintAction.action?.Disable();
        crouchAction.action?.Disable();
        // Restore cursor when disabled
        SetCursorState(false);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        Vector2 input = Vector2.zero;
        if (allowMovement && moveAction != null && moveAction.action != null) input = moveAction.action.ReadValue<Vector2>();

        Vector2 look = Vector2.zero;
        if (allowLook && lookAction != null && lookAction.action != null) look = lookAction.action.ReadValue<Vector2>();

        bool sprint = sprintAction != null && sprintAction.action != null && sprintAction.action.IsPressed();
        bool crouch = crouchAction != null && crouchAction.action != null && crouchAction.action.IsPressed();
        bool jumpPressed = jumpAction != null && jumpAction.action != null && jumpAction.action.triggered;

        float targetSpeed = 0f;
        if (input.sqrMagnitude > 0.0001f)
        {
            if (crouch) targetSpeed = crouchSpeed;
            else targetSpeed = sprint ? sprintSpeed : walkSpeed;
        }

        Vector3 moveDirection = Vector3.zero;
        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f; right.y = 0f;
            forward.Normalize(); right.Normalize();
            moveDirection = (right * input.x + forward * input.y);
            if (moveDirection.sqrMagnitude > 1f) moveDirection.Normalize();
        }
        else
        {
            moveDirection = (transform.right * input.x + transform.forward * input.y);
            if (moveDirection.sqrMagnitude > 1f) moveDirection.Normalize();
        }

        Vector3 targetVelocity = moveDirection * targetSpeed;

        float accel = (targetSpeed > horizontalVelocity.magnitude) ? acceleration : deceleration;
        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, accel * dt);

        // Smoothly interpolate character height and camera when crouching/standing
        if (controller != null)
        {
            float targetHeight = crouch ? crouchHeight : standHeight;
            if (Mathf.Abs(controller.height - targetHeight) > 0.001f)
            {
                controller.height = Mathf.MoveTowards(controller.height, targetHeight, crouchTransitionSpeed * dt);
                controller.center = new Vector3(0f, controller.height * 0.5f, 0f);
                if (cameraTransform != null)
                {
                    Vector3 camLocal = cameraTransform.localPosition;
                    float targetCamY = crouch ? crouchCameraY : standCameraY;
                    camLocal.y = Mathf.MoveTowards(camLocal.y, targetCamY, crouchTransitionSpeed * dt);
                    cameraTransform.localPosition = camLocal;
                }
            }
        }

        bool grounded = controller.isGrounded;
        if (grounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = -2f; // small downward to keep grounded
            if (jumpPressed)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        verticalVelocity += gravity * dt;

        Vector3 finalMove = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(finalMove * dt);

        // Look
        if (look.sqrMagnitude > 0.00001f)
        {
            float yaw = look.x * lookSensitivity;
            float pitch = look.y * lookSensitivity;

            transform.Rotate(Vector3.up, yaw * dt, Space.World);

            cameraPitch -= pitch * dt;
            cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
            if (cameraTransform != null)
            {
                cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
            }
        }

        // Camera head-bob: vertical bob + slight roll while moving (attenuated while airborne)
        if (cameraTransform != null)
        {
            // Use initial x/z so bob doesn't drift; use current local Y (affected by crouch)
            Vector3 baseCamLocal = new Vector3(cameraInitialLocalPos.x, cameraTransform.localPosition.y, cameraInitialLocalPos.z);
            float horizSpeed = new Vector3(horizontalVelocity.x, 0f, horizontalVelocity.z).magnitude;

            bool shouldBob = horizSpeed > bobStartSpeed;
            float airFactor = 1f;
            if (!grounded)
            {
                // When airborne, reduce bob proportionally to fall speed (so running down a steep slope still bobs)
                float fallSpeed = Mathf.Abs(Mathf.Min(0f, verticalVelocity));
                airFactor = Mathf.Clamp01(1f - (fallSpeed / 20f));
            }

            if (shouldBob && airFactor > 0.01f)
            {
                float speedFactor = Mathf.Clamp01(horizSpeed / Mathf.Max(0.0001f, walkSpeed));
                // adjust intensity based on sprint / crouch
                float amplitudeMult = 1f;
                float freqMult = 1f;
                if (sprint) amplitudeMult *= bobSprintMultiplier;
                if (crouch) amplitudeMult *= bobCrouchMultiplier;
                if (sprint) freqMult *= bobSprintFreqMultiplier;

                bobTimer += dt * bobFrequency * freqMult * (0.5f + speedFactor) * (0.5f + airFactor * 0.5f);
                float bobY = Mathf.Sin(bobTimer) * bobAmplitude * (0.5f + speedFactor) * airFactor * amplitudeMult;
                float roll = Mathf.Sin(bobTimer * 0.5f) * bobSwayAngle * speedFactor * airFactor * amplitudeMult;

                Vector3 targetPos = new Vector3(baseCamLocal.x, baseCamLocal.y + bobY, baseCamLocal.z);
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, bobSmooth * dt);

                cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, roll);
            }
            else
            {
                // Smoothly return to neutral camera position/rotation
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, baseCamLocal, bobSmooth * dt);
                cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
                // gently reset timer to avoid jump when landing
                bobTimer = Mathf.Lerp(bobTimer, 0f, bobSmooth * dt);
            }
        }

        // Toggle cursor lock with Escape
        var kb = Keyboard.current;
        if (kb != null && kb.escapeKey.wasPressedThisFrame)
        {
            cursorLocked = !cursorLocked;
            SetCursorState(cursorLocked);
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && lockCursor)
        {
            SetCursorState(cursorLocked);
        }
        else if (!hasFocus)
        {
            SetCursorState(false);
        }
    }

    private bool cursorLocked;
    private void SetCursorState(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        cursorLocked = locked;
    }

    private InputActionReference CreateTempFromValue(InputActionReference existing, InputValue value)
    {
        // If the user is using PlayerInput callbacks without assigning actions to the inspector,
        // we don't create persistent actions here. This method is a noop placeholder to keep
        // compilation safe. Prefer assigning InputActionReferences in the inspector.
        return existing;
    }
}
