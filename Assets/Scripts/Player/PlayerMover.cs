using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody), typeof(ConstantForce))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private Engine _engine;
    [SerializeField] private float _constantForcePower;
    [SerializeField] private CowCatcher _cowCatcher;

    private Rigidbody _rigidbody;
    private PlayerInput _input;
    private ConstantForce _constantForce;

    private bool _isVerticalAxisActive;

    private void Awake()
    {
        _constantForce = GetComponent<ConstantForce>();
        _input = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        _cowCatcher.SetInput(_input);

        _engine.Initialize(_rigidbody);
    }

    private void FixedUpdate()
    {
        _constantForce.force = -Vector3.right * _input.Controls.x * _constantForcePower + Physics.gravity * _rigidbody.mass;
    }

    private void Update()
    {
        _isVerticalAxisActive = Mathf.Approximately(_input.Controls.y, 0) == false;

        if (_isVerticalAxisActive)
        {
            _engine.SetHeight(_engine.GetCurrentHeight());
            _engine.SetOverrideControls(_input.Controls.y);
        }

        _engine.IsOverrided = _isVerticalAxisActive;
    }
}