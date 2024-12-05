using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager _inputManager;


    public Transform playerTransform;
    public Transform cameraTransform;


    [Header("Camera Movement")]
    public Transform cameraPivot;
    private Vector3 _cameraFollowVelocity = Vector3.zero;
    public float cameraFollowSpeed = 0.3f;
    public float camLookSpeed = 2f;
    public float camPivotSpeed = 2f;
    public float lookAngle;
    public float pivotAngle;


    public float minPivotAngle = -30f;
    public float maxPivotAngle = 30f;


    [Header("Camera Collision")]
    public LayerMask collisionLayer;
    public float cameraCollisionOffset = 0.2f;
    public float minCollsionOffset = 0.2f;
    public float cameraCollisionRadius = 0.2f;


    private float defaultPosition;
    private Vector3 cameraVectorPosition;


    private PlayerMovement _playerMovement;


    [Header("Scope")]
    public GameObject scopeCanvas;
    public GameObject playerUI;
    public Camera mainCamera;
    private bool isScoped = false;
    private float originaFOV = 60f;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerTransform = FindObjectOfType<PlayerManager>().transform;
        _inputManager = FindObjectOfType<InputManager>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
        _playerMovement = FindObjectOfType<PlayerMovement>();
    }


    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        CameraCollision();
        isPlayerScoped();
    }
    private void FollowTarget()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) > 0.01f) // Ngưỡng khác biệt có thể điều chỉnh
        {
            Vector3 targetPosition = Vector3.SmoothDamp(transform.position, playerTransform.position,
                ref _cameraFollowVelocity, cameraFollowSpeed);
            transform.position = targetPosition;
        }
    }


    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (_inputManager.cameraInputX * camLookSpeed);
        pivotAngle = pivotAngle + (_inputManager.cameraInputY * camPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;
        //transform.eulerAngles = rotation;


        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
        //cameraPivot.localEulerAngles = rotation;
        if (_playerMovement.isMoving == false && _playerMovement.isSprinting == false)
        {
            playerTransform.rotation = Quaternion.Euler(0, lookAngle, 0);
            //Quaternion targetRotation = Quaternion.Euler(0, lookAngle, 0);
            //playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        }

    }
    private float rotationSpeed = 30f;
    private void LateUpdate()
    {

    }


    private void CameraCollision()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayer))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffset);
        }
        if (Mathf.Abs(targetPosition) < minCollsionOffset)
        {
            targetPosition = targetPosition - minCollsionOffset;
        }


        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
    private void OnDrawGizmos()
    {
        // Kiểm tra nếu cameraTransform hoặc cameraPivot chưa được gán
        if (cameraTransform == null || cameraPivot == null)
            return;


        // Lấy vị trí mặc định của camera
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();


        // Màu sắc Gizmos mặc định
        Gizmos.color = Color.red;


        // Vẽ SphereCast
        if (Physics.SphereCast(cameraPivot.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayer))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffset);


            // Vẽ đường từ pivot đến điểm va chạm
            Gizmos.color = Color.green;
            Gizmos.DrawLine(cameraPivot.position, hit.point);
            Gizmos.DrawWireSphere(hit.point, cameraCollisionRadius);
        }
        else
        {
            Vector3 endPosition = cameraPivot.position + direction * Mathf.Abs(targetPosition);


            // Vẽ đường từ pivot đến vị trí cuối cùng
            Gizmos.color = Color.red;
            Gizmos.DrawLine(cameraPivot.position, endPosition);
            Gizmos.DrawWireSphere(endPosition, cameraCollisionRadius);
        }


        // Kiểm tra khoảng cách tối thiểu
        if (Mathf.Abs(targetPosition) < minCollsionOffset)
        {
            targetPosition = targetPosition - minCollsionOffset;
        }


        // Vẽ vị trí camera hiện tại
        Vector3 cameraVectorPosition = cameraTransform.localPosition;
        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(cameraTransform.position, cameraCollisionRadius);
    }


    public void isPlayerScoped()
    {
        if (_inputManager.scopeInput)
        {
            scopeCanvas.SetActive(true);
            playerUI.SetActive(false);
            mainCamera.fieldOfView = 10f;
        }
        else
        {
            scopeCanvas.SetActive(false);
            playerUI.SetActive(true);
            mainCamera.fieldOfView = originaFOV;
        }
    }
}


   