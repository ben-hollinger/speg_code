using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy")]
    [SerializeField] private string _enemyName;
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private int _XPReward = 10;
    [SerializeField] private float _attackInterval = 2f;

    [Header("Targeting")]
    [SerializeField] private float _aggroRadius = 9f;
    [SerializeField] private float _aggroHeight = 2f;
    [SerializeField] private float _turnSpeed = 360f;

    [Header("Assets")]
    [SerializeField] private GameObject _modelPrefab;
    [SerializeField] private AnimationClip _deathAnimation;
    [SerializeField] private AudioClip _deathSfx;

    public string EnemyName => _enemyName;
    public int MaxHealth => _maxHealth;
    public int XPReward => _XPReward;
    public float AttackInterval => _attackInterval;
    public float AggroRadius => _aggroRadius;
    public float AggroHeight => _aggroHeight;
    public float TurnSpeed => _turnSpeed;
    public GameObject ModelPrefab => _modelPrefab;
    public AnimationClip DeathAnimation => _deathAnimation;
    public AudioClip DeathSfx => _deathSfx;
}

