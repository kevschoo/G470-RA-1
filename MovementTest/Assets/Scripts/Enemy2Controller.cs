using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
    public KPhysicsObject kPhysicsObject;
    public Transform OriginalTransform;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            GravityProjectile projectileScript = other.GetComponent<GravityProjectile>();
            Rigidbody2D projectileRb = other.GetComponent<Rigidbody2D>();

            if (projectileScript != null && projectileRb != null)
            {
                if(projectileScript.Parent == this.gameObject)
                {
                    Debug.Log("Hit By self");
                }
                else
                {
                    Debug.Log("Hit By Enemy Projectile");
                    ApplyProjectileVelocityToEnemy(projectileRb.velocity);
                    Destroy(other.gameObject); 
                }
            }
        }
        if (other.gameObject.CompareTag("Spikes"))
        {
            this.transform.SetPositionAndRotation(OriginalTransform.position,OriginalTransform.rotation);

            
        }
    }

    void ApplyProjectileVelocityToEnemy(Vector2 velocity)
    {
        if (kPhysicsObject != null && kPhysicsObject.rb != null)
        {
            kPhysicsObject.rb.velocity = velocity;
        }
    }
}
