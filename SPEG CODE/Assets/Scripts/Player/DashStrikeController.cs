using UnityEngine;
using UnityEngine.InputSystem;

public class DashStrikeController : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeed    = 22f;
    [SerializeField] private float _dashDuration = 0.18f;
    [SerializeField] private float _dashCooldown = 1.2f;
    [SerializeField] private LayerMask _wallLayer;

    [Header("Damage")]
    [SerializeField] private int _dashDamage = 15;
    [SerializeField] private float _hitRadius = 0.8f;
    [SerializeField] private LayerMask _enemyLayer;

    [Header("References")]
    [SerializeField] private TrailRenderer _trailRenderer;

    public bool IsDashing => _state != DashState.Idle;

    private enum DashState { Idle, Dashing }

    private DashState _state = DashState.Idle;
    private Vector3   _dashDir;
    private float     _dashTimer;
    private float     _cooldownTimer;

    // Track enemies already hit so we only damage each one once per dash
    private readonly Collider[] _hitBuffer = new Collider[8];
    private readonly System.Collections.Generic.HashSet<IDamageable> _hitThisDash
        = new System.Collections.Generic.HashSet<IDamageable>();

    private PlayerMovement _movement;
    private Animator       _animator;

    private static readonly int DashStartTrigger = Animator.StringToHash("DashStart");
    private static readonly int DashEndTrigger   = Animator.StringToHash("DashEnd");

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        if (_state == DashState.Idle
            && _cooldownTimer <= 0f
            && Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            StartDash();
        }

        UpdateDash();
    }

    private void StartDash()
    {
        _dashDir   = GetDashDirection();
        _dashTimer = _dashDuration;
        _state     = DashState.Dashing;
        _hitThisDash.Clear();

        if (_trailRenderer != null)
            _trailRenderer.emitting = true;

        if (_animator != null)
        {
            _animator.ResetTrigger(DashEndTrigger);
            _animator.SetTrigger(DashStartTrigger);
        }
    }

    // Stub — hook up to a windup animation event later if needed.
    public void FireDash() { }

    private void UpdateDash()
    {
        if (_state != DashState.Dashing) return;

        _dashTimer -= Time.deltaTime;

        if (Physics.Raycast(transform.position, _dashDir, 0.6f, _wallLayer))
        {
            FinishDash();
            return;
        }

        _movement.SetGrappleMotion(_dashDir * _dashSpeed);

        // Check for enemies in a sphere around the player each frame,
        // mirroring how MagicBlast uses OnTriggerEnter per-frame contact.
        CheckDamage();

        if (_dashTimer <= 0f)
            FinishDash();
    }

    private void CheckDamage()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position, _hitRadius, _hitBuffer, _enemyLayer);

        for (int i = 0; i < count; i++)
        {
            var damageable = _hitBuffer[i].GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.IsDead) continue;

            // HashSet prevents hitting the same enemy more than once per dash,
            // same idea as MagicBlast's _hasHit flag.
            if (!_hitThisDash.Add(damageable)) continue;

            damageable.TakeDamage(_dashDamage);
        }
    }

    private void FinishDash()
    {
        _movement.SetGrappleMotion(Vector3.zero);
        _state         = DashState.Idle;
        _cooldownTimer = _dashCooldown;
        _hitThisDash.Clear();

        if (_trailRenderer != null)
            _trailRenderer.emitting = false;

        if (_animator != null)
        {
            _animator.ResetTrigger(DashStartTrigger);
            _animator.SetTrigger(DashEndTrigger);
        }
    }

    private Vector3 GetDashDirection()
    {
        Vector3 input = _movement.MoveInput;
        return input.sqrMagnitude > 0.01f ? input.normalized : transform.forward;
    }
}