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

    [Header("Combo")]
    [SerializeField, Range(0f, 1f)] private float _comboWindowStart = 0.45f;
    [SerializeField, Range(0f, 1f)] private float _comboWindowEnd = 0.90f;

    [Header("SFX")]
    [SerializeField] private AudioClip _attackSoundClip;
    [SerializeField] private AudioClip _attackHitSoundClip;
    [SerializeField] private AudioClip[] _footstepSoundClips;

    private PlayerMovement _movement;
    private PlayerStats _stats;
    private Animator _animator;

    private bool _isBusy;
    private bool _wasDead;

    private int _comboQueuedFromHash;

    private static readonly int MoveSpeedParam  = Animator.StringToHash("MoveSpeed");
    private static readonly int IsDeadParam      = Animator.StringToHash("IsDead");
    private static readonly int AttackTrigger    = Animator.StringToHash("Attack");
    private static readonly int ComboTrigger     = Animator.StringToHash("Combo");
    private static readonly int IdleHash         = Animator.StringToHash("Idle");
    private static readonly int RunHash          = Animator.StringToHash("Run");
    private static readonly int InwardSlashHash  = Animator.StringToHash("Inward Slash");
    private static readonly int OutwardSlashHash = Animator.StringToHash("Outward Slash");

    public string DisplayName  => _characterData != null ? _characterData.characterName : "Player";
    public int CurrentHealth   => _stats.CurrentHealth;
    public int MaxHealth       => _stats.MaxHealth;
    public bool IsDead         => _stats.IsDead;

    private void Awake()
    {
        _movement  = GetComponent<PlayerMovement>();
        _stats     = GetComponent<PlayerStats>();
        _animator  = GetComponentInChildren<Animator>();

        _movement.SetMovement(_characterData.moveSpeed, _characterData.gravity);
        _wasDead = _stats.IsDead;

        if (_animator != null)
            _animator.SetBool(IsDeadParam, _stats.IsDead);
    }

    private void Update()
    {
        bool isDead = _stats.IsDead;

        if (!_wasDead && isDead)
        {
            _wasDead = true;
            HandleDeath();
        }

        if (!isDead)
        {
            HandleCombatInput(Mouse.current);
            HandleMoveInput(Keyboard.current);
        }

        MaybeUnfreezeOnLocomotionTransition();

        UpdateAnimator();
    }

    private void HandleMoveInput(Keyboard kb)
    {
        if (kb == null) return;

        float h = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float v = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        _movement.SetMoveInput(new Vector2(h, v).normalized);
    }

    private void HandleCombatInput(Mouse mouse)
    {
        if (mouse == null) return;

        // Start attack chain
        if (!_isBusy && mouse.leftButton.wasPressedThisFrame)
        {
            _isBusy = true;
            _movement.SetFrozen(true);
            _comboQueuedFromHash = 0;
            _animator.SetTrigger(AttackTrigger);
            return;
        }

        // Continue attack chain
        if (!_isBusy || _animator == null) return;
        if (!mouse.leftButton.isPressed) return;

        var st = _animator.GetCurrentAnimatorStateInfo(0);
        if (!IsSlashState(st.shortNameHash)) return;

        if (st.shortNameHash == _comboQueuedFromHash) return;

        float t = st.normalizedTime % 1f;
        if (t < _comboWindowStart || t > _comboWindowEnd) return;

        _comboQueuedFromHash = st.shortNameHash;
        _animator.SetTrigger(ComboTrigger);
    }

    private void MaybeUnfreezeOnLocomotionTransition()
    {
        if (!_isBusy) return;
        if (_animator == null) return;
        if (!_animator.IsInTransition(0)) return;

        var next = _animator.GetNextAnimatorStateInfo(0);
        if (!IsLocomotionState(next.shortNameHash)) return;

        _isBusy = false;
        _movement.SetFrozen(false);
        _comboQueuedFromHash = 0;

        _animator.ResetTrigger(ComboTrigger);
    }

    public void OnAttackStateEntered()
    {
        _isBusy = true;
        _movement.SetFrozen(true);
    }

    public void OnAttackStateExited()
    {
        if (!_isBusy) return;

        if (_animator != null)
        {
            var current = _animator.GetCurrentAnimatorStateInfo(0);
            if (IsSlashState(current.shortNameHash)) return;
        }

        _isBusy = false;
        _movement.SetFrozen(false);
        _comboQueuedFromHash = 0;
    }

    // Called by animation event
    private void PerformMeleeHit()
    {
        if (_meleeHitPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(_meleeHitPoint.position, _meleeRadius, _enemyLayer);
        foreach (var hit in hits)
        {
            var dmg = hit.GetComponentInParent<IDamageable>();
            if (dmg != null && !dmg.IsDead) {
                dmg.TakeDamage(_meleeDamage);
                AudioManager.Instance.PlaySfx(_attackHitSoundClip);
                return;
            }
        }
        AudioManager.Instance.PlaySfx(_attackSoundClip);
    }

    private static bool IsSlashState(int hash) => hash == InwardSlashHash || hash == OutwardSlashHash;

    private static bool IsLocomotionState(int hash) => hash == IdleHash || hash == RunHash;

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

    private void PlayFootstepSound()
    {
        AudioManager.Instance.PlaySfx(_footstepSoundClips[Random.Range(0, _footstepSoundClips.Length)]);
    }

    // ICombatant
    public int GetAttackPower() => _meleeDamage;
    public void TakeDamage(int amount) => _stats.TakeDamage(amount);
    public void Heal(int amount) => _stats.Heal(amount);
    public void OnCombatStart() { }
    public void OnCombatEnd(bool won) { }
}
