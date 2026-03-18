using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ICombatant
{
    [SerializeField] private PlayerCharacterData _characterData;

    [Header("Melee")]
    [SerializeField] private Transform _meleeHitPoint;
    [SerializeField] private float _meleeRadius = 1.2f;
    [SerializeField] private int _meleeDamage = 10;
    [SerializeField] private LayerMask _enemyLayer;

    private PlayerMovement _movement;
    private PlayerStats _stats;
    private Animator _animator;

    private bool _isBusy;
    private bool _wasDead;

    private static readonly int MoveSpeedParam = Animator.StringToHash("MoveSpeed");
    private static readonly int IsDeadParam = Animator.StringToHash("IsDead");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    public string DisplayName => _characterData != null ? _characterData.characterName : "Player";
    public int CurrentHealth => _stats.CurrentHealth;
    public int MaxHealth => _stats.MaxHealth;
    public bool IsDead => _stats.IsDead;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _stats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();

        _movement.SetMovement(_characterData.moveSpeed, _characterData.gravity);
        _isBusy = false;
        _wasDead = _stats.IsDead;

        if (_animator != null)
            _animator.SetBool(IsDeadParam, _stats.IsDead);
    }

    private void Update()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;

        bool isDead = _stats.IsDead;
        if (!isDead && _wasDead) isDead = true; 

        if (!_wasDead && isDead)
        {
            _wasDead = true;
            HandleDeath();
        }

        if (!isDead)
        {
            HandleCombatInput(mouse);
            HandleMoveInput(kb);
        }

        UpdateAnimator();
    }

    private void HandleMoveInput(Keyboard kb)
    {
        float horizontal = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float vertical = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        _movement.SetMoveInput(new Vector2(horizontal, vertical).normalized);
    }

    private void HandleCombatInput(Mouse mouse)
    {
        if (!mouse.leftButton.wasPressedThisFrame) return;
        if (_isBusy) return;

        _isBusy = true;
        _movement.SetFrozen(true);
        _animator.SetTrigger(AttackTrigger);
    }

    // Called by animation event
    private void PerformMeleeHit()
    {
        if (_meleeHitPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(_meleeHitPoint.position, _meleeRadius, _enemyLayer);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null && !damageable.IsDead)
                damageable.TakeDamage(_meleeDamage);
        }
    }

    public void OnAttackStateEntered()
    {
        _isBusy = true;
        _movement.SetFrozen(true);
    }

    public void OnAttackStateExited()
    {
        _isBusy = false;
        _movement.SetFrozen(false);
    }

    private void UpdateAnimator()
    {
        if (_animator == null) return;

        _animator.SetFloat(MoveSpeedParam, _movement.MoveInput.magnitude);
        _animator.SetBool(IsDeadParam, _stats.IsDead);
    }

    private void HandleDeath()
    {
        _isBusy = false;
        _movement.SetFrozen(false);
        _movement.SetMoveInput(Vector2.zero);

        if (_animator != null)
            _animator.SetBool(IsDeadParam, true);
    }

    // ICombatant
    public int GetAttackPower() => _meleeDamage;
    public void TakeDamage(int amount) => _stats.TakeDamage(amount);
    public void Heal(int amount) => _stats.Heal(amount);
    public void OnCombatStart() { }
    public void OnCombatEnd(bool won) { }
}
