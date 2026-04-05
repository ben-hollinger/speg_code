public static class PlayerProgression
{
    public const int BaseMeleeDamage = 10;
    public const int BaseMaxHealth = 6;

    public static int MeleeDamageForLevel(int level)
    {
        if (level < 1) level = 1;
        return BaseMeleeDamage + (level - 1);
    }

    public static int MaxHealthForLevel(int level)
    {
        if (level < 1) level = 1;
        return BaseMaxHealth + (level - 1) / 2;
    }
}
