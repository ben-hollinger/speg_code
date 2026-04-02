using UnityEngine;

/// <summary>
/// Attach to the magic blast prefab.
/// The prefab should have:
///   - A Rigidbody (Use Gravity = false, Collision Detection = Continuous)
///   - A Collider (Is Trigger = true) sized to taste (~0.2 radius sphere)
///   - A Particle System child for the blue core glow  (assign to _coreParticles)
///   - A Particle System child for the fire trail       (assign to _trailParticles)
///   - Optionally an AudioSource for an impact clip
/// </summary>
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
    /// <summary>
    /// Optional separate prefab that is spawned at the hit point (e.g. a burst
    /// of blue fire). Leave null to skip.
    /// </summary>
    [SerializeField] private GameObject _impactVfxPrefab;
    [SerializeField] private float _impactVfxLifetime = 1.5f;

    [Header("SFX")]
    [SerializeField] private AudioClip _impactClip;

    private Rigidbody _rb;
    private bool _hasHit;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.linearDamping = 0f;
    }

    private void Start()
    {
        // Launch straight forward at constant speed.
        _rb.linearVelocity = transform.forward * _speed;

        // Auto-destroy if it never hits anything.
        Destroy(gameObject, _lifetime);
    }

    // Called by MagicBlastShooter after Instantiate so the shooter can configure
    // damage from its own inspector without needing a reference to this prefab's fields.
    public void Initialise(int damage, float speed)
    {
        _damage = damage;
        _speed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        // Ignore the player's own colliders.
        if (other.GetComponentInParent<PlayerController>() != null) return;

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null && !damageable.IsDead)
        {
            damageable.TakeDamage(_damage);
        }

        HandleImpact(other.ClosestPoint(transform.position));
    }

    private void HandleImpact(Vector3 hitPoint)
    {
        _hasHit = true;

        // Stop moving.
        _rb.linearVelocity = Vector3.zero;
        _rb.isKinematic = true;

        // Stop emitting particles so they fade out naturally.
        StopParticleEmission(_coreParticles);
        StopParticleEmission(_trailParticles);

        // Spawn impact burst.
        if (_impactVfxPrefab != null)
        {
            var burst = Instantiate(_impactVfxPrefab, hitPoint, Quaternion.identity);
            Destroy(burst, _impactVfxLifetime);
        }

        // Play impact sound through AudioManager if available.
        if (_impactClip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(_impactClip);

        // Destroy the projectile itself once the longest-living particle has faded.
        // We wait a short moment so the trail particles aren't just popped out of existence.
        Destroy(gameObject, 0.8f);
    }

    private static void StopParticleEmission(ParticleSystem ps)
    {
        if (ps == null) return;
        var emission = ps.emission;
        emission.enabled = false;
    }
}