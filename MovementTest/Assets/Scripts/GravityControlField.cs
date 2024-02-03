using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControlField : MonoBehaviour
{
    public GravityDirection fieldDirection;
    public bool KeepPlayerGravityStationary = true;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovementController playerController = other.GetComponent<PlayerMovementController>();

            Vector2 gravityVector = KPhysics.GetGravityVector(fieldDirection);
            playerController.localGravityDirection = gravityVector;

            if (playerController != null)
            {
                playerController.CanSwapGravity = true;
                if(KeepPlayerGravityStationary)
                {
                    playerController.useGlobalGravity = false;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovementController playerController = other.GetComponent<PlayerMovementController>();
            Vector2 gravityVector = KPhysics.GetGravityVector(fieldDirection);
            if (playerController != null)
            {
                playerController.CanSwapGravity = false;
                playerController.useGlobalGravity = false;
                playerController.localGravityDirection = gravityVector;
            }
        }
    }
}