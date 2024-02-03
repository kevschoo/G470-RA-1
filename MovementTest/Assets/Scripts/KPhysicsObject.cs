using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KPhysicsObject : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D rb;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float maxVelocity = 10f; 
    private Vector2 storedVelocity;
    private float storedAngularVelocity;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer; 
    public bool isGrounded;
    public Transform groundCheckLeft;
    public Transform groundCheckMid;
    public Transform groundCheckRight;

    public bool useGlobalGravity = true;
    public Vector2 localGravityDirection = Vector2.down; 
    public float localGravityStrength = 9.81f; 

    public float rotationSpeed = 360f;
    public bool useGravityRotation = true;
    public float moveDirection = 0;
    
    public bool inBulletTimeMode = false;
    public bool inPausedTimeMode = false;

    void Awake()
    {
        rb.gravityScale = 0;
    }


    public virtual void Jump()
    {
        if (isGrounded)
        {
            Debug.Log("Gravity Controller Jump");
            Vector2 jumpDirection = -localGravityDirection.normalized;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }
    }

    protected virtual void OnEnable()
    {
        if (KPhysics.instance == null) 
        {Debug.LogError("KPhysics.instance is null on OnEnable.");} 
        else 
        {Debug.Log("Registering to KPhysics events.");}

        KPhysics.instance.onGravityChange.AddListener(HandleGravityChange);
        KPhysics.instance.onBulletTimeStart.AddListener(HandleBulletTimeStart);
        KPhysics.instance.onBulletTimeEnd.AddListener(HandleBulletTimeEnd);
        KPhysics.instance.onPause.AddListener(HandlePausedTimeStart);
        KPhysics.instance.onUnpause.AddListener(HandlePausedTimeEnd);
    }

    protected virtual void HandlePausedTimeEnd(GameObject initiator)
    {
        rb.velocity = storedVelocity;
        rb.angularVelocity = storedAngularVelocity;
        this.inPausedTimeMode = false;
    }

    protected virtual void HandlePausedTimeStart(GameObject initiator, float duration)
    {
        storedVelocity = rb.velocity;
        storedAngularVelocity = rb.angularVelocity;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        this.inPausedTimeMode = true;
    }

    protected virtual void HandleBulletTimeEnd(GameObject initiator)
    {
        this.inBulletTimeMode = false;
    }

    protected virtual void HandleBulletTimeStart(GameObject initiator, float slowMotionFactor, float duration)
    {
        this.inBulletTimeMode = true;
    }

    void OnDisable()
    {
        if (KPhysics.instance != null)
            KPhysics.instance.onGravityChange.RemoveListener(HandleGravityChange);
    }

    public virtual  void HandleGravityChange(GameObject initiator, Vector2 newGravityDirection)
    {
        if(useGlobalGravity)
            localGravityDirection = newGravityDirection.normalized;
    }

    protected virtual void Update()
    {
        if (inPausedTimeMode)
        {
            UpdatePaused();
        }
        else if (inBulletTimeMode)
        {
            UpdateDuringBulletTime();
        }
        else
        {
            UpdateDuringNormalTime();
        }
    }

    protected virtual void UpdatePaused()
    {
        // Custom behavior during pause
    }

    protected virtual void UpdateDuringBulletTime()
    {
        // Custom behavior during bullet time
    }

    protected virtual void UpdateDuringNormalTime()
    {
        // Custom behavior during normal gameplay
    }


    protected virtual void FixedUpdate()
    {
        if (inPausedTimeMode)
        {
            FixedUpdatePaused();
        }
        else if (inBulletTimeMode)
        {
            FixedUpdateDuringBulletTime();
        }
        else
        {
            FixedUpdateDuringNormalTime();
        }

        
    }

    protected virtual void FixedUpdatePaused()
    {
        // Custom behavior during pause
    }

    protected virtual void FixedUpdateDuringBulletTime()
    {
        CalculateGravityEffects();
    }

    protected virtual void FixedUpdateDuringNormalTime()
    {
        CalculateGravityEffects();
    }

    protected virtual void CalculateGravityEffects()
    {
        Vector2 gravityDirection = useGlobalGravity ? KPhysics.instance.gravityVector : this.localGravityDirection;
        float gravityStrength = useGlobalGravity ? KPhysics.instance.gravityStrength : this.localGravityStrength;
        ApplyGravity(gravityDirection, gravityStrength, moveDirection);
        CheckGrounded(gravityDirection);
        
        if(useGravityRotation)
            RotateToGravity(gravityDirection);
    }

    protected virtual void ApplyGravity(Vector2 gravityDirection, float gravityStrength, float moveDir)
    {
        Vector2 gravityAcceleration = gravityDirection * gravityStrength * Time.fixedDeltaTime;
        Vector2 newVelocity = rb.velocity + gravityAcceleration;

        if (newVelocity.magnitude > maxVelocity)
        {
            newVelocity = newVelocity.normalized * maxVelocity;
        }

        rb.velocity = newVelocity;

        float move = moveDir;
        if (gravityDirection == Vector2.up)
        {
            move = -move;
        }

        Vector2 moveDirection = new Vector2(-gravityDirection.y, gravityDirection.x).normalized * move * moveSpeed;

        if (Mathf.Abs(gravityDirection.x) < 1) 
        {
            rb.velocity = new Vector2(moveDirection.x, rb.velocity.y);
        }
        else if (Mathf.Abs(gravityDirection.y) < 1) 
        {
            rb.velocity = new Vector2(rb.velocity.x, moveDirection.y);
        }

        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    protected virtual void RotateToGravity(Vector2 gravityDirection)
    {
        float targetRotationAngle = Vector2.SignedAngle(Vector2.down, gravityDirection);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetRotationAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    protected virtual void CheckGrounded(Vector2 gravityDirection)
    {
        Vector2 rayDirection = gravityDirection;
        isGrounded = Physics2D.Raycast(groundCheckLeft.position, rayDirection, groundCheckDistance, groundLayer) ||
                     Physics2D.Raycast(groundCheckMid.position, rayDirection, groundCheckDistance, groundLayer)  ||
                     Physics2D.Raycast(groundCheckRight.position, rayDirection, groundCheckDistance, groundLayer);
    }
    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 rayDirection = useGlobalGravity ? Physics2D.gravity.normalized : localGravityDirection;

        DrawGroundCheckRay(groundCheckLeft.position, rayDirection);
        DrawGroundCheckRay(groundCheckMid.position, rayDirection);
        DrawGroundCheckRay(groundCheckRight.position, rayDirection);
    }

    void DrawGroundCheckRay(Vector3 startPosition, Vector2 rayDirection)
    {
        Gizmos.DrawLine(startPosition, startPosition + (Vector3)rayDirection * groundCheckDistance);
    }
}
