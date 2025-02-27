using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{

    [Header("Object InputActions")]
    [SerializeField]
    [Tooltip("Script Untuk Menerima Input dari Player")]
    InputManager _InputActions;


    [Header("Gerak")]
    [SerializeField]
    [Tooltip("Kecepatan Gerak")]
    float _WalkSpeed = 2f;  
    [SerializeField]
    [Tooltip("Kecepatan Sprint")]
    private float _SprintSpeed;
    [SerializeField]
    [Tooltip("Transisi Kecepatan")]
    private float _WalkSprintTransition;
    private float _Speed;

    Rigidbody _Rigidbody;
    private float _RotationSmoothTime = 0.1f;
    private float _RotationSmoothSpeed;

    [Header("Loncat")]
    [SerializeField]
    [Tooltip("Posisi Deteksi Tanah")]
    private Transform _GroundDetector;
    [Tooltip("Layer Dari Deteksi Tanah")]
    [SerializeField]
    private LayerMask _GroundLayer;
    [Tooltip("Radius untuk Mendeteksi player dan tanah")]
    [SerializeField]
    private float _DetectorRadius;
    [Tooltip("Kekuatan Loncat nya")]
    [SerializeField]
    private float _JumpForce;

    private bool _IsGround;


    [Header("Naik Tangga")]
    [SerializeField]
    [Tooltip("Offset Vector 3 ")]
    private Vector3 _UpperStepOffset;
    [SerializeField]
    [Tooltip("Jarak Antara Tangga")]
    private float _StepCheckerDistance;
    [SerializeField]
    [Tooltip("Kekuatan Step")]
    private float _StepForce;


    [Header("Climb")]
    [Tooltip("Posisi Untuk Mnedeteksi Climb")]
    [SerializeField] Transform _ClimbDetector;

    [Tooltip("Jarak Untuk Mnedeteksi Climb")]
    [SerializeField] float _ClimbCheckDistance;


    [Tooltip("Layer Untuk Climb")]
    [SerializeField] LayerMask _ClimbLayer;

    [Tooltip("Offset Untuk Climb")]
    [SerializeField] Vector3 _ClimbOffset;


    [Tooltip("Kecepatan Climb")]
    [SerializeField] float _ClimbSpeed;
    private PlayerStance _Stance;



    [Header("Camera")]
    [SerializeField]
    [Tooltip("Posisi Camera")]
    private Transform _cameraTransform;
    [SerializeField]
    [Tooltip("Mengatur Camera")]
    private CameraManager _CameraManager;

    private void Awake()
    {
        _Stance = PlayerStance.Stand;
        _Rigidbody = GetComponent<Rigidbody>();
        _Speed = _WalkSpeed;
        HideAndLockCursor();
    }


    private void HideAndLockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;   
        Cursor.visible = false;
    }
    private void Subscribe()
    {
        _InputActions.OnClimbInput += StartClimb;
        _InputActions.OnMoveInput += Move;
        _InputActions.OnSprintInput += Sprint;
        _InputActions.OnJumpInput += Jump;
        _InputActions.OnCanceClimbInput += CancelClimb;
        _InputActions.OnChangePOV += _CameraManager.SwitchCamera;
    }
    private void UnSubscribe()
    {
        _InputActions.OnChangePOV -= _CameraManager.SwitchCamera;
        _InputActions.OnClimbInput -= StartClimb;
        _InputActions.OnMoveInput -= Move;
        _InputActions.OnSprintInput -= Sprint;
        _InputActions.OnJumpInput -= Jump;
        _InputActions.OnCanceClimbInput -= CancelClimb;
    }

    private void CancelClimb()
    {
        if (_Stance == PlayerStance.Climb)
        {
            _CameraManager.SetTPSFieldOfView(40);
            _Stance = PlayerStance.Stand;
            _Rigidbody.useGravity = true;
            transform.position -= transform.forward * 1;
            _CameraManager.SetFpsClampedCamera(false, transform.eulerAngles);
        }
    }
    private void StartClimb()
    {
        if (Physics.Raycast(_ClimbDetector.transform.position, transform.forward, out RaycastHit hit, _ClimbCheckDistance, _ClimbLayer) && _IsGround && _Stance != PlayerStance.Climb && _IsGround) {
            _CameraManager.SetTPSFieldOfView(70);
            Vector3 OffSet = (transform.forward * _ClimbOffset.z) + (Vector3.up * _ClimbOffset.y);
            transform.position = hit.point - OffSet;    
            _Stance = PlayerStance.Climb;
            _Rigidbody.useGravity = false ;
            _CameraManager.SetFpsClampedCamera(true,transform.rotation.eulerAngles);
        } 
    }

    private void CheckStep()
    {
        bool IsHitLowerStep = Physics.Raycast(_GroundDetector.position,transform.forward,_StepCheckerDistance);
        bool IsHitUpperStep = Physics.Raycast(_GroundDetector.position + _UpperStepOffset, transform.forward, _StepCheckerDistance);
        if (IsHitLowerStep && !IsHitUpperStep) _Rigidbody.AddForce(0f, _StepForce, 0f);

    }
    void Start()
    {
        Subscribe();
    }
    private void OnDestroy()
    {
        UnSubscribe();
    }


    private void Sprint(bool IsSprint)
    {
        if (IsSprint)
        {
            if (_Speed < _SprintSpeed)
            {
                _Speed = _Speed + _WalkSprintTransition * Time.deltaTime;
            }
        }
        else
        {
            if (_Speed > _SprintSpeed)
            {
                _Speed = _Speed - _WalkSprintTransition * Time.deltaTime;
            }
    }
        }
    private void Move(Vector2 AxisDirection)
    {
        Vector3 MovementDirection = Vector3.zero;
        if(_Stance == PlayerStance.Stand)
        {
            switch (_CameraManager._CameraState)
            {
                case CameraState.ThirdPerson:
                    if (AxisDirection.magnitude >= 0.1f)
                    {
                        float rotasiAngle = Mathf.Atan2(AxisDirection.x, AxisDirection.y) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                        float SmoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotasiAngle, ref _RotationSmoothSpeed, _RotationSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, SmoothAngle, 0f);
                        MovementDirection = Quaternion.Euler(0f, rotasiAngle, 0f) * Vector3.forward;
                        _Rigidbody.AddForce(MovementDirection * Time.deltaTime * _Speed);
                    }
                    break;
                case CameraState.FirstPerson:
                    transform.rotation = Quaternion.Euler(0f,_cameraTransform.eulerAngles.y,0f);
                    Vector3 verticalDirection = AxisDirection.y * transform.forward;
                    Vector3 horizontalDirection = AxisDirection.x * transform.right;
                    MovementDirection = verticalDirection + horizontalDirection;
                    _Rigidbody.AddForce(MovementDirection * Time.deltaTime * _Speed);
                    break;
                default:
                    break;
            }
        }
        else if (_Stance == PlayerStance.Climb)
        {
            Vector3 Horizontal = AxisDirection.x * transform.right;
            Vector3 Vertikal = AxisDirection.y * transform.up;
            MovementDirection = Horizontal + Vertikal;
            _Rigidbody.AddForce(MovementDirection * Time.deltaTime * _ClimbSpeed);

        }


    }   
    private void CheckGrounded()
    {
        _IsGround = Physics.CheckSphere(_GroundDetector.position, _DetectorRadius, _GroundLayer);
    }
    private void Jump()
    {
        if (_IsGround) _Rigidbody.AddForce(Vector3.up * _JumpForce * Time.deltaTime);
    }
    void Update()
    {
        CheckGrounded();
        CheckStep();
    }
}
