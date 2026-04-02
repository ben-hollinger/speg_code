using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attach to the Player GameObject alongside PlayerController.
///
/// Listens for right-click and fires a MagicBlast projectile from the
/// designated spawn point.  Designed to be self-contained so it can be
/// swapped in/out without touching PlayerController or GrappleController.
///
/// Setup checklist:
///   1. Create a MagicBlast prefab (see MagicBlast.cs header for requirements).
///   2. Assign the prefab to _blastPrefab.
///   3. Create an empty child Transform on the player at the hand/staff tip
///      and assign it to _spawnPoint.  If left null, shoots from the player root.
///   4. Tune _damage, _projectileSpeed and _cooldown as needed.
/// </summary>
public class MagicBlastShooter : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject _blastPrefab;
    [SerializeField] private Transform _spawnPoint;

    [Header("Stats")]
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _projectileSpeed = 18f;

    [Header("Cooldown")]
    [SerializeField] private float _cooldown = 0.4f;

    [Header("SFX")]
    [SerializeField] private AudioClip _shootSoundClip;

    // ── Cached references ────────────────────────────────────────────────────
    private PlayerMovement _movement;
    private PlayerStats _stats;
    private GrappleController _grapple;   // kept so we can skip firing while grappling
    private Animator _animator;

    // Animator parameter – reuses the same trigger your grapple animation already hooks into.
    // If you want a dedicated "MagicShoot" trigger, add it to the Animator and swap the hash here.
    private static readonly int GrappleShootTrigger = Animator.StringToHash("GrappleShoot");
    private static readonly int GrappleEndTrigger = Animator.StringToHash("GrappleEnd");

    private float _nextFireTime;

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _stats    = GetComponent<PlayerStats>();
        _grapple  = GetComponent<GrappleController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_stats == null || _stats.IsDead) return;
        if (_movement != null && !_movement.IsGrounded) return;

        // Skip while grapple is active (remove this guard once you decouple the two).
        if (_grapple != null && _grapple.IsGrappling) return;

        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            TryShoot();
    }

    
    private void FireGrapple() { } // absorbs the animation event from Grapple Shoot clip
    
    // ── Shooting ──────────────────────────────────────────────────────────────
    private void TryShoot()
    {
        if (Time.time < _nextFireTime) return;
        if (_blastPrefab == null)
        {
            Debug.LogWarning("MagicBlastShooter: no blast prefab assigned.");
            return;
        }

        _nextFireTime = Time.time + _cooldown;

        Vector3 origin    = _spawnPoint != null ? _spawnPoint.position : transform.position + Vector3.up;

        Quaternion facing = Quaternion.LookRotation(transform.forward);
        var blast = Instantiate(_blastPrefab, origin, facing);
        var blastComponent = blast.GetComponent<MagicBlast>();
        if (blastComponent != null)
            blastComponent.Initialise(_damage, _projectileSpeed);

        if (_shootSoundClip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(_shootSoundClip);

        if (_animator != null)
        {
            _animator.SetTrigger(GrappleShootTrigger);
            StartCoroutine(ResetShootTrigger());
        }
    }

    private System.Collections.IEnumerator ResetShootTrigger()
    {
        // Wait one frame for the animator to pick up the trigger, then clear it
        // and fire GrappleEnd to return the animator to its idle/locomotion state.
        yield return null;
        _animator.ResetTrigger(GrappleShootTrigger);
        _animator.SetTrigger(GrappleEndTrigger);
    }
}