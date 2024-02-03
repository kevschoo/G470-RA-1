using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject GateObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("GravityObject"))
        {
            KPhysicsObject kController = other.GetComponent<KPhysicsObject>();

            if (kController != null)
            {
                GateObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("GravityObject"))
        {
            KPhysicsObject kController = other.GetComponent<KPhysicsObject>();

            if (kController != null)
            {
                GateObject.SetActive(true);
            }
        }
    }
}
