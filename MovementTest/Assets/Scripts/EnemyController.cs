using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public KPhysicsObject kPhysicsObject;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            GravityProjectile projectileScript = other.GetComponent<GravityProjectile>();

            if (projectileScript != null)
            {
                if(projectileScript.Parent == this.gameObject)
                {
                    Debug.Log("Hit By self");

                }
                else
                {
                    Debug.Log("Hit By Enemy Projectile");
                    InvertGravity();
                }
            }
        }
       
    }

    void InvertGravity()
    {
        if (kPhysicsObject != null)
        {
            Vector2 currentGravityDirection = kPhysicsObject.localGravityDirection;
            Vector2 invertedGravityDirection = -currentGravityDirection;
            
            if(kPhysicsObject.useGlobalGravity)
            {
                kPhysicsObject.useGlobalGravity = false;
            }
            kPhysicsObject.localGravityDirection = invertedGravityDirection;
        }
    }


}
