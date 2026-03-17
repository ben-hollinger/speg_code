using UnityEngine;

[CreateAssetMenu(fileName = "BulletPattern", menuName = "Data/Bullet Pattern")]
public class BulletPatternData : ScriptableObject
{
    [Header("ID")]
    [SerializeField] private string _patternName;
    [SerializeField] private float _patternWeight = 1f;

    [Header("Pattern")]
    [SerializeField] private int _bulletCount = 1;
    [SerializeField] private float _baseSpeed = 5f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private float _spreadAngle = 30f;
    [SerializeField] private float _radius = 1f;
    [SerializeField] private float _angleOffset = 0f;

    public string PatternName => _patternName;
    public float PatternWeight => _patternWeight;
    public int BulletCount => _bulletCount;
    public float BaseSpeed => _baseSpeed;
    public int Damage => _damage;
    public float Lifetime => _lifetime;
    public float SpreadAngle => _spreadAngle;
    public float Radius => _radius;
    public float AngleOffset => _angleOffset;
}

