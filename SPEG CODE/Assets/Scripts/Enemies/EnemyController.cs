using UnityEngine;

public class EnemyController : MonoBehaviour, ICombatant
{
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private Animator _animator;
    [SerializeField] private BulletEmitter _bulletEmitter;
    private int _currentHealth;
    private bool _isDefeated;
    private float _nextAttackTime;
    private int _currentPatternIndex = -1;
    private BulletPatternData _currentPattern;

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

    private void Update()
    {
        if (_isDefeated || _enemyData == null) return;

        if (Time.time >= _nextAttackTime && _enemyData.BulletPatterns.Count > 0)
        {
            SelectNextPattern();
            TriggerAttackAnimation();
            _nextAttackTime = Time.time + _enemyData.AttackInterval;
        }
    }

    private void SelectNextPattern()
    {
        if (_enemyData.BulletPatterns.Count == 0)
        {
            _currentPattern = null;
            return;
        }

        if (_currentPatternIndex < 0) _currentPatternIndex = 0;
        else
        {
            _currentPatternIndex++;
            if (_currentPatternIndex >= _enemyData.BulletPatterns.Count)
            {
                _currentPatternIndex = 0;
            }
        }

        _currentPattern = _enemyData.BulletPatterns[_currentPatternIndex];
    }

    private void TriggerAttackAnimation()
    {
        if (_animator == null) return;

        _animator.SetInteger("AttackIndex", _currentPatternIndex);
        _animator.SetTrigger("Attack");
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

        if (_animator != null)
        {
            _animator.SetTrigger("Death");
        }
    }
}

