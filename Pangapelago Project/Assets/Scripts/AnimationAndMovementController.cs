using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    // declare reference variables
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    // variables to store optimized setter/getter parameter IDs
    int isWalkingHash;
 //   int isRunningHash;

    // variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 cameraRelativeMovement;
    bool isMovementPressed;
 //   bool isRunPressed;
    //float runMultiplier = 100f;

    // constants
    float rotationFactorPerFrame = 15f;
    float PlayerSpeed;
    int zero = 0;

    float gravity = -10f;
    float groundedGravity = -.05f;

    //jumping variables
    bool isJumpPressed = false;
    float initialJumpVelocity;
    float maxJumpHeight = 1f;
    float maxJumpTime = 0.75f;
    bool isJumping = false;
    int isJumpingHash;
    bool isJumpAnimating = false;


    // Awake is called earlier than Start in Unity's event life cycle
    void Awake()
    {
        // initially set reference variables
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
 //       isRunningHash = Animator.StringToHash("isRunning");
 //       isJumpingHash = Animator.StringToHash("isJumping");
        PlayerSpeed = 50f;
       
        // keyboard input dectection on press move
        playerInput.CharacterControls.Movement.started += onMovementInput;
        // keyboard input detection on release move
        playerInput.CharacterControls.Movement.canceled += onMovementInput;
        // controller input detection move
        playerInput.CharacterControls.Movement.performed += onMovementInput;
        // keyboard input dectection on press run
  //      playerInput.CharacterControls.Run.started += onRun;
        // keyboard input dectection on release run
  //      playerInput.CharacterControls.Run.canceled += onRun;

        // keyboard input dectection on press jump
        playerInput.CharacterControls.Jump.started += onJump;
        // keyboard input detection on release jump
        playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();
    }
        
    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            currentMovement.y = initialJumpVelocity * .5f;
            Debug.Log("Jump");
        }
        else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    void onJump (InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

 //   void onRun (InputAction.CallbackContext context)
  //  {
 //       isRunPressed = context.ReadValueAsButton();
  //  }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        // the change in position our character should point to
        positionToLookAt.x = cameraRelativeMovement.x;
        positionToLookAt.y = zero;
        positionToLookAt.z = cameraRelativeMovement.z;
        // the current rotation of our character
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            // creates a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

    }

    void onMovementInput (InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
 //       currentRunMovement.x = currentMovementInput.x * 100.0f;
 //       currentRunMovement.z = currentMovementInput.y * 100.0f;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        Debug.Log("Move");
    }

    void handleAnimation()
    {
        // get parameter values from animator
        bool isWalking = animator.GetBool("isWalkingHash");
 //       bool isRunning = animator.GetBool("isRunningHash");

        // start walking if movement pressed is true and not already walking
        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        // stop walking if isMovementPressed is false and not already walking
        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }
        // run if movement and run pressed are true and not currently running
 //       if ((isMovementPressed && isRunPressed) && !isRunning)
 //       {
 //           animator.SetBool(isRunningHash, true);
 //       }
        // stop running if movement or run pressed are false and currently running
 //       else if ((!isMovementPressed || !isRunPressed) && isRunning)
 //       {
 //           animator.SetBool(isRunningHash, false);
 //       }
    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;
        // apply proper gravity depending on if the character is grounded or not
        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }
            animator.SetBool("isJumping", false);
            currentMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) * .5f, -20.0f);
            currentMovement.y = nextYVelocity;
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            currentMovement.y = nextYVelocity;
        }
    }


    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();

  //      if (isRunPressed)
 //       {
 //           characterController.Move(currentRunMovement * Time.deltaTime);
  //      }
  //      else
  //      {
            // transform position using Move and the rotated player input
  //          characterController.Move(cameraRelativeMovement * Time.deltaTime * PlayerSpeed);
            // rotate player input vector to camera space
  //          cameraRelativeMovement = ConvertToCameraSpace(currentMovement);
    //    }

        handleGravity();
        handleJump();
        // transform position using Move and the rotated player input
              characterController.Move(cameraRelativeMovement * Time.deltaTime * PlayerSpeed);
        // rotate player input vector to camera space
              cameraRelativeMovement = ConvertToCameraSpace(currentMovement);

    }

    private void OnEnable()
    {
        // enable the character controls action map
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        // disable the character controls actions map
        playerInput.CharacterControls.Disable();
    }

    // handles rotation based on camera
    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        // store the Y value of the original vector to rotate
        float currentYValue = vectorToRotate.y;

        // get the forward and right directional vectors of the camera
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // remove the Y values to ignore upward/downward camera angles
        cameraForward.y = 0;
        cameraRight.y = 0;

        // re-normalize both vectors so they each have a magnitude of 1
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        // rotate the X and Z VectorToRotate values to camera space
        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

        // the sum of both products is the Vector3 in camera space and set Y value
        Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotatedToCameraSpace.y = currentYValue;
        return vectorRotatedToCameraSpace;
    }

}
