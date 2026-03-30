public interface IDamageable
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    bool IsDead { get; }

    void TakeDamage(int amount);
    void Heal(int amount);
}

