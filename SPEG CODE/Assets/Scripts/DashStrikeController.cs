using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashStrikeController : MonoBehaviour
{
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float damageRadius = 2f;
    public int dashDamage = 20;
    public LayerMask enemyLayer;

    private bool isDashing = false;
    private Vector2 dashDirection;
    private float dashTime;

    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame && !isDashing)
        {
            TryDash();
        }
    }

    void TryDash()
    {
        isDashing = true;
        dashTime = 0f;
        dashDirection = new Vector2(1, 0);  // Assume player faces right (can be dynamic)
        // Could use the player's current facing direction
    }
    
    void DashMovement()
    {
        dashTime += Time.deltaTime;

        // Move player during the dash
        transform.Translate(dashDirection * dashSpeed * Time.deltaTime);

        // Check for enemies in the dash path
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, damageRadius, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            // Apply damage to enemies
            //enemy.GetComponent<EnemyHealth>().TakeDamage(dashDamage);
        }

        // Dash duration logic
        if (dashTime > dashDuration)
        {
            isDashing = false;
        }
    }

}
