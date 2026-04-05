using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    static bool hasCheckpoint;
    static Vector3 position;
    static float rotation;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        position = transform.position;
        rotation = transform.eulerAngles.y;
        hasCheckpoint = true;
    }

    public static void Restore(Transform player)
    {
        if (!hasCheckpoint) return;
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.SetPositionAndRotation(position, Quaternion.Euler(0f, rotation, 0f));
        if (cc != null) cc.enabled = true;
    }
}
