using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public bool canMove { get; private set; } = true;
    public bool isSprinting => canSprint && sprintHeld;

    [Header("Functional Options")] 
    [SerializeField] private bool canSprint = true;
    
    [Header("Movement Parameters")] 
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float gravity = 30.0f;
    
    [Header("Movement Parameters")] 
    [SerializeField, Range(1, 10)] private float lookXSpeed = 2.0f;
    [SerializeField, Range(1, 10)] private float lookYSpeed = 2.0f;
    [SerializeField, Range(1, 100)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 100)] private float lowerLookLimit = 80.0f;

    private Camera playerCamera;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 currentInput, inputMovement, inputView;

    private float rotationX = 0;
    public PlayerInput playerControls;
    bool sprintHeld;
    private void Awake()
    {
        playerControls = new PlayerInput();
        playerControls.Player.Movement.performed += context => inputMovement = context.ReadValue<Vector2>();
        playerControls.Player.Look.performed += context => inputView = context.ReadValue<Vector2>();
        playerControls.Player.Sprint.performed += context => sprintHeld = true;
    }

    //Enable controls on startup
    private void OnEnable()
    {
        playerControls.Enable();
    }

    //Disable controls on shutdown
    private void OnDisable()
    {
        playerControls.Disable();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            HandleMovementInput();
            HandleMouseLook();
            ApplyFinalMovement();
        }
    }

    #region Movement

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isSprinting ? walkSpeed : sprintSpeed) * inputMovement.x, 
            (isSprinting ? walkSpeed : sprintSpeed) * inputMovement.y);
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) +
                        (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }
    
    private void HandleMouseLook()
    {
        rotationX -= inputView.y * lookYSpeed;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0,0);
        transform.rotation *= Quaternion.Euler(0,inputView.x * lookXSpeed, 0);
    }
    
    private void ApplyFinalMovement()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    #endregion
}


