using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : MonoBehaviour
{
    public GravityDirection fieldDirection;
    public bool affectsGlobalGravity;
    public bool affectsPlayerGravity;
    public bool affectsGravityObjects = true;

    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                Vector2 gravityVector = WorldPhysicsEvents.instance.GetGravityVector(fieldDirection);

                if (affectsGlobalGravity)
                {
                    WorldPhysicsEvents.instance?.FlipGravity(gravityVector);
                }
                else if (affectsPlayerGravity)
                {
                    playerController.useGlobalGravity = false;
                    playerController.playerGravityDirection = gravityVector;
                }
            }
        }
        else if (other.gameObject.CompareTag("GravityObject"))
        {
            GravObjectController gravController = other.GetComponent<GravObjectController>();

            if (gravController != null)
            {
                Vector2 gravityVector = WorldPhysicsEvents.instance.GetGravityVector(fieldDirection);

                if (affectsGravityObjects)
                {
                    gravController.useGlobalGravity = false;
                    gravController.gravityDirection = gravityVector;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && affectsPlayerGravity)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.useGlobalGravity = true;
                playerController.HandleGravityChange(WorldPhysicsEvents.instance.gravityVector);
            }
        }
        else if (other.gameObject.CompareTag("GravityObject"))
        {
            GravObjectController gravController = other.GetComponent<GravObjectController>();

            if (gravController != null)
            {
                gravController.useGlobalGravity = true;
            }
        }
    }
}