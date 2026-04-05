using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowTurning : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(5f, 5f, -5f);
    [SerializeField] private float _smoothSpeed = 8f;
    [SerializeField] private float _verticalSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 90f;
    [SerializeField] private float _minHeight = 0f;
    [SerializeField] private float _maxHeight = 10f;
    [SerializeField] private float _minAngle = -90f;
    [SerializeField] private float _maxAngle = 90f;

    private float _currentAngle = 0f;

    private void Update()
    {
        if (Keyboard.current.iKey.isPressed)
            _offset.y += _verticalSpeed * Time.deltaTime;
        else if (Keyboard.current.kKey.isPressed)
            _offset.y -= _verticalSpeed * Time.deltaTime;

        _offset.y = Mathf.Clamp(_offset.y, _minHeight, _maxHeight);

        if (Keyboard.current.jKey.isPressed)
            _currentAngle -= _rotationSpeed * Time.deltaTime;
        else if (Keyboard.current.lKey.isPressed)
            _currentAngle += _rotationSpeed * Time.deltaTime;

        _currentAngle = Mathf.Clamp(_currentAngle, _minAngle, _maxAngle);
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        Quaternion rotation = Quaternion.Euler(0f, _currentAngle, 0f);
        Vector3 rotatedOffset = rotation * _offset;

        Vector3 desired = _target.position + rotatedOffset;
        transform.position = Vector3.Lerp(transform.position, desired, _smoothSpeed * Time.deltaTime);
        transform.LookAt(_target);
    }
}
