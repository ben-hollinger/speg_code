using UnityEngine;

public class RockMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveDistance = 2f;   // how far left/right it travels from origin
    [SerializeField] private float _moveSpeed = 1.5f;    // full cycles per second

    private Vector3 _origin;

    private void Start()
    {
        _origin = transform.position;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * _moveSpeed * Mathf.PI * 2f) * _moveDistance;
        transform.position = _origin + transform.right * offset;
    }
}
