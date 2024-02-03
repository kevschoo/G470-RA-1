using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeField : MonoBehaviour
{
    public float slowMotionFactor = 0.1f;
    public float duration = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovementController playerController = other.GetComponent<PlayerMovementController>();

            if (playerController != null)
            {
                KPhysics.instance.StartBulletTime(other.gameObject, slowMotionFactor, duration);
                playerController.inBulletTimeMode = true;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovementController playerController = other.GetComponent<PlayerMovementController>();

            if (playerController != null)
            {
                KPhysics.instance.EndBulletTime(other.gameObject);
                playerController.inBulletTimeMode = false;

            }
        }
    }
}