using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedTimeField : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovementController playerController = other.GetComponent<PlayerMovementController>();

            if (playerController != null)
            {
                KPhysics.instance.PauseGame(other.gameObject, float.MaxValue); 
                playerController.inPausedTimeMode = true;
                playerController.CanMoveInPausedTime = true; 
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
                KPhysics.instance.UnpauseGame(other.gameObject);
                playerController.inPausedTimeMode = false;
                playerController.CanMoveInPausedTime = false;
            }
        }
    }
}