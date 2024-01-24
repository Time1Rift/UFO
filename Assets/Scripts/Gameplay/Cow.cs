using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class Cow : MonoBehaviour
{
    [SerializeField, Min(0)] private float _jumpPower;
    [SerializeField, Min(0)] private float _minJumpTimer = 1f;
    [SerializeField, Min(0)] private float _maxJumpTimer = 2f;
    [SerializeField] private GameObject _deadCowPrefab;

    private Transform _transform;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private float _jumpTimer = 1;
    private bool _isCatched = false;

    private void OnValidate()
    {
        if (_maxJumpTimer <= _minJumpTimer)
            _maxJumpTimer = _minJumpTimer + 1;
    }

    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isCatched == false)
            TryJump();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isCatched)
            return;

        Rigidbody attachedRigidbody = collision.collider.attachedRigidbody;

        if (attachedRigidbody == null)
            return;

        if (attachedRigidbody.TryGetComponent(out PlayerInput player))
        {
            Instantiate(_deadCowPrefab, _transform.position, _transform.rotation);
            Destroy(gameObject);
        }
    }

    public void Catched()
    {
        _isCatched = true;
        _animator.SetBool("Fly", _isCatched);
        _rigidbody.isKinematic = true;
    }

    private void TryJump()
    {
        if (_jumpTimer > 0)
        {
            _jumpTimer -= Time.deltaTime;

            if (_jumpTimer < 0)
            {
                Jump();
                _jumpTimer = Random.Range(_minJumpTimer, _maxJumpTimer);
            }
        }
    }

    private void Jump()
    {
        _animator.SetTrigger("Jump");
        _rigidbody.velocity = (Vector3.up + _transform.forward) * _jumpPower;
    }
}