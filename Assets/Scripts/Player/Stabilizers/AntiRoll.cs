using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AntiRoll : MonoBehaviour
{
    [SerializeField, Min(0)] private float _stabilizerForce;
    [SerializeField, Min(0)] private float _damping;

    private Rigidbody _rigidbody;
    private Transform _transform;

    private Vector3 _up;
    private float _dot;
    private float _lastDot;
    private float _difference;
    private Vector3 _axis;
    private Vector3 _needVector;
    private Quaternion _needRotate;

    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _up = _transform.up;
        _dot = Vector3.Dot(_up, Vector3.up);
        _axis = Vector3.Cross(_up, Vector3.up);

        Stabilize(_dot, _axis);
        Damping(_dot, _axis);
        AtiRolling(_dot, _up);
    }

    private void AtiRolling(float dot, Vector3 up)
    {
        if (dot <= 0)
        {
            _needVector = Vector3.ProjectOnPlane(up, Vector3.up).normalized;
            _needRotate = Quaternion.FromToRotation(up, _needVector);
            _transform.rotation *= _needRotate;
        }
    }

    private void Stabilize(float dot, Vector3 axis)
    {
        if (dot > 0)
            _rigidbody.AddTorque(axis * (1 - dot) * _stabilizerForce, ForceMode.Force);
    }

    private void Damping(float dot, Vector3 axis)
    {
        _difference = (_lastDot - dot) * Time.fixedDeltaTime;

        if (_difference > 0)
            _rigidbody.AddTorque(-axis * _difference * _damping, ForceMode.Force);

        _lastDot = dot;
    }
}