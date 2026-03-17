using UnityEngine;

public class BulletEmitter : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletPrefab;

    private void FireStraight(BulletPatternData pattern)
    {
        Vector3 forward = transform.forward;
        int count = Mathf.Max(1, pattern.BulletCount);

        for (int i = 0; i < count; i++)
        {
            SpawnBullet(_firePoint.position, forward, pattern);
        }
    }

    private void FireSpread(BulletPatternData pattern)
    {
        Vector3 forward = transform.forward;
        int count = Mathf.Max(1, pattern.BulletCount);
        float spread = pattern.SpreadAngle;

        if (count == 1 || spread <= 0f)
        {
            SpawnBullet(_firePoint.position, forward, pattern);
            return;
        }

        float step = spread / (count - 1);
        float startAngle = -spread * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + step * i + pattern.AngleOffset;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rotation * forward;
            SpawnBullet(_firePoint.position, dir, pattern);
        }
    }

    private void FireCircle(BulletPatternData pattern)
    {
        int count = Mathf.Max(1, pattern.BulletCount);
        Vector3 forward = transform.forward;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i + pattern.AngleOffset;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rotation * forward;
            SpawnBullet(_firePoint.position, dir, pattern);
        }
    }

    private void SpawnBullet(Vector3 position, Vector3 direction, BulletPatternData pattern)
    {
        if (_bulletPrefab == null)
        {
            return;
        }

        GameObject bulletObject = Instantiate(_bulletPrefab, position, Quaternion.LookRotation(direction));
        var bullet = bulletObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            const float bulletSpeed = 5f;
            bullet.Initialize(direction, bulletSpeed, pattern.Damage, pattern.Lifetime);
        }
    }
}
