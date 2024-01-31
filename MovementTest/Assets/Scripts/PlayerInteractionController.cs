using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{



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
                    Debug.Log("Hit By Projectile");
            }
        }


    }
}
