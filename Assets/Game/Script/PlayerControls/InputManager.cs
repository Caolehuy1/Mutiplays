using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    AnimatorManager animatorManeger;
    PlayerMovement playerMovement;

    public Vector2 movementInput;
    public Vector2 cameraMovementInput;

    public float verticalInput;
    public float horizontalInput;
    public float cameraInputX;
    public float cameraInputY;
    public float movementAmount;
    [Header("Input Buttons Flag")]
    public bool bInput;
    public bool jumpInput;

    public bool fireInput;
    public bool reloadInput;
    public bool scopeInput;

     void Awake()
    {
        animatorManeger = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.CameraMovement.performed += i => cameraMovementInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.B.performed += i => bInput = true;
            playerControls.PlayerActions.B.canceled += i => bInput = false;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.Fire.performed += i => fireInput = true;
            playerControls.PlayerActions.Fire.canceled += i => fireInput = false;
            playerControls.PlayerActions.Reload.performed += i => reloadInput = true;
            playerControls.PlayerActions.Scope.performed += i => scopeInput = true;
            playerControls.PlayerActions.Scope.canceled += i => scopeInput = false;
        }

        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllinptus()
    {
        HandleMovementInput();
        HandleSpringtingInput();
        HandleJumpingInput();

    }
    private void HandleMovementInput()
    {
       verticalInput = movementInput.y;
      horizontalInput = movementInput.x; 

        cameraInputX = cameraMovementInput.x;
        cameraInputY = cameraMovementInput.y;

        movementAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManeger.ChangeAnimatorValue(0, movementAmount, playerMovement.isSprinting);
    }
    private void HandleSpringtingInput()
    {
        if(bInput && movementAmount > 0.5)
        {
            playerMovement.isSprinting = true;
        }
        else
        {
            playerMovement.isSprinting = false;
        }
    }
    private void HandleJumpingInput()
    {
        if(jumpInput)
        {
            jumpInput = false;
            playerMovement.HandleJumping();
        }
    }
}


