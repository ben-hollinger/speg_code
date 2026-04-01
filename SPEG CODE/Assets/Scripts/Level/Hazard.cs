using UnityEngine;

public class Hazard : MonoBehaviour
{   
    private void Kill(Collider other)
    {
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null && !damageable.IsDead)
            damageable.TakeDamage(damageable.MaxHealth);
    }

    private void OnCollisionEnter(Collision collision) => Kill(collision.collider);
    private void OnTriggerEnter(Collider other) => Kill(other);
    public void HandleControllerHit(GameObject controllerGameObject) => Kill(controllerGameObject.GetComponent<Collider>());
}