using UnityEngine;

public abstract class CharacterData : ScriptableObject
{
    public string characterName;
    public int maxHealth;
    public int baseAttack;
    public GameObject modelPrefab;
}
