using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy")]
    [SerializeField] private string _enemyName;
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private float _attackInterval = 2f;

    [Header("Assets")]
    [SerializeField] private GameObject _modelPrefab;
    [SerializeField] private AnimationClip _deathAnimation;
    [SerializeField] private AudioClip _deathSfx;

    public string EnemyName => _enemyName;
    public int MaxHealth => _maxHealth;
    public float AttackInterval => _attackInterval;
    public GameObject ModelPrefab => _modelPrefab;
    public AnimationClip DeathAnimation => _deathAnimation;
    public AudioClip DeathSfx => _deathSfx;
}

