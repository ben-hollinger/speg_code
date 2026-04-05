using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private AudioClip[] _damageGruntClips;

    private int _currentHealth;
    private bool _isDead;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public bool IsDead => _isDead;

    // Events used by StatsUI and DeathScreenUI
    public event Action<int, int> HealthChanged;  // (current, max)
    public event Action           PlayerDied;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead || amount <= 0) return;
        _currentHealth = Mathf.Max(0, _currentHealth - amount);

        if (AudioManager.Instance != null && _damageGruntClips != null && _damageGruntClips.Length > 0)
            AudioManager.Instance.PlaySfx(
                _damageGruntClips[UnityEngine.Random.Range(0, _damageGruntClips.Length)]);

        HealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        if (_isDead || amount <= 0) return;
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        HealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void ResetStats()
    {
        _isDead = false;
        _currentHealth = _maxHealth;
        HealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    private void Die()
    {
        _isDead = true;
        PlayerDied?.Invoke();
    }
}
