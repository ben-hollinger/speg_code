using System.Collections;
using UnityEngine;

// ═════════════════════════════════════════════════════════════════════════════
// HAZARD SPAWNER
// ═════════════════════════════════════════════════════════════════════════════
public class HazardSpawner : MonoBehaviour
{
    [Header("Hazard Prefabs (assign in Inspector)")]
    public GameObject logPrefab;
    public GameObject firePrefab;

    [Header("Player Reference")]
    public Transform player;

    [Header("Spawn Area")]
    public Vector3 mapCentre = Vector3.zero;
    public float mapRadius = 50f;
    public float playerRadius = 8f;
    public float spawnHeight = 15f;
    public LayerMask groundLayer = ~0;

    [Header("Spawn Timing & Count")]
    public float spawnInterval = 2f;
    [Range(1, 20)] public int spawnCount = 6;
    [Range(0f, 1f)] public float playerConcentration = 0.7f;
    [Range(0f, 1f)] public float fireChance = 0.4f;

    [Header("Damage")]
    public float damagePerHit = 10f;
    public string playerTag = "Player";

    [Header("Hazard Lifetime")]
    public float lifetimeDuration = 4f;

    void Start()
    {
        if (logPrefab  == null) Debug.LogWarning("[HazardSpawner] Log prefab not assigned!");
        if (firePrefab == null) Debug.LogWarning("[HazardSpawner] Fire prefab not assigned!");
        if (player     == null) Debug.LogWarning("[HazardSpawner] Player not assigned!");

        if (player != null && !player.CompareTag(playerTag))
            Debug.LogWarning($"[HazardSpawner] Player tag is '{player.tag}' but " +
                             $"playerTag is '{playerTag}' — these must match!");

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            SpawnBatch();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBatch()
    {
        int nearPlayer = Mathf.RoundToInt(spawnCount * playerConcentration);
        int scattered  = spawnCount - nearPlayer;
        for (int i = 0; i < nearPlayer; i++) SpawnHazard(PointNearPlayer());
        for (int i = 0; i < scattered;  i++) SpawnHazard(PointOnMap());
    }

    void SpawnHazard(Vector2 xzPoint)
    {
        bool useFire = Random.value < fireChance;
        GameObject prefab = (useFire && firePrefab != null) ? firePrefab
                          : (logPrefab != null)             ? logPrefab
                          : null;
        if (prefab == null) return;

        float groundY    = FindGroundY(xzPoint);
        Vector3 spawnPos = new Vector3(xzPoint.x, groundY + spawnHeight, xzPoint.y);
        Quaternion rot   = Quaternion.Euler(Random.Range(-15f, 15f), Random.Range(0f, 360f), Random.Range(-15f, 15f));

        GameObject hazard = Instantiate(prefab, spawnPos, rot);

        Rigidbody rb = hazard.GetComponent<Rigidbody>();
        if (rb == null) rb = hazard.AddComponent<Rigidbody>();
        rb.useGravity  = true;
        rb.isKinematic = false;

        if (hazard.GetComponentInChildren<Collider>() == null)
            hazard.AddComponent<BoxCollider>();

        HazardDamage dmg = hazard.GetComponent<HazardDamage>();
        if (dmg == null) dmg = hazard.AddComponent<HazardDamage>();
        dmg.damage    = damagePerHit;
        dmg.playerTag = playerTag;
        dmg.lifetime  = lifetimeDuration;
    }

    float FindGroundY(Vector2 xz)
    {
        Ray ray = new Ray(new Vector3(xz.x, 500f, xz.y), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            return hit.point.y;
        return player != null ? player.position.y : 0f;
    }

    Vector2 PointNearPlayer()
    {
        Vector2 c = Random.insideUnitCircle * playerRadius;
        Vector3 o = player != null ? player.position : mapCentre;
        return new Vector2(o.x + c.x, o.z + c.y);
    }

    Vector2 PointOnMap()
    {
        Vector2 c = Random.insideUnitCircle * mapRadius;
        return new Vector2(mapCentre.x + c.x, mapCentre.z + c.y);
    }
}

// ═════════════════════════════════════════════════════════════════════════════
// HAZARD DAMAGE
// ═════════════════════════════════════════════════════════════════════════════
public class HazardDamage : MonoBehaviour
{
    [HideInInspector] public float  damage    = 10f;
    [HideInInspector] public string playerTag = "Player";
    [HideInInspector] public float  lifetime  = 4f;

    private bool _hasHit = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_hasHit) return;
        if (!collision.gameObject.CompareTag(playerTag)) return;
        _hasHit = true;
        ApplyDamage(collision.gameObject);
        Destroy(gameObject, 0.1f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;
        if (!other.CompareTag(playerTag)) return;
        _hasHit = true;
        ApplyDamage(other.gameObject);
        Destroy(gameObject, 0.1f);
    }

    void Update()
    {
        if (_hasHit) return;
        Collider[] nearby = Physics.OverlapSphere(transform.position, 1f);
        foreach (var col in nearby)
        {
            if (col.CompareTag(playerTag))
            {
                _hasHit = true;
                ApplyDamage(col.gameObject);
                Destroy(gameObject, 0.1f);
                return;
            }
        }
    }

    void ApplyDamage(GameObject playerObj)
    {
        // Check shield first — if active it blocks the hit entirely
        ShieldAbility shield = playerObj.GetComponent<ShieldAbility>();
        if (shield != null && shield.TryTakeDamage(damage))
        {
            Debug.Log("[HazardDamage] Blocked by shield.");
            return;
        }

        // Apply real damage to PlayerStats
        PlayerStats stats = playerObj.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamage(Mathf.RoundToInt(damage));
            Debug.Log($"[HazardDamage] Hit player for {Mathf.RoundToInt(damage)}. HP: {stats.CurrentHealth}/{stats.MaxHealth}");
        }
        else
        {
            Debug.LogWarning("[HazardDamage] PlayerStats component not found on player GameObject!");
        }
    }
}
