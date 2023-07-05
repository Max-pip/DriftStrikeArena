using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{

    #region Parameters
    [SerializeField] private Rigidbody _rigidbody;

    [Header("Initial Parameters")]
    [SerializeField] private float _initialAccel = 15.0f;         // In meters/second2 
    [SerializeField] private float _initialGripX = 12.0f;         // In meters/second2
    [SerializeField] private float _initialGripZ = 3.0f;          // In meters/second2
    [SerializeField] private float _initialRotVel = 0.8f;         // Ratio of forward velocity transfered on rotation
    [SerializeField] private float _TopSpeed = 30.0f;      // In meters/second
    public float initialRotate = 170;                            // In degree/second

    //Center of mass
    [SerializeField] private Vector3 _centerOfMass = new Vector3(0f, -1f, 0f);

    [Header("Rotation Parameters")]
    [SerializeField] private AnimationCurve _slipL;    // Slip hysteresis static to full (x, input = speed)
    [SerializeField] private AnimationCurve _slipU;    // Slip hysteresis full to static (y, output = slip ratio)
    [SerializeField] private float _slipModification = 20f;     // Basically widens the slip curve
    private float _minRotationSpeed = 1f;           // Velocity to start rotating
    private float _maxRotationSpeed = 4f;           // Velocity to reach max rotation

    [Header("For Check Ground")]
    [SerializeField] private WheelCollider _wheelCollider;
    private Bounds _groupCollider;
    private float _distToGround;

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

    #endregion

    void Start()
    {
        AllForStart();
    }

    void Update()
    {
        Debug.DrawRay(transform.position, _rigidbody.velocity / 2, Color.green);
        Debug.DrawRay(transform.position, -transform.up * (_distToGround + (_wheelCollider.radius * 1.5f)), Color.red);
    }

    void FixedUpdate()
    {
        _accel = _initialAccel;
        _rotate = initialRotate;
        _gripX = _initialGripX;
        _gripZ = _initialGripZ;
        _rotateVelocity = _initialRotVel;
        _rigidbody.angularDrag = _angularDragGround;

        AdjustmentOfSlope();

        CheckGround();

        RotateBehaviour();

        Controller();

        RotateVelocityVector();

        Grip();

        MaxSpeed();

        _rigidbody.velocity = transform.TransformDirection(_velocityVector);
    }

    private void AllForStart()
    {
        _groupCollider = GetBounds(gameObject);     // Get the full collider boundary of group
        _distToGround = _groupCollider.extents.y;    // Pivot to the outermost collider

        // Move the CoM to a fraction of colliders boundaries
        _rigidbody.centerOfMass = Vector3.Scale(_groupCollider.extents, _centerOfMass);
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
            _rigidbody.angularDrag = _angularDragAir;
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

        if (_rotate > initialRotate) _rotate = initialRotate;

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
            if (_slip == 0f) _isSlip = false;
        }

        _rotate *= (1f - 0.3f * _slip);   // Overall rotation, (body + vector)
        _rotateVelocity *= (1f - _slip);          // The vector modifier (just vector)
    }

    private void RotateVelocityVector()
    {
        // Get the local-axis velocity after rotation
        _velocityVector = transform.InverseTransformDirection(_rigidbody.velocity);

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
        if (_velocityVector.z > _TopSpeed) _velocityVector.z = _TopSpeed;
        else if (_velocityVector.z < -_TopSpeed) _velocityVector.z = -_TopSpeed;
    }

    // Executing the queued inputs
    void Controller()
    {

        if (ForwardValue > 0.5f || ForwardValue < -0.5f)
        {
            _rigidbody.velocity += transform.forward * ForwardValue * _accel * Time.deltaTime;
            _gripZ = 0f;     // Remove straight grip if wheel is rotating
        }

        _isRotating = false;

        // Get the local-axis velocity before new input (+x, +y, and +z = right, up, and forward)
        _pVelocityVector = transform.InverseTransformDirection(_rigidbody.velocity);

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

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
        {

            Vector3 rigidbodyForward = transform.forward;

            Rigidbody enemyRigidbody = other.gameObject.GetComponentInParent<Rigidbody>();

            if (enemyRigidbody != null)
            {
                
                enemyRigidbody.AddForce((rigidbodyForward + new Vector3(0, 3f, 0)) * 5000f, ForceMode.Impulse);
            } else
            {
                Debug.Log("No enemy rigidbody");
            }
        }
    }
    */
}
