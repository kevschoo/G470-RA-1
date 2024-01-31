using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravObjectController : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D rb;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer; 
    public bool isGrounded;

    public Transform groundCheckLeft;
    public Transform groundCheckMid;
    public Transform groundCheckRight;

    public bool useGlobalGravity = true;
    public Vector2 gravityDirection = Vector2.down; 
    public float gravityStrength = 9.81f; 

    public float rotationSpeed = 360f;
    public bool useGravityRotation = true;
    public float moveDirection = 0;

    void Awake()
    {
        rb.gravityScale = 0;
    }

    void Update()
    {

    }

    public void Jump()
    {
        if (isGrounded)
        {
            Debug.Log("Gravity Controller Jump");
            Vector2 jumpDirection = -gravityDirection.normalized;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void HandleGravityChange(Vector2 newGravityDirection)
    {
        if(useGlobalGravity)
            gravityDirection = newGravityDirection.normalized;
    }

    void OnEnable()
    {
        WorldPhysicsEvents.instance.onGravityChange.AddListener(HandleGravityChange);
    }

    void OnDisable()
    {
        if (WorldPhysicsEvents.instance != null)
            WorldPhysicsEvents.instance.onGravityChange.RemoveListener(HandleGravityChange);
    }

    void FixedUpdate()
    {


        Vector2 gravityDirection = useGlobalGravity ? WorldPhysicsEvents.instance.gravityVector : this.gravityDirection;
        float gravityStrength = useGlobalGravity ? WorldPhysicsEvents.instance.gravityStrength : this.gravityStrength;
        ApplyGravity(gravityDirection, gravityStrength, moveDirection);
        CheckGrounded(gravityDirection);
        
        if(useGravityRotation)
            RotateToGravity(gravityDirection);
    }

    void ApplyGravity(Vector2 gravityDirection, float gravityStrength, float moveDir)
    {
        Vector2 gravityAcceleration = gravityDirection * gravityStrength * Time.fixedDeltaTime;
        rb.velocity += gravityAcceleration;

        float move = moveDir;

        if (gravityDirection == Vector2.up)
        {
            move = -move;
        }

        Vector2 moveDirection = new Vector2(-gravityDirection.y, gravityDirection.x).normalized * move * moveSpeed;
        
        if (Mathf.Abs(gravityDirection.x) < 1) 
            rb.velocity = new Vector2(moveDirection.x, rb.velocity.y);
        else if (Mathf.Abs(gravityDirection.y) < 1) 
            rb.velocity = new Vector2(rb.velocity.x, moveDirection.y);
    }

    private void RotateToGravity(Vector2 gravityDirection)
    {
        float targetRotationAngle = Vector2.SignedAngle(Vector2.down, gravityDirection);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetRotationAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void CheckGrounded(Vector2 gravityDirection)
    {
        Vector2 rayDirection = gravityDirection;
        isGrounded = Physics2D.Raycast(groundCheckLeft.position, rayDirection, groundCheckDistance, groundLayer) ||
                     Physics2D.Raycast(groundCheckRight.position, rayDirection, groundCheckDistance, groundLayer);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 rayDirection = useGlobalGravity ? Physics2D.gravity.normalized : gravityDirection;

        DrawGroundCheckRay(groundCheckLeft.position, rayDirection);
        DrawGroundCheckRay(groundCheckMid.position, rayDirection);
        DrawGroundCheckRay(groundCheckRight.position, rayDirection);
    }

    void DrawGroundCheckRay(Vector3 startPosition, Vector2 rayDirection)
    {
        Gizmos.DrawLine(startPosition, startPosition + (Vector3)rayDirection * groundCheckDistance);
    }
}
