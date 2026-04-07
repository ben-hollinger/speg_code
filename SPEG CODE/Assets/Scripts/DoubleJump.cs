using UnityEngine;

public class DoubleJump : MonoBehaviour
{
    [SerializeField] private int _maxJumps = 2;

    private PlayerMovement _playerMovement;
    private int _jumpsRemaining;
    private bool _wasGrounded;

    public bool HasDoubleJump { get; private set; } = false;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public void UnlockDoubleJump()
    {
        HasDoubleJump = true;
    }

    private void Update()
    {
        if (!HasDoubleJump) return;

        if (_playerMovement.IsGrounded)
            _jumpsRemaining = _maxJumps;

        _wasGrounded = _playerMovement.IsGrounded;
    }

    public bool TryDoubleJump()
    {
        if (!HasDoubleJump) return false;
        if (_playerMovement.IsGrounded) return false;
        if (_jumpsRemaining <= 1) return false;

        _jumpsRemaining--;
        return true;
    }
}