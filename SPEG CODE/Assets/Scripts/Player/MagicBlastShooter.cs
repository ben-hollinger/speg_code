using UnityEngine;
using UnityEngine.InputSystem;

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

    private PlayerMovement _movement;
    private PlayerStats _stats;
    private GrappleController _grapple;
    private Animator _animator;

    private static readonly int GrappleShootTrigger = Animator.StringToHash("GrappleShoot");
    private static readonly int GrappleEndTrigger = Animator.StringToHash("GrappleEnd");

    private float _nextFireTime;

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
<<<<<<< HEAD
=======
        if (!HatSelector.IsActiveAbility(HatSelector.AbilityType.MagicBlast)) return;
>>>>>>> origin/main
        if (_grapple != null && _grapple.IsGrappling) return;

        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            TryShoot();
    }

    private void TryShoot()
    {
        if (Time.time < _nextFireTime) return;
        if (_blastPrefab == null)
        {
            Debug.LogWarning("MagicBlastShooter: no blast prefab assigned.");
            return;
        }

        _nextFireTime = Time.time + _cooldown;

        // Only trigger the animation here. The actual projectile is spawned
        // by FireGrapple() which is called by the animation event mid-swing.
        if (_animator != null)
        {
            _animator.SetTrigger(GrappleShootTrigger);
            StartCoroutine(ResetShootTrigger());
        }
    }

    // Called by the animation event on the Grapple Shoot clip.
    // This is the single place where the blast is spawned.
    private void FireGrapple()
    {
<<<<<<< HEAD
=======
        if (!HatSelector.IsActiveAbility(HatSelector.AbilityType.MagicBlast)) return;

>>>>>>> origin/main
        Vector3 origin = _spawnPoint != null ? _spawnPoint.position : transform.position + Vector3.up;
    
        // Use the animator's transform since that's what actually rotates with movement
        Vector3 aimDirection = _animator != null ? _animator.transform.forward : transform.forward;
        Quaternion facing = Quaternion.LookRotation(aimDirection);

        GameObject blastObject = Instantiate(_blastPrefab, origin, facing);
        var blastComponent = blastObject.GetComponent<MagicBlast>();
        if (blastComponent != null)
            blastComponent.Initialise(_damage, _projectileSpeed, transform.root);

        if (_shootSoundClip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(_shootSoundClip);
    }

    private System.Collections.IEnumerator ResetShootTrigger()
    {
        yield return null;
        _animator.ResetTrigger(GrappleShootTrigger);
        _animator.SetTrigger(GrappleEndTrigger);
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> origin/main
