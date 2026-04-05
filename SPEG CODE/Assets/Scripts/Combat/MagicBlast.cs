using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagicBlast : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _lifetime = 5f;

    [Header("Movement")]
    [SerializeField] private float _speed = 18f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem _coreParticles;
    [SerializeField] private ParticleSystem _trailParticles;

    [Header("Impact VFX")]
    [SerializeField] private GameObject _impactVfxPrefab;
    [SerializeField] private float _impactVfxLifetime = 1.5f;

    [Header("SFX")]
    [SerializeField] private AudioClip _impactClip;

    private Rigidbody _rb;
    private bool _hasHit;
    private Transform _owner;

    // Single Initialise — always pass the owner so the self-hit check works.
    public void Initialise(int damage, float speed, Transform owner)
    {
        _damage = damage;
        _speed = speed;
        _owner = owner;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.linearDamping = 0f;
    }

    private void Start()
    {
        _rb.linearVelocity = transform.forward * _speed;
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore any collider that belongs to the shooter.
        if (_owner != null && other.transform.IsChildOf(_owner)) return;
        if (_hasHit) return;

        int hitLayer = other.gameObject.layer;
        
        if (hitLayer == LayerMask.NameToLayer("TutorialBoundary")) return;

        // Ignore the player layer as a fallback safety check.
        if (hitLayer == LayerMask.NameToLayer("Player")) return;

        // Hitting ground/walls destroys the blast without dealing damage.
        if (hitLayer == LayerMask.NameToLayer("Ground"))
        {
            HandleImpact(other.ClosestPoint(transform.position));
            return;
        }

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null && !damageable.IsDead)
            damageable.TakeDamage(_damage);

        HandleImpact(other.ClosestPoint(transform.position));
    }

    private void HandleImpact(Vector3 hitPoint)
    {
        _hasHit = true;

        _rb.linearVelocity = Vector3.zero;
        _rb.isKinematic = true;

        StopParticleEmission(_coreParticles);
        StopParticleEmission(_trailParticles);

        if (_impactVfxPrefab != null)
        {
            var burst = Instantiate(_impactVfxPrefab, hitPoint, Quaternion.identity);
            Destroy(burst, _impactVfxLifetime);
        }

        if (_impactClip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(_impactClip);

        Destroy(gameObject, 0.8f);
    }

    private static void StopParticleEmission(ParticleSystem ps)
    {
        if (ps == null) return;
        var emission = ps.emission;
        emission.enabled = false;
    }
}