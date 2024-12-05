using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Photon.Pun;
using System.Diagnostics;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    [Header("Player Health")]
    private const float maxHealth = 150f;
    public float currentHealth;
    public Slider healthbarSlider;
    public GameObject playerUI;


    [Header("Ref & Physics")]
    private InputManager _inputManager;
    private PlayerManager _playerManager;
    private PlayerControllerManager _playerControllerManager;
    private AnimatorManager _animatorManager;
    private Vector3 _moveDirection;
    private Transform _cameraGameObejct;
    private Rigidbody _playerRigidbody;


    [Header("Falling and Landing")]
    public float inAirTimer;


    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;
    public LayerMask groundLayer;




    [Header("Movement flags")]
    public bool isMoving;
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;


    [Header("Movement values")]
    public float movementSpeed = 2f;
    public float rotationSpeed = 13f;
    public float sprintingSpeed = 7f;


    [Header("Jump var")]
    public float jumpHeight = 4;
    public float gravityIntensity = -15f;


    private PhotonView view;


    public int playerTeam;


    private void Awake()
    {
        currentHealth = maxHealth;
        _inputManager = GetComponent<InputManager>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _cameraGameObejct = Camera.main.transform;
        _playerManager = GetComponent<PlayerManager>();
        _animatorManager = GetComponent<AnimatorManager>();
        view = GetComponent<PhotonView>();

        _playerControllerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerControllerManager>();


        healthbarSlider.minValue = 0f;
        healthbarSlider.maxValue = maxHealth;
        healthbarSlider.value = currentHealth;

    }


    private void Start()
    {
        if (!view.IsMine)
        {
            Destroy(_playerRigidbody);
            Destroy(playerUI);
        }
        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            playerTeam = team;
        }
    }


    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        if (_playerManager.isInteracting) return;
        HandleMovement();
        HandleRotation();
    }
    private void HandleMovement()
    {
        if (isJumping) return;
        //_moveDirection = _cameraGameObejct.forward * _inputManager.verticalInput; old
        _moveDirection = new Vector3(_cameraGameObejct.forward.x, 0f, _cameraGameObejct.forward.z) *
                         _inputManager.verticalInput;
        _moveDirection = _moveDirection + _cameraGameObejct.right * _inputManager.horizontalInput;
        _moveDirection.Normalize();


        _moveDirection.y = 0;
        if (isSprinting)
        {
            _moveDirection = _moveDirection * sprintingSpeed;
        }
        else
        {
            if (_inputManager.movementAmount >= 0.5f)
            {
                _moveDirection = _moveDirection * movementSpeed;
                isMoving = true;
            }
            if (_inputManager.movementAmount <= 0f)
            {
                isMoving = false;
            }
        }


        Vector3 movementVelocity = _moveDirection;
        _playerRigidbody.velocity = movementVelocity;
    }


    private void HandleRotation()
    {
        if (isJumping) return;
        Vector3 targetDirection = Vector3.zero;
        targetDirection = _cameraGameObejct.forward * _inputManager.verticalInput;
        targetDirection = targetDirection + _cameraGameObejct.right * _inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;


        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        transform.rotation = playerRotation;
    }


    void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffset;
        targetPosition = transform.position;


        if (!isGrounded && !isJumping) // player is jumping
        {
            if (!_playerManager.isInteracting)
            {
                _animatorManager.PlayTargetAnimation("Falling", true);
            }


            inAirTimer = inAirTimer + Time.deltaTime;
            _playerRigidbody.AddForce(transform.forward * leapingVelocity);
            _playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }


        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            if (!isGrounded && !_playerManager.isInteracting)
            {
                _animatorManager.PlayTargetAnimation("Landing", true);
            }


            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }


        if (isGrounded && !isJumping)
        {
            if (_playerManager.isInteracting || _inputManager.movementAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }


    public void HandleJumping()
    {
        if (isGrounded)
        {
            _animatorManager._animator.SetBool("isJumping", true);
            _animatorManager.PlayTargetAnimation("Jump", false);


            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = _moveDirection;
            playerVelocity.y = jumpingVelocity;
            _playerRigidbody.velocity = playerVelocity;


            isJumping = false;
        }
    }


    public void SetIsJumping(bool isJumping)
    {
        this.isJumping = isJumping;
    }


    public void ApplyDamage(float damageValue)
    {
        view.RPC("RPC_TakeDamge", RpcTarget.All, damageValue);
    }


    [PunRPC] // cai nay de goi cho may cho khac co the goi ham nay tu xa
    void RPC_TakeDamge(float damage)
    {
        if (!view.IsMine) return;
        currentHealth -= damage;
        healthbarSlider.value = currentHealth;
        if (currentHealth < 0)
        {
            Die();
        }
        UnityEngine.Debug.Log("Damege taken " + damage);
        UnityEngine.Debug.Log("Current Health " + currentHealth);
    }


    private void Die()
    {
        _playerControllerManager.Die();

        // cong diem
        ScoreBoard.instance.PlayerDied(playerTeam);
    }

}
 