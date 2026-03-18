using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _gravity = -9.8f;

    private CharacterController _cc;
    private Vector3 _moveInput;
    private float _verticalVelocity;
    private bool _isFrozen;

    public Vector3 MoveInput => _moveInput;
    public bool IsGrounded => _cc.isGrounded;

    public void SetMovement(float moveSpeed, float gravity)
    {
        _moveSpeed = moveSpeed;
        _gravity = gravity;
    }

    public void SetFrozen(bool frozen)
    {
        _isFrozen = frozen;
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
        _verticalVelocity += _gravity * Time.deltaTime;

        Vector3 horizontal = _isFrozen ? Vector3.zero : _moveInput;
        Vector3 motion = horizontal * _moveSpeed + Vector3.up * _verticalVelocity;
        _cc.Move(motion * Time.deltaTime);
    }
}
