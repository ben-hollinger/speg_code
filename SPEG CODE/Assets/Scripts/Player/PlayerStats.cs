using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private AudioClip[] _damageGruntClips;

    private int _currentHealth;
    private bool _isDead;
    public delegate void HealthChangedHandler(int currentHealth, int maxHealth);
    public event HealthChangedHandler HealthChanged;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public bool IsDead => _isDead;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        NotifyHealthChanged();
    }

    public void TakeDamage(int amount)
    {
        if (_isDead || amount <= 0) return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        NotifyHealthChanged();

        AudioManager.Instance.PlaySfx(_damageGruntClips[Random.Range(0, _damageGruntClips.Length)]);

        if (_currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (_isDead || amount <= 0) return;

        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        NotifyHealthChanged();
    }

    public void ResetStats()
    {
        _isDead = false;
        _currentHealth = _maxHealth;
        NotifyHealthChanged();
    }

    private void Die()
    {
        _isDead = true;
    }

    private void NotifyHealthChanged()
    {
        HealthChanged?.Invoke(_currentHealth, _maxHealth);
    }
}
