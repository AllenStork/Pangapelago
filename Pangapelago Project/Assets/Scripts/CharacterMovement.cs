using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    // variable to store character animator component
    Animator animator;

    // variables to store optimized setter/getter parameter IDs
    int isMovingHash;

    //variable to store the instance of the PlayerInput
    PlayerInput input;

    //variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool movementPressed;

    CharacterController characterController;
    // [SerializeField] float moveSpeed = 10f;
    // Rigidbody rb;

    private void Awake()
    {
        input = new PlayerInput();

        // set the player input values using listeners
        input.CharacterControls.Movement.performed += ctx =>
        {
            currentMovementInput = ctx.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            movementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
            
        };
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // set the animator reference
        animator = GetComponent<Animator>();

        // set the ID references
        isMovingHash = Animator.StringToHash("isMoving");

       // rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleMovement();

        characterController.Move(currentMovementInput);
        //Vector3 playerVelocity = new Vector3(currentMovement.x * moveSpeed, rb.velocity.y, currentMovement.y * moveSpeed);
    }

    void handleRotation()
    {
        // Current position of our character
        Vector3 currentPosition = transform.position;

        // the change in position our character should point to
        Vector3 newPosition = new Vector3(currentMovementInput.x, 0, currentMovementInput.y);

        // combine the positions to give a position to look at
        Vector3 positionToLookAt = currentPosition + newPosition;

        // rotate the character to face the positionToLookAt
        transform.LookAt(positionToLookAt);
    }


    void handleMovement()
    {
        //get parameter values from animator
        bool isMoving = animator.GetBool(isMovingHash);

        if (movementPressed && !isMoving)
        {
            
            animator.SetBool(isMovingHash, true);
        }

        if (!movementPressed && isMoving)
        {
            animator.SetBool(isMovingHash, false);
        }

        
    }

    private void OnEnable()
    {
        // enable the character controls action map
        input.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        // disable the character controls actions map
        input.CharacterControls.Disable();
    }

}
