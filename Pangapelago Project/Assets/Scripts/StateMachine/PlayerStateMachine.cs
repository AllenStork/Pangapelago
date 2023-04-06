using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    // declare reference variables
    CharacterController characterController;
    Animator animator;
    PlayerInput playerInput;

    // variables to store optimized setter/getter parameter IDs
    int isWalkingHash;
    int isRunningHash;

    // variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;

    // constants
    public float rotationFactorPerFrame = 15.0f;
    public float runMultiplier = 100f;
    public float walkMultiplier = 50f;


    // gravity variables
    public float gravity = -9.8f;
    public float groundedGravity = -.05f;

    // jumping variables
    bool isJumpPressed = false;
    float initialJumpVelocity;
    public float maxJumpHeight = 40.0f;
    public float maxJumpTime = 0.75f;
    bool isJumping = false;
    int isJumpingHash;
    bool requireNewJumpPress = false;

    // state variables
    PlayerBaseState currentState;
    PlayerStateFactory states;

    // getters and setters
    public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; }}
    public Animator Animator { get { return animator; }}
    public CharacterController CharacterController { get { return characterController; }}
    public int InitialJumpVelocity { get { return (int)initialJumpVelocity; }} //might be a problem having (int)
    public int Gravity { get { return (int)gravity; }} //might be a problem having (int)
    public int IsWalkingHash { get { return isWalkingHash; }}
    public int IsRunningHash { get { return isRunningHash; }}
    public int IsJumpingHash { get { return isJumpingHash; }}
    public bool IsMovementPressed { get { return isMovementPressed; }}
    public bool IsRunPressed { get { return isRunPressed; }}
    public bool RequireNewJumpPress { get { return requireNewJumpPress; } set { requireNewJumpPress = value; }}
    public bool IsJumping { set { isJumping = value; }}
    public bool IsJumpPressed { get { return isJumpPressed; }}
    public float GroundedGravity { get { return groundedGravity; }}
    public float CurrentMovementY { get { return currentMovement.y; } set { currentMovement.y = value; }}
    public float CurrentRunMovementY { get { return currentRunMovement.y; } set { currentRunMovement.y = value; }}
    public float CurrentMovementX { get { return currentMovement.x; } set { currentMovement.x = value; }}
    public float CurrentMovementZ { get { return currentMovement.z; } set { currentMovement.z = value; }}
    public float RunMultiplier { get { return runMultiplier; }}
    public Vector2 CurrentMovementInput { get { return currentMovementInput; }}

    void Awake()
    {
        // initially set reference variables
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // setup state
        states = new PlayerStateFactory(this);
        currentState = states.Grounded();
        currentState.EnterState();

        // set the parameter hash references
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");

        // keyboard input dectection on press move
        playerInput.CharacterControls.Movement.started += onMovementInput;
        // keyboard input detection on release move
        playerInput.CharacterControls.Movement.canceled += onMovementInput;
        // controller input detection move
        playerInput.CharacterControls.Movement.performed += onMovementInput;
        // keyboard input dectection on press run
        playerInput.CharacterControls.Run.started += onRun;
        // keyboard input dectection on release run
        playerInput.CharacterControls.Run.canceled += onRun;
        // keyboard input dectection on press run
        playerInput.CharacterControls.Jump.started += onJump;
        // keyboard input dectection on release run
        playerInput.CharacterControls.Jump.canceled += onJump;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        currentState.UpdateStates();
        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        // the change in position our character should point to
        positionToLookAt.x = currentMovement.x; //cameraRelativeMovement.x
        positionToLookAt.y = 0f;
        positionToLookAt.z = currentMovement.z; // cameraRelativeMovement.z
        // the current rotation of our character
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            // creates a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        // controller.Move(move * speed * Time.deltaTime);
        currentMovement.x = currentMovementInput.x * walkMultiplier * Time.deltaTime;
        currentMovement.z = currentMovementInput.y * walkMultiplier * Time.deltaTime;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        requireNewJumpPress = false;
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        // enable the character controls action map
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

}
