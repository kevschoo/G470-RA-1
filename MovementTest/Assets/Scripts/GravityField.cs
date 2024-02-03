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
            PlayerMovementController playerController = other.GetComponent<PlayerMovementController>();

            if (playerController != null)
            {
                Vector2 gravityVector = KPhysics.GetGravityVector(fieldDirection);

                if (affectsGlobalGravity)
                {
                    KPhysics.instance?.FlipGravity(other.gameObject, gravityVector);
                }
                else if (affectsPlayerGravity)
                {
                    playerController.useGlobalGravity = false;
                    playerController.localGravityDirection = gravityVector;
                }
            }
        }
        else if (other.gameObject.CompareTag("GravityObject"))
        {
            KPhysicsObject gravController = other.GetComponent<KPhysicsObject>();

            if (gravController != null)
            {
                Vector2 gravityVector = KPhysics.GetGravityVector(fieldDirection);

                if (affectsGravityObjects)
                {
                    gravController.useGlobalGravity = false;
                    gravController.localGravityDirection = gravityVector;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && affectsPlayerGravity)
        {
            PlayerMovementController playerController = other.GetComponent<PlayerMovementController>();

            if (playerController != null)
            {
                playerController.useGlobalGravity = true;
                playerController.HandleGravityChange(this.gameObject, KPhysics.instance.gravityVector);
            }
        }
        else if (other.gameObject.CompareTag("GravityObject"))
        {
            KPhysicsObject gravController = other.GetComponent<KPhysicsObject>();

            if (gravController != null)
            {
                gravController.useGlobalGravity = true;
            }
        }
    }
}