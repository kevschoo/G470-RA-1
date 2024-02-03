using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileController : MonoBehaviour
{
    public PlayerMovementController playerController;
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

        if (playerController.isFlipped)
        {
            fireDirection *= -1;
        }

        Quaternion projectileRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, fireDirection));
        GravityProjectile gravBullet = Instantiate(projectilePrefab, BulletSpawnPoint.position, projectileRotation);
        gravBullet.Initialize(fireDirection, playerController.isFlipped);
    }

    Vector2 GetFireDirection()
    {
        Vector2 gravityDirection = playerController.useGlobalGravity 
            ? KPhysics.instance.gravityVector
            : playerController.localGravityDirection;

        Vector2 baseDirection = new Vector2(-gravityDirection.y, gravityDirection.x).normalized;

        return baseDirection;
    }
}
