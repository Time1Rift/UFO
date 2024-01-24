using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private Transform _transform;
    private Vector3 _startPosition;

    private void Awake()
    {
        _transform = transform;
        _startPosition = _transform.position;
    }

    private void LateUpdate()
    {
        _startPosition.x = _target.position.x;
        _transform.position = _startPosition;
    }
}