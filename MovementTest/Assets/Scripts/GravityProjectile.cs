using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityProjectile : MonoBehaviour
{
    public GameObject Parent;
    public float speed = 10f; 

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 moveDirection = transform.right; 
            rb.velocity = moveDirection * speed;
        }
        Destroy(this.gameObject, 3);
    }

    void FixedUpdate()
    {
        
    }

}