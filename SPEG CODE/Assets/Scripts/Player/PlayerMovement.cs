using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _gravity = -9.8f;
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _rotationSpeed = 720f;

    private CharacterController _cc;
    private Vector3 _moveInput;
    private float _verticalVelocity;
    private bool _isFrozen;
    private Vector3 _grappleMotion;

    public Vector3 MoveInput => _moveInput;
    public bool IsGrounded => _cc != null && _cc.isGrounded;
    public float VerticalVelocity => _verticalVelocity;

    public void SetMovement(float moveSpeed, float gravity, float jumpForce)
    {
        _moveSpeed = moveSpeed;
        _gravity = gravity;
        _jumpForce = jumpForce;
    }

    public void SetFrozen(bool frozen)
    {
        _isFrozen = frozen;
    }

    public void SetGrappleMotion(Vector3 motion)
    {
        _grappleMotion = motion;
    }

    public bool TryJump()
    {
        if (_isFrozen || !IsGrounded) return false;

        _verticalVelocity = _jumpForce;
        return true;
    }

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    public void SetMoveInput(Vector2 input)
    {
        _moveInput = new Vector3(input.x, 0f, input.y);
    }

    private void Update()
    {
        if (_grappleMotion.sqrMagnitude > 0.001f) return;

        if (!_isFrozen && _moveInput.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(_moveInput.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, target, _rotationSpeed * Time.deltaTime);
        }

        if (IsGrounded && _verticalVelocity < 0f)
            _verticalVelocity = -2f;

        _verticalVelocity += _gravity * Time.deltaTime;

        Vector3 horizontal = _isFrozen ? Vector3.zero : _moveInput;
        Vector3 motion = horizontal * _moveSpeed + Vector3.up * _verticalVelocity;
        _cc.Move(motion * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_grappleMotion.sqrMagnitude <= 0.001f) return;

        _verticalVelocity = 0f;
        _cc.Move(_grappleMotion * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var hazard = hit.gameObject.GetComponentInParent<Hazard>();
        if (hazard != null)
        {
            _verticalVelocity = 0f;
            hazard.HandleControllerHit(gameObject);
        }
    }
}
