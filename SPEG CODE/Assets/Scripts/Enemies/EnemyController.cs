using UnityEngine;

public class EnemyController : MonoBehaviour, ICombatant
{
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private Animator _animator;
    [SerializeField] private BulletEmitter _bulletEmitter;
    [SerializeField] private Transform _targetPlayer;
    private int _currentHealth;
    private bool _isDefeated;
    private float _nextAttackTime;
    private bool _isAttacking;
    private bool _wasInAttackState;
    private bool _attackRequested;
    private bool _wasPlayerInAggro;

    [Header("Attacks")]
    [SerializeField] private int _attackCount = 2;
    private int _attackIndex = -1;

    public string DisplayName => _enemyData != null ? _enemyData.EnemyName : name;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _enemyData != null ? _enemyData.MaxHealth : _currentHealth;
    public bool IsDead => _isDefeated;

    private void Awake()
    {
        if (_enemyData != null) _currentHealth = _enemyData.MaxHealth;
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_bulletEmitter == null) _bulletEmitter = GetComponent<BulletEmitter>();
    }

    private void Start() {
        _targetPlayer = PlayerController.Instance.transform;
    }

    private void Update()
    {
        if (_isDefeated || _enemyData == null) return;

        if (_animator != null)
        {
            bool isInAttackState = _animator.GetCurrentAnimatorStateInfo(0).IsTag("attack");

            if (isInAttackState && !_wasInAttackState) _isAttacking = true;

            if (!isInAttackState && _wasInAttackState) OnAttackStateExited();

            _wasInAttackState = isInAttackState;
        }

        bool playerInAggro = IsPlayerInAggroCylinder(_targetPlayer.position);
        if (playerInAggro && !_wasPlayerInAggro)
        {
            _nextAttackTime = Time.time + _enemyData.AttackInterval;
        }

        if (!playerInAggro)
        {
            if (!_isAttacking)
            {
                _attackRequested = false;
            }
            _wasPlayerInAggro = false;
            return;
        }
        _wasPlayerInAggro = true;

        if (!_isAttacking && !_attackRequested)
        {
            FaceTarget(_targetPlayer.position);
        }

        if (!_isAttacking && !_attackRequested && Time.time >= _nextAttackTime && _attackCount > 0)
        {
            TriggerNextAttack();
        }
    }

    private bool IsPlayerInAggroCylinder(Vector3 playerPosition)
    {
        Vector3 toPlayer = playerPosition - transform.position;
        float horizontalDistance = new Vector2(toPlayer.x, toPlayer.z).magnitude;
        float verticalDistance = Mathf.Abs(toPlayer.y);

        return horizontalDistance <= _enemyData.AggroRadius && verticalDistance <= _enemyData.AggroHeight;
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        Vector3 flatToTarget = targetPosition - transform.position;
        flatToTarget.y = 0f;
        if (flatToTarget.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(flatToTarget.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _enemyData.TurnSpeed * Time.deltaTime);
    }

    private void TriggerNextAttack()
    {
        if (_animator == null) return;

        if (_attackCount <= 0) return;

        _attackIndex = (_attackIndex + 1) % _attackCount;
        _attackRequested = true;

        _animator.SetInteger("AttackIndex", _attackIndex);
        _animator.SetTrigger("Attack");
    }

    public void OnAttackStateEntered()
    {
        if (_isDefeated)
        {
            return;
        }

        _isAttacking = true;
        _attackRequested = false;
    }

    public void OnAttackStateExited()
    {
        if (_isDefeated || _enemyData == null)
        {
            return;
        }

        _isAttacking = false;
        _attackRequested = false;
        _nextAttackTime = Time.time + _enemyData.AttackInterval;
    }

    public int GetAttackPower()
    {
        return 1;
    }

    public void OnCombatStart() {}

    public void OnCombatEnd(bool won) {}

    public void TakeDamage(int amount)
    {
        if (_isDefeated || amount <= 0)
        {
            return;
        }

        _currentHealth = Mathf.Max(0, _currentHealth - amount);

        if (_currentHealth <= 0)
        {
            Die();
        }
        else if (_animator != null)
        {
            _animator.SetTrigger("Hit");
        }
    }

    public void Heal(int amount) {
        if (amount <= 0 || _isDefeated) {
            return;
        }

        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, MaxHealth);
    }

    private void Die()
    {
        if (_isDefeated)
        {
            return;
        }

        _isDefeated = true;
        _isAttacking = false;
        _attackRequested = false;

        if (_animator != null)
        {
            _animator.SetTrigger("Death");
        }
    }
}

