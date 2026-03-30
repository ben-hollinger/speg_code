public interface ICombatant : IDamageable
{
    string DisplayName { get; }

    int GetAttackPower();
    void OnCombatStart();
    void OnCombatEnd(bool won);
}

