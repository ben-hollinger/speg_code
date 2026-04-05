using UnityEngine;

public class DestructibleRock : MonoBehaviour
{
    [Header("XP")]
    [SerializeField] private float _xpReward = 25f;

    [Header("Optional VFX")]
    [SerializeField] private GameObject _destroyVfxPrefab;
    [SerializeField] private float _vfxLifetime = 1.5f;

    [Header("Optional SFX")]
    [SerializeField] private AudioClip _destroySfx;

    // Assigned automatically if this rock is registered with a RockTutorialTracker.
    // You don't need to set this manually.
    [HideInInspector] public RockTutorialTracker TutorialTracker;

    private bool _isDestroyed;

    private void OnTriggerEnter(Collider other)
    {
        if (_isDestroyed) return;

        // Only magic blast can destroy rocks.
        if (!other.CompareTag("MagicBlast")) return;

        DestroyRock();
    }

    private void DestroyRock()
    {
        _isDestroyed = true;

        // Award XP.
        if (XPBar.Instance != null)
            XPBar.Instance.AddXP(_xpReward);

        // Notify tutorial tracker if this rock is part of one.
        if (TutorialTracker != null)
            TutorialTracker.OnRockDestroyed();

        // Spawn destruction VFX.
        if (_destroyVfxPrefab != null)
        {
            var vfx = Instantiate(_destroyVfxPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, _vfxLifetime);
        }

        // Play destruction SFX.
        if (_destroySfx != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(_destroySfx);

        Destroy(gameObject);
    }
}