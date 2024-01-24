using System;
using UnityEngine;
using UnityEngine.Windows;

public class CowCatcher : MonoBehaviour
{
    [SerializeField, Min(0)] private float _catchDistance;
    [SerializeField, Min(0)] private float _catchRadius;
    [SerializeField] private GameObject _effect;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField, Min(0)] private float _catchTime;

    private Transform _transform;
    private bool _isCatchActionActive = false;
    private Cow _cow;
    private float _catchTimer = -1;
    private Vector3 _startCowPosition;
    private Vector3 _startCowScale;

    private void Awake()
    {
        _transform = transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(transform.position + transform.forward * _catchDistance, _catchRadius);
    }

    private void Update()
    {
        if (_catchTimer > 0)
        {
            _catchTimer -= Time.deltaTime / _catchTime;

            if (_catchTimer <= 0)
                if (_cow != null)
                    Destroy(_cow.gameObject);
        }

        if (_cow != null)
            UpdateCowTransform();
    }

    private void FixedUpdate()
    {
        if (_isCatchActionActive == false)
            return;

        if (_cow != null)
            return;

        Collider[] colliders = Physics.OverlapSphere(_transform.position + _transform.forward * _catchDistance, _catchRadius, _layerMask, QueryTriggerInteraction.Ignore);
        
        foreach (Collider collider in colliders)
        {
            _cow = collider.GetComponentInParent<Cow>();

            if (_cow != null)
            {
                _cow.transform.SetParent(_transform);
                _cow.Catched();
                _startCowPosition = _cow.transform.localPosition;
                _startCowScale = _cow.transform.localScale;
                
                _catchTimer = 1f;
                return;
            }
        }
    }

    public void SetInput(PlayerInput input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));
        
        input.CatchPressed += OnCatchPressed;
        input.CatchReleased += OnCatchReleased;
    }

    private void UpdateCowTransform()
    {
        float t = Mathf.SmoothStep(0, 1, _catchTimer);

        _cow.transform.localPosition = Vector3.Lerp(Vector3.zero, _startCowPosition, t);
        _cow.transform.localScale = Vector3.Lerp(Vector3.zero, _startCowScale, t);
    }

    private void OnCatchReleased()
    {
        if (_cow != null)
            return;

        SetCatch(false);
    }

    private void OnCatchPressed()
    {
        SetCatch(true);
    }

    private void SetCatch(bool value)
    {
        _effect.SetActive(value);
        _isCatchActionActive = value;
    }
}