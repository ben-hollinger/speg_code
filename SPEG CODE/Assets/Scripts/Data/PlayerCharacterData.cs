using UnityEngine;

[CreateAssetMenu(menuName = "SPEG/Player Character")]
public class PlayerCharacterData : CharacterData
{
    public float moveSpeed = 4f;
    public float gravity = -9.8f;

    public int meleeDamage = 10;
    public float meleeRadius = 1.2f;
}
