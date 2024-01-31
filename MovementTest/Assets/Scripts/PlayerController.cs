using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
    public Vector2 playerGravityDirection = Vector2.down; 
    public float playerGravityStrength = 9.81f; 

    public float rotationSpeed = 360f;
    public bool isFlipped = true;

    void Awake()
    {
        rb.gravityScale = 0;
    }

    void Update()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump");

            Vector2 jumpDirection = -playerGravityDirection.normalized;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            WorldPhysicsEvents.instance?.CycleGlobalGravity(1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            WorldPhysicsEvents.instance?.CycleGlobalGravity(-1);
        }


    }


    public void HandleGravityChange(Vector2 newGravityDirection)
    {
        if(useGlobalGravity)
            playerGravityDirection = newGravityDirection.normalized;
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


        Vector2 gravityDirection = useGlobalGravity ? WorldPhysicsEvents.instance.gravityVector : playerGravityDirection;
        float gravityStrength = useGlobalGravity ? WorldPhysicsEvents.instance.gravityStrength : playerGravityStrength;
        ApplyGravity(gravityDirection, gravityStrength);
        CheckGrounded(gravityDirection);
        RotatePlayerToGravity(gravityDirection);

        HandleMovementAndSpriteFlip(gravityDirection);
    }

    void ApplyGravity(Vector2 gravityDirection, float gravityStrength)
    {
        Vector2 gravityAcceleration = gravityDirection * gravityStrength * Time.fixedDeltaTime;
        rb.velocity += gravityAcceleration;

        float move = Input.GetAxis("Horizontal");

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

    private void RotatePlayerToGravity(Vector2 gravityDirection)
    {
        float targetRotationAngle = Vector2.SignedAngle(Vector2.down, gravityDirection);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetRotationAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void CheckGrounded(Vector2 gravityDirection)
    {
        Vector2 rayDirection = gravityDirection;
        isGrounded = Physics2D.Raycast(groundCheckLeft.position, rayDirection, groundCheckDistance, groundLayer) ||
                     Physics2D.Raycast(groundCheckMid.position, rayDirection, groundCheckDistance, groundLayer) ||
                     Physics2D.Raycast(groundCheckRight.position, rayDirection, groundCheckDistance, groundLayer);
    }

    void HandleMovementAndSpriteFlip(Vector2 gravityDirection)
    {
        float move = Input.GetAxis("Horizontal");
        if (gravityDirection == Vector2.up)
        {
            move = -move;
        }

        if (move > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isFlipped = false;
        }

        else if (move < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFlipped = true;
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 rayDirection = useGlobalGravity ? Physics2D.gravity.normalized : playerGravityDirection;

        DrawGroundCheckRay(groundCheckLeft.position, rayDirection);
        DrawGroundCheckRay(groundCheckMid.position, rayDirection);
        DrawGroundCheckRay(groundCheckRight.position, rayDirection);
    }

    void DrawGroundCheckRay(Vector3 startPosition, Vector2 rayDirection)
    {
        Gizmos.DrawLine(startPosition, startPosition + (Vector3)rayDirection * groundCheckDistance);
    }
}