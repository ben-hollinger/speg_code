using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private float _speed = 5f;

    private Vector3 _velocity;
    private float _timeAlive;

    public void Initialize(Vector3 direction, int damage)
    {
        _velocity = direction.normalized * _speed;
        _damage = damage;
        _timeAlive = 0f;
    }

    private void Update()
    {
        transform.position += _velocity * Time.deltaTime;

        _timeAlive += Time.deltaTime;
        if (_timeAlive >= _lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO: Handle enemy vs player layer detection
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !damageable.IsDead)
        {
            damageable.TakeDamage(_damage);
        }

        Destroy(gameObject);
    }
}

