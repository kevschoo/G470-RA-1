using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileController : MonoBehaviour
{
    public PlayerController playerController;
    public Transform BulletSpawnPoint;
    public GravityProjectile projectilePrefab;
    public float projectileCooldown = 1f; 
    private float lastProjectileTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= lastProjectileTime + projectileCooldown)
        {
            FireProjectile();
            lastProjectileTime = Time.time;
        }
    }

    void FireProjectile()
    {
        Vector2 fireDirection = GetFireDirection();
        Quaternion projectileRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, fireDirection));
        GravityProjectile gravBullet = Instantiate(projectilePrefab, BulletSpawnPoint.transform.position, projectileRotation);
        gravBullet.Parent = this.gameObject;
    }

    Vector2 GetFireDirection()
    {
        if (playerController != null)
        {
            Vector2 gravityDirection = playerController.useGlobalGravity 
                ? WorldPhysicsEvents.instance.gravityVector
                : playerController.playerGravityDirection;

            Vector2 baseDirection = new Vector2(-gravityDirection.y, gravityDirection.x).normalized;

            if (playerController.isFlipped)
            {
                baseDirection = new Vector2(-baseDirection.x, baseDirection.y);
            }

            return baseDirection;
        }
        return Vector2.right; 
    }
}
