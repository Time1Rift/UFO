using System;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [HideInInspector] public bool IsOverrided = false;

    [SerializeField, Min(0)] private float _height;
    [SerializeField, Min(0)] private float _sphereCastRadius;
    [SerializeField, Min(0)] private float _maxDistance;
    [SerializeField, Min(0)] private float _maxForce;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField, Min(0)] private float _damping;

    private Rigidbody _rigidbody;
    private Transform _transform;

    private float _springSpeed;
    private float _oldDistance;
    private float _minForceHeight;
    private float _maxForceHeight;
    private float _distance;
    private float _forceFactor;
    private float _inputY;

    private Vector3 Forward => _transform.forward;

    private void FixedUpdate()
    {
        if (_rigidbody == null)
            return;

        if (IsOverrided)
            ForceUpDown();
        else
            Lift();

        Damping();
    }

    public void Initialize(Rigidbody rigidbody)
    {
        _rigidbody = rigidbody ?? throw new ArgumentNullException(nameof(rigidbody));
        _transform = transform;
    }

    public void SetOverrideControls(float inputY) => _inputY = inputY;

    public float GetCurrentHeight()
    {
        if(Physics.SphereCast(_transform.position, _sphereCastRadius, Forward, out RaycastHit hitInfo, _maxDistance, _layerMask, QueryTriggerInteraction.Ignore))
            return hitInfo.distance;

        return _maxDistance;
    }

    public void SetHeight(float height) => _height = Mathf.Clamp(height, _sphereCastRadius, _maxDistance);

    private void ForceUpDown()
    {
        _rigidbody.AddForce(-Forward * Mathf.Max(0f, ((_inputY > 0 ? 1 : 0) - _springSpeed * _damping) * _maxForce), ForceMode.Force);
    }

    private void Lift()
    {
        if (Physics.SphereCast(_transform.position, _sphereCastRadius, Forward, out RaycastHit hitInfo, _maxDistance, _layerMask, QueryTriggerInteraction.Ignore))
        {
            _distance = hitInfo.distance;

            _minForceHeight = _height + 1f;
            _maxForceHeight = _height - 1f;
            
            _forceFactor = Mathf.Clamp(_distance, _maxForceHeight, _minForceHeight).Remap(_maxForceHeight, _minForceHeight, 1f, 0f);
            _rigidbody.AddForce(-Forward * Mathf.Max(0f, (_forceFactor  - _springSpeed * _damping) * _maxForce), ForceMode.Force);
        }
    }

    private void Damping()
    {
        _springSpeed = (_distance - _oldDistance) * Time.fixedDeltaTime;
        _springSpeed = Mathf.Max(_springSpeed, 0);
        _oldDistance = _distance;
    }
}