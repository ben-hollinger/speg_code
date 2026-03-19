using UnityEngine;

public class BulletEmitter : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private AudioClip _attackSfxClip;

    private void FireStraight(BulletPatternData pattern)
    {
        PlayAttackSfx();
        Vector3 forward = transform.forward;
        int count = Mathf.Max(1, pattern.BulletCount);

        Vector3 right = transform.right;
        float spacing = Mathf.Max(0f, pattern.LineSpacing);
        float halfWidth = (count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float offset = i * spacing - halfWidth;
            Vector3 pos = _firePoint.position + right * offset;
            SpawnBullet(pos, forward, pattern);
        }
    }

    private void FireSpread(BulletPatternData pattern)
    {
        PlayAttackSfx();
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
        PlayAttackSfx();
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
        if (_bulletPrefab == null) return;

        GameObject bulletObject = Instantiate(_bulletPrefab, position, Quaternion.LookRotation(direction));
        var bullet = bulletObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(direction, pattern.Damage, isPlayerBullet: false, owner: transform.root);
        }
    }

    private void PlayAttackSfx()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlaySfx(_attackSfxClip);
    }
}
