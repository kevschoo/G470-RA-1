using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3Controller : MonoBehaviour
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
                    kPhysicsObject.useGlobalGravity = true;
                    Destroy(other.gameObject); 
                }
            }
        }
        if (other.gameObject.CompareTag("Spikes"))
        {
            this.transform.SetPositionAndRotation(OriginalTransform.position,OriginalTransform.rotation);

            
        }
    }
}
