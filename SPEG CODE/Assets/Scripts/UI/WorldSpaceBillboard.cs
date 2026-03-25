using UnityEngine;

public class WorldSpaceBillboard : MonoBehaviour
{
    private Camera _targetCamera;

    private void Start()
    {
        _targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _targetCamera.transform.rotation * Vector3.forward, _targetCamera.transform.rotation * Vector3.up);
    }
}
