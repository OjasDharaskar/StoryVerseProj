using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5.5f;
    [SerializeField] private float runningSpeed = 9f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;

    [Header("Camera Settings")]
    [SerializeField] private float lookSensitivity = 0.2f;
    [SerializeField] private float lookAngleLimit = 80f;

    private Camera mainCamera;
    private CharacterController characterController;

    // Input Actions
    private InputAction moveInput;
    private InputAction runInput;
    private InputAction jumpInput;

    // State Variables
    private float currentMoveSpeed;
    private Vector3 moveDirection;
    private float lookAngle;
    private bool jumped = false;

    private void Start()
    {
        // Get Components
        mainCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();

        // Find and assign Input Actions 
        // (Make sure your Input Action maps match these names: "Move", "Sprint", "Jump")
        moveInput = InputSystem.actions.FindAction("Move");
        runInput = InputSystem.actions.FindAction("Sprint");
        jumpInput = InputSystem.actions.FindAction("Jump");

        // Subscribe to jump event
        jumpInput.started += Jump;

        // Hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentMoveSpeed = walkSpeed;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        jumped = true;
    }

    private void Update()
    {
        // Reset jump flag if in the air so holding it down doesn't keep triggering jumps
        if (!characterController.isGrounded)
        {
            jumped = false;
        }

        // Sprint check using ternary operator
        currentMoveSpeed = runInput.IsPressed() ? runningSpeed : walkSpeed;

        // Handle Movement
        Vector2 moveVector = moveInput.ReadValue<Vector2>();
        HandleMovement(moveVector);

        // Handle Looking (Mouse Input)
        Vector2 mouseDelta = new Vector2(Mouse.current.delta.x.ReadValue(), Mouse.current.delta.y.ReadValue());
        HandleLooking(mouseDelta);
    }

    private void HandleMovement(Vector2 moveVector)
    {
        // Get local forward and right directions
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Save current vertical movement (for gravity/jumping)
        float oldY = moveDirection.y;

        // Calculate new horizontal movement speeds (swapping X and Y to match world space)
        Vector2 newSpeed = new Vector2(moveVector.y * currentMoveSpeed, moveVector.x * currentMoveSpeed);

        // Build the movement direction
        moveDirection = (forward * newSpeed.x) + (right * newSpeed.y);

        // Apply Jump or keep previous vertical movement
        moveDirection.y = (jumped && characterController.isGrounded) ? jumpForce : oldY;

        // Apply Gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleLooking(Vector2 mouseDelta)
    {
        // Calculate vertical looking angle and clamp it
        lookAngle += -mouseDelta.y * lookSensitivity;
        lookAngle = Mathf.Clamp(lookAngle, -lookAngleLimit, lookAngleLimit);

        // Apply vertical rotation to the camera
        mainCamera.transform.localRotation = Quaternion.Euler(lookAngle, 0, 0);

        // Apply horizontal rotation to the entire player object
        transform.rotation *= Quaternion.Euler(0, mouseDelta.x * lookSensitivity, 0);
    }
}