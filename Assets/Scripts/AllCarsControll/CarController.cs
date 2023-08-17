using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    #region Parameters
    private const string PlayerTag = "Player";
    private const string AccelerationString = "acceleration";
    private const string BreakingString = "breaking";

    public Rigidbody rigidbody;

    [SerializeField] private AudioSource _carAudio;
    [SerializeField] private AudioClip _fallingCarClip;
    public static Action<string> onTouchedAccelerationPlatform;

    [Header("Update Parameters")]
    [SerializeField] private float _updateAccel = 15.0f;         // In meters/second2 
    [SerializeField] private float _updateGripX = 12.0f;         // In meters/second2
    [SerializeField] private float _updateGripZ = 3.0f;          // In meters/second2
    [SerializeField] private float _updateRotVel = 0.8f;         // Ratio of forward velocity transfered on rotation
    [SerializeField] private float _updateTopSpeed = 30.0f;      // In meters/second
    public float updateRotate = 170;                            // In degree/second

    [Header("Acceleration Parameters")]
    [SerializeField] private float _boostAccel = 25;
    [SerializeField] private float _boostMaxSpeed = 45;
    [SerializeField] private float _boostRotate = 300;
    [SerializeField] private float _reduceAccel = 10;
    [SerializeField] private float _reduceMaxSpeed = 20;
    private bool _isChangingAccel;
    float initialTopSpeed;
    float initialAccel;
    float initialRotate;


    [Header("Center of mass")]
    [SerializeField] private Vector3 _centerOfMass = new Vector3(0f, -1f, 0f);

    [Header("Rotation Parameters")]
    [SerializeField] private AnimationCurve _slipL;    // Slip hysteresis static to full (x, input = speed)
    [SerializeField] private AnimationCurve _slipU;    // Slip hysteresis full to static (y, output = slip ratio)
    [SerializeField] private float _slipModification = 20f;     // Basically widens the slip curve
    [SerializeField] private TrailRenderer _leftTrail;
    [SerializeField] private TrailRenderer _rightTrail;
    private float _minRotationSpeed = 1f;           // Velocity to start rotating
    private float _maxRotationSpeed = 4f;           // Velocity to reach max rotation

    [Header("For Check Ground")]
    [SerializeField] private WheelCollider _wheelCollider;
    private Bounds _groupCollider;
    private float _distToGround;

    [Header("Turn over parameters")]
    [SerializeField] private float _pushTurnForce = 4f;
    [SerializeField] private float _pushDurationForce = 0.5f;
    [SerializeField] private float _smoothTurnDuration = 5;
    private float _timeForTurnCoroutine;

    private MeshCollider _myMeshCollider;

    // Ground & air angular drag
    // reduce stumbling time on ground but maintain on-air one
    private float _angularDragGround = 5.0f;
    private float _angularDragAir = 0.05f;

    // The actual value to be used (modification of parameters)
    private float _accel;
    private float _gripX;
    private float _gripZ;
    private float _rotate;
    private float _rotateVelocity;
    private float _topSpeed;
    private float _slip;     // The value used based on Slip curves

    // For determining drag direction
    private float _rightDragValue = 1.0f;
    private float _forwardDragValue = 1.0f;

    private bool _isRotating = false;
    private bool _isGrounded = true;

    // Control signals
    public float ForwardValue { get; set; } = 0f;
    public float TurnValue { get; set; } = 0f;
    private bool _isSlip = false;

    private Vector3 _velocityVector = new Vector3(0f, 0f, 0f);
    private Vector3 _pVelocityVector = new Vector3(0f, 0f, 0f);

    private float _receiveDamage = 2000f;

    public float ReceiveDamage
    {
        get { return _receiveDamage; } 
        private set { _receiveDamage = value; }
    }

    #endregion

    void Start()
    {
        AllForStart();
    }

    void Update()
    {
        Debug.DrawRay(transform.position, rigidbody.velocity / 2, Color.green);
    }

    
    void FixedUpdate()
    {
           
        _accel = _updateAccel;
        _gripX = _updateGripX;
        _gripZ = _updateGripZ;
        _rotate = updateRotate;
        _rotateVelocity = _updateRotVel;
        _topSpeed = _updateTopSpeed;
        rigidbody.angularDrag = _angularDragGround;
        


        AdjustmentOfSlope();

        CheckGround();

        RotateBehaviour();

        Trail();

        Controller();

        RotateVelocityVector();

        Grip();

        MaxSpeed();

        rigidbody.velocity = transform.TransformDirection(_velocityVector);
        
    }
   

    private void AllForStart()
    {
        _myMeshCollider = GetComponentInChildren<MeshCollider>();
        _groupCollider = GetBounds(gameObject);     // Get the full collider boundary of group
        _distToGround = _groupCollider.extents.y;    // Pivot to the outermost collider

        // Move the CoM to a fraction of colliders boundaries
        rigidbody.centerOfMass = Vector3.Scale(_groupCollider.extents, _centerOfMass);
    }

    #region FixedUpdateMethods

    private void AdjustmentOfSlope()
    {
        _accel = _accel * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad);
        _accel = _accel > 0f ? _accel : 0f;
        _gripZ = _gripZ * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad);
        _gripZ = _gripZ > 0f ? _gripZ : 0f;
        _gripX = _gripX * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad);
        _gripX = _gripX > 0f ? _gripX : 0f;
    }

    private void CheckGround()
    {
        // A short raycast to check below
        _isGrounded = Physics.Raycast(transform.position, -transform.up, _distToGround + (_wheelCollider.radius * 1.5f));
        if (!_isGrounded)
        {
            _rotate = 0f;
            _accel = 0f;
            _gripX = 0f;
            _gripZ = 0f;
            rigidbody.angularDrag = _angularDragAir;
        }
    }

    private void RotateBehaviour()
    {
        // Start turning only if there's velocity
        if (_pVelocityVector.magnitude < _minRotationSpeed)
        {
            _rotate = 0f;
        }
        else
        {
            _rotate = _pVelocityVector.magnitude / _maxRotationSpeed * _rotate;
        }

        if (_rotate > updateRotate) _rotate = updateRotate;

        // Calculate grip based on sideway velocity in hysteresis curve
        if (!_isSlip)
        {
            // Normal => slip
            _slip = this._slipL.Evaluate(Mathf.Abs(_pVelocityVector.x) / _slipModification);
            if (_slip == 1f) _isSlip = true;
        }
        else
        {
            // Slip => Normal
            _slip = this._slipU.Evaluate(Mathf.Abs(_pVelocityVector.x) / _slipModification);
            if (_slip != 1f) _isSlip = false;
        }

        _rotate *= (1f - 0.3f * _slip);   // Overall rotation, (body + vector)
        _rotateVelocity *= (1f - _slip);          // The vector modifier (just vector)
    }

    private void Trail()
    {
        if (_leftTrail != null && _rightTrail != null)
        {
            if (_isSlip && _isGrounded)
            {
                _leftTrail.emitting = true;
                _rightTrail.emitting = true;
            }
            else
            {
                _leftTrail.emitting = false;
                _rightTrail.emitting = false;
            }
        }       
    }

    private void RotateVelocityVector()
    {
        // Get the local-axis velocity after rotation
        _velocityVector = transform.InverseTransformDirection(rigidbody.velocity);

        // Rotate the velocity vector
        if (_isRotating)
        {
            _velocityVector = _velocityVector * (1 - _rotateVelocity) + _pVelocityVector * _rotateVelocity; // Partial transfer
        }
    }

    private void Grip()
    {
        // Sideway grip
        _rightDragValue = _velocityVector.x > 0f ? 1f : -1f;
        _velocityVector.x -= _rightDragValue * _gripX * Time.deltaTime;  // Accelerate in opposing direction
        if (_velocityVector.x * _rightDragValue < 0f) _velocityVector.x = 0f;       // Check if changed polarity

        // Straight grip
        _forwardDragValue = _velocityVector.z > 0f ? 1f : -1f;
        _velocityVector.z -= _forwardDragValue * _gripZ * Time.deltaTime;
        if (_velocityVector.z * _forwardDragValue < 0f) _velocityVector.z = 0f;
    }

    private void MaxSpeed()
    {
        // Top speed
        if (_velocityVector.z > _topSpeed) _velocityVector.z = _topSpeed;
        else if (_velocityVector.z < -_topSpeed) _velocityVector.z = -_topSpeed;
    }

    // Executing the queued inputs
    void Controller()
    {

        if (ForwardValue > 0.5f || ForwardValue < -0.5f)
        {
            rigidbody.velocity += transform.forward * ForwardValue * _accel * Time.deltaTime;
            _gripZ = 0f;     // Remove straight grip if wheel is rotating
        }

        _isRotating = false;

        // Get the local-axis velocity before new input (+x, +y, and +z = right, up, and forward)
        _pVelocityVector = transform.InverseTransformDirection(rigidbody.velocity);

        // Turn statically
        if (TurnValue > 0.5f || TurnValue < -0.5f)
        {
            float dir = (_pVelocityVector.z < 0) ? -1 : 1;    // To fix direction on reverse
            RotateGradConst(TurnValue * dir);
        }
    }

    Vector3 drot = new Vector3(0f, 0f, 0f);

    void RotateGradConst(float isCW)
    {
        drot.y = isCW * _rotate * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(drot.y, transform.up);
        _isRotating = true;
    }
    #endregion

    // Get bound of a large 
    public static Bounds GetBounds(GameObject obj)
    {

        // Switch every collider to renderer for more accurate result
        Bounds bounds = new Bounds();
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        if (colliders.Length > 0)
        {

            //Find first enabled renderer to start encapsulate from it
            foreach (Collider collider in colliders)
            {

                if (collider.enabled)
                {
                    bounds = collider.bounds;
                    break;
                }
            }

            //Encapsulate (grow bounds to include another) for all collider
            foreach (Collider collider in colliders)
            {
                if (collider.enabled)
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }
        }
        return bounds;
    }
    
    public float TakingDamage(float inscreaseTakingDamage)
    {
        _receiveDamage += inscreaseTakingDamage;
        return _receiveDamage;
    }

    #region TurnOverVehicleCoroutine

    private IEnumerator AddImpulseVahicleCoroutine()
    {
        rigidbody.useGravity = false;

        rigidbody.AddForce(Vector3.up * _pushTurnForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(_pushDurationForce);

        rigidbody.useGravity = true;
    }

    private IEnumerator DefaultZTurnRotationCoroutine()
    {
        rigidbody.interpolation = RigidbodyInterpolation.None;

        yield return new WaitForSeconds(0.2f);

        Quaternion defaultTurnVehicle = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        _timeForTurnCoroutine = 0f;

        while (_timeForTurnCoroutine < 0.4f)
        {
            _timeForTurnCoroutine += Time.deltaTime / _smoothTurnDuration;

            transform.rotation = Quaternion.Slerp(transform.rotation, defaultTurnVehicle, _timeForTurnCoroutine);
            yield return null;
        }

        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        yield return null;
    }

    public void StartDefaultTurnVahicleCoroutine()
    {
        StartCoroutine(DefaultZTurnRotationCoroutine());
        StartCoroutine(AddImpulseVahicleCoroutine());
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone") && _myMeshCollider != null)
        {
            Destroy(_myMeshCollider);
            if (MainManager.Instance.isSoundOn)
            {
                _carAudio.PlayOneShot(_fallingCarClip);
            }
        }

        if (other.CompareTag("AccelPlatform"))
        {
            float angle = Vector3.Angle(transform.forward, other.transform.forward);
            if (angle <= 90 && !_isChangingAccel)
            {
                StartCoroutine(AddAccelerationCoroutine());
            } else if (angle >= 115 && !_isChangingAccel)
            {
                StartCoroutine(ReduceAccelerationCoroutine());
            } else { }
        }
    }

    
    private IEnumerator AddAccelerationCoroutine()
    {
        if (tag == PlayerTag)
        {
            onTouchedAccelerationPlatform?.Invoke(AccelerationString);
        }

        initialTopSpeed = _updateTopSpeed;
        initialAccel = _updateAccel;
        initialRotate = updateRotate;

        _isChangingAccel = true;
        _updateTopSpeed = _boostMaxSpeed;
        _updateAccel = _boostAccel;
        updateRotate = _boostRotate;

        yield return new WaitForSeconds(2f);

        _updateTopSpeed = initialTopSpeed;
        _updateAccel = initialAccel;
        updateRotate = initialRotate;
        _isChangingAccel = false;  

        yield return null;
    }

    private IEnumerator ReduceAccelerationCoroutine()
    {
        if (tag == PlayerTag)
        {
            onTouchedAccelerationPlatform?.Invoke(BreakingString);
        }

        initialTopSpeed = _updateTopSpeed;
        initialAccel = _updateAccel;

        _isChangingAccel = true;
        _updateTopSpeed = _reduceMaxSpeed;
        _updateAccel = _reduceAccel;

        yield return new WaitForSeconds(2f);

        _updateTopSpeed = initialTopSpeed;
        _updateAccel = initialAccel;
        _isChangingAccel = false;

        yield return null;
    }
    
}
