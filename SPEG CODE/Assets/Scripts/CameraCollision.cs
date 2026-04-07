using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _collisionOffset = 0.3f;
    [SerializeField] private LayerMask _collisionMask = ~0;

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 direction = transform.position - _target.position;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(_target.position, direction.normalized, out hit, distance, _collisionMask, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point - direction.normalized * _collisionOffset;
        }
    }
}