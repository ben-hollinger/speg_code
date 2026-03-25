using UnityEngine;

[CreateAssetMenu(fileName = "BulletPattern", menuName = "Data/Bullet Pattern")]
public class BulletPatternData : ScriptableObject
{
    [Header("ID")]
    [SerializeField] private string _patternName;
    [SerializeField] private float _patternWeight = 1f;

    [Header("Pattern")]
    [SerializeField] private int _bulletCount = 1;
    [SerializeField] private int _damage = 1;
    [Tooltip("For straight patterns with BulletCount > 1: spacing between bullets on a line.")]
    [SerializeField] private float _lineSpacing = 0.25f;
    [SerializeField] private float _spreadAngle = 30f;
    [SerializeField] private float _radius = 1f;
    [SerializeField] private float _angleOffset = 0f;

    public string PatternName => _patternName;
    public float PatternWeight => _patternWeight;
    public int BulletCount => _bulletCount;
    public int Damage => _damage;
    public float LineSpacing => _lineSpacing;
    public float SpreadAngle => _spreadAngle;
    public float Radius => _radius;
    public float AngleOffset => _angleOffset;
}

