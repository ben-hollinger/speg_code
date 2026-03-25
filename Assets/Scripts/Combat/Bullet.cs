using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private float _speed = 5f;

    private Vector3 _velocity;
    private float _timeAlive;
    private bool _isPlayerBullet;
    private Transform _owner;

    public void Initialize(Vector3 direction, int damage, bool isPlayerBullet)
    {
        Initialize(direction, damage, isPlayerBullet, owner: null);
    }

    public void Initialize(Vector3 direction, int damage, bool isPlayerBullet, Transform owner)
    {
        _velocity = direction.normalized * _speed;
        _damage = damage;
        _timeAlive = 0f;
        _isPlayerBullet = isPlayerBullet;
        _owner = owner;
    }

    private void Update()
    {
        transform.position += _velocity * Time.deltaTime;

        _timeAlive += Time.deltaTime;
        if (_timeAlive >= _lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the bullet was spawned inside the shooter, ignore that collision so
        // the bullet can exit and travel normally.
        if (_owner != null && other.transform.IsChildOf(_owner))
            return;

        int hitLayer = other.gameObject.layer;
        int groundLayer = LayerMask.NameToLayer("Ground");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");

        // Colliding with ground/walls destroys the bullet.
        if (hitLayer == groundLayer)
        {
            Destroy(gameObject);
            return;
        }

        if (_isPlayerBullet)
        {
            // Player bullets damage enemies only.
            if (hitLayer == playerLayer) { Destroy(gameObject); return; } // self-hit (no owner match)
            if (hitLayer != enemyLayer) return;
        }
        else
        {
            // Enemy bullets damage player only.
            if (hitLayer == enemyLayer) { Destroy(gameObject); return; } // enemy hit (not player)
            if (hitLayer != playerLayer) return;
        }

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null && !damageable.IsDead)
            damageable.TakeDamage(_damage);

        Destroy(gameObject);
    }
}
