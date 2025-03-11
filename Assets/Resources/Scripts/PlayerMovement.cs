using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
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


    [Header("Animator")]
    [Tooltip("Component Animator")]
    private Animator _Animator;


    [Header("Crounch")]
    [SerializeField]

    [Tooltip("kecepatan Crounh")]
    private float _SpeedCrounch;
    [Tooltip("Colider")]
    private CapsuleCollider _collider;



    [Header("Glide")]
    [SerializeField]

    [Tooltip("Kecepatan Glide")]
    private  float _glideSpeed;

    [SerializeField]

    [Tooltip("AirDrag")]
    private float _AirDrag;

    [SerializeField]
    [Tooltip("Rotation Speed")]
    private Vector3 _GlideRotationSpeed;

    [SerializeField]
    [Tooltip("min Rotation Speed")]
    private float _minGlideRotationSpeed;


    [SerializeField]
    [Tooltip("max Rotation Speed")]
    private float _maxGlideRotationSpeed;


    [Header("Punch")]
    [SerializeField]
    [Tooltip("Batas Waktu Combo")]
    private float _resetComboInterval;

    [SerializeField]
    [Tooltip("Hit Detector ")]
    private Transform _hitDetector;

    [SerializeField]
    [Tooltip("Radius")]
    private float _hitDetectorRadius;

    [SerializeField]
    [Tooltip("Layer")]
    private LayerMask _hitlayer;



    private Coroutine _resetCombo;

    private bool _IsPunching;
    private int _combo = 0;

    private void Hit()
    {
        Collider[] hit = Physics.OverlapSphere(_hitDetector.position, _hitDetectorRadius, _hitlayer);

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].gameObject != null) { 
                Destroy(hit[i].gameObject);
            }
        }
    }

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
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
        _InputActions.OnPunchInput += Punch;
        _InputActions.OnChangeCrouch += Crouch;
        _InputActions.OnGlide += StartGlide;
        _InputActions.OnCancelGlide += CancelGlide;
        _InputActions.OnClimbInput += StartClimb;
        _InputActions.OnMoveInput += Move;
        _InputActions.OnSprintInput += Sprint;
        _InputActions.OnJumpInput += Jump;
        _InputActions.OnCanceClimbInput += CancelClimb;
        _InputActions.OnChangePOV += _CameraManager.SwitchCamera;
        _CameraManager.OnChagePerspective += ChagePerspective;
    }

    private void Punch()
    {
        if (!_IsPunching && _Stance == PlayerStance.Stand) { 
            _IsPunching = true;
         
            if(_combo < 3)
            {
                _combo = _combo +1;
            }
            else
            {
                _combo = 1;
            }
            _Animator.SetInteger("Combo",_combo);
            _Animator.SetTrigger("Punch");

        }
    }

    public void EndPunch()
    {
        _IsPunching = false;
        if (_resetCombo != null)
        {
            StopCoroutine(_resetCombo);
        }
        _resetCombo = StartCoroutine(ResetCombo());
    }
    IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(_resetComboInterval);
        _combo = 0;
    }
    private void UnSubscribe()

    {
        _InputActions.OnPunchInput -= Punch;

        _InputActions.OnGlide -= StartGlide;
        _InputActions.OnCancelGlide -= CancelGlide;
        _CameraManager.OnChagePerspective -= ChagePerspective;

        _InputActions.OnChangeCrouch -= Crouch;
        _InputActions.OnChangePOV -= _CameraManager.SwitchCamera;
        _InputActions.OnClimbInput -= StartClimb;
        _InputActions.OnMoveInput -= Move;
        _InputActions.OnSprintInput -= Sprint;
        _InputActions.OnJumpInput -= Jump;
        _InputActions.OnCanceClimbInput -= CancelClimb;

    }

    private void Glide()
    {
        if (_Stance == PlayerStance.Glide)
        {
            Vector3 playerRotation = transform.rotation.eulerAngles;
            float lift = playerRotation.x;
            Vector3 upForce= transform.up  *  (lift + _AirDrag);
            Vector3 forwardForce = transform.forward * _glideSpeed;
            Vector3 totalForce = upForce + forwardForce;
            _Rigidbody.AddForce(totalForce * Time.deltaTime);


        }

    }
    private void StartGlide()
    {
        if (_Stance != PlayerStance.Glide && !_IsGround)
        {
            _CameraManager.SetFpsClampedCamera(true, transform.rotation.eulerAngles);
            _Animator.SetBool("IsGlide", true);
            _Stance = PlayerStance.Glide;
        }
    }

    private void CancelGlide()
    {
        if (_Stance == PlayerStance.Glide)
        {
            _CameraManager.SetFpsClampedCamera(false, transform.rotation.eulerAngles);

            _Animator.SetBool("IsGlide", false);

            _Stance = PlayerStance.Stand;

        }
    }
    private void Crouch()
    {
        if (_Stance == PlayerStance.Stand) {
            _collider.height = 1.3f;
            _collider.center = Vector3.up * 0.66f;
            _Stance = PlayerStance.Crouch;
            _Animator.SetBool("IsCrouch", true);
            _Speed = _SpeedCrounch;
        }else if (_Stance == PlayerStance.Crouch)
        {
            _collider.height = 1.8f;
            _collider.center = Vector3.up * 0.9f;
            _Stance = PlayerStance.Stand;
            _Animator.SetBool("IsCrouch", false);

            _Speed = _WalkSpeed;

        }
    }
    public void ChagePerspective()
    {
        _Animator.SetTrigger("CheckPerspective");
    }
   

    private void CancelClimb()
    {
        if (_Stance == PlayerStance.Climb)
        {
            _collider.center = Vector3.up * 0.9f;

            _Animator.SetBool("IsClimb", false);

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
            _collider.center = Vector3.up * 1.3f;
            _Animator.SetBool("IsClimb", true);
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
        _Animator = GetComponent<Animator>();

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
                _Speed = Mathf.Lerp(_Speed, _SprintSpeed, _WalkSprintTransition * Time.deltaTime);
            }
        }
        else
        {
           
            
                _Speed = Mathf.Lerp(_Speed, _WalkSpeed, _WalkSprintTransition * Time.deltaTime);
            
        }
    }
    private void Move(Vector2 AxisDirection)
    {
        Vector3 MovementDirection = Vector3.zero;
        if ((_Stance == PlayerStance.Stand || _Stance == PlayerStance.Crouch) && !_IsPunching)
        {
            Vector3 velocity = new Vector3(_Rigidbody.velocity.x, 0, _Rigidbody.velocity.z);
            _Animator.SetFloat("Velocity", velocity.magnitude * AxisDirection.magnitude);

            _Animator.SetFloat("VelocityZ", velocity.magnitude * AxisDirection.y);
            _Animator.SetFloat("VelocityX", velocity.magnitude * AxisDirection.x);

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
                    transform.rotation = Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f);
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
            Vector3 velocity = new Vector3(_Rigidbody.velocity.x, _Rigidbody.velocity.y, 0);
            _Animator.SetFloat("ClimbVelocityx", velocity.magnitude * AxisDirection.x);
            _Animator.SetFloat("ClimbVelocityy", velocity.magnitude * AxisDirection.y);

            _Rigidbody.AddForce(MovementDirection * Time.deltaTime * _ClimbSpeed);


        }
        else if (_Stance == PlayerStance.Glide)
        {
            Vector3 RotationDegre = transform.rotation.eulerAngles;
            RotationDegre.x += _GlideRotationSpeed.x * AxisDirection.y * Time.deltaTime;
            RotationDegre.x = Mathf.Clamp(RotationDegre.x , _minGlideRotationSpeed, _maxGlideRotationSpeed);
            RotationDegre.z += _GlideRotationSpeed.z * AxisDirection.x * Time.deltaTime;
            RotationDegre.y += _GlideRotationSpeed.y * AxisDirection.x * Time.deltaTime;
            transform.rotation = Quaternion.Euler(RotationDegre);


        }



    }   
    private void CheckGrounded()
    {

        _IsGround = Physics.CheckSphere(_GroundDetector.position, _DetectorRadius, _GroundLayer);
        _Animator.SetBool("IsGround", _IsGround);
    }
    private void Jump()
    {
        if (_IsGround)
        {
            CancelGlide();  
            _Animator.SetTrigger("Jump");

            _Rigidbody.AddForce(Vector3.up * _JumpForce * Time.deltaTime);

        }
    }
    void Update()
    {

        Glide();
        CheckGrounded();
        CheckStep();
    }
}
