using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : KPhysicsObject
{

    public bool CanMoveInPausedTime = false;
    public bool CanSwapGravity = false;
    public bool isFlipped = true;

    public override void Jump()
    {
        if (isGrounded)
        {
            Debug.Log("Gravity Controller Jump");
            Vector2 jumpDirection = -localGravityDirection.normalized;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }
    }

    protected override void OnEnable()
    {
        KPhysics.instance.onGravityChange.AddListener(HandleGravityChange);
        KPhysics.instance.onBulletTimeStart.AddListener(HandleBulletTimeStart);
        KPhysics.instance.onBulletTimeEnd.AddListener(HandleBulletTimeEnd);
        KPhysics.instance.onPause.AddListener(HandlePausedTimeStart);
        KPhysics.instance.onUnpause.AddListener(HandlePausedTimeEnd);
    }

    protected override void HandlePausedTimeEnd(GameObject initiator)
    {
        this.inPausedTimeMode = false;
    }

    protected override void HandlePausedTimeStart(GameObject initiator, float duration)
    {
        this.inPausedTimeMode = true;
    }

    protected override void HandleBulletTimeEnd(GameObject initiator)
    {
        this.inBulletTimeMode = false;
    }

    protected override void HandleBulletTimeStart(GameObject initiator, float slowMotionFactor, float duration)
    {
        this.inBulletTimeMode = true;
    }

    void OnDisable()
    {
        if (KPhysics.instance != null)
            KPhysics.instance.onGravityChange.RemoveListener(HandleGravityChange);
    }

    public override void HandleGravityChange(GameObject initiator, Vector2 newGravityDirection)
    {
        if(useGlobalGravity)
            localGravityDirection = newGravityDirection.normalized;
    }

    protected override void Update()
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

    protected override void UpdatePaused()
    {
        if(CanMoveInPausedTime)
        {
            TryJump();
            TryGravitySwap();
        }
    }

    protected override void UpdateDuringBulletTime()
    {
        TryJump();
        TryGravitySwap();
    }

    protected override void UpdateDuringNormalTime()
    {
        TryJump();
        TryGravitySwap();
    }

    private void TryJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump");

            Vector2 jumpDirection = -localGravityDirection.normalized;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void TryGravitySwap()
    {
        if(!CanSwapGravity){return;}

        if (Input.GetKeyDown(KeyCode.E))
        {
            KPhysics.instance?.CycleGlobalGravity(this.gameObject, 1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            KPhysics.instance?.CycleGlobalGravity(this.gameObject, -1);
        }
    }

    protected override void FixedUpdate()
    {

        moveDirection = Input.GetAxis("Horizontal");
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

    protected override void FixedUpdatePaused()
    {
        if(CanMoveInPausedTime)
            CalculateGravityEffects();
    }

    protected override void FixedUpdateDuringBulletTime()
    {
        CalculateGravityEffects();
    }

    protected override void FixedUpdateDuringNormalTime()
    {
        CalculateGravityEffects();
    }

    protected override void CalculateGravityEffects()
    {
        Vector2 gravityDirection = useGlobalGravity ? KPhysics.instance.gravityVector : localGravityDirection;
        float gravityStrength = useGlobalGravity ? KPhysics.instance.gravityStrength : localGravityStrength;
        ApplyGravity(gravityDirection, gravityStrength, moveDirection);
        CheckGrounded(gravityDirection);
        RotateToGravity(gravityDirection);

        HandleMovementAndSpriteFlip(gravityDirection);
    }

    protected override void ApplyGravity(Vector2 gravityDirection, float gravityStrength, float moveDir)
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

    void HandleMovementAndSpriteFlip(Vector2 gravityDirection)
    {
        float move = moveDirection;
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

    protected override void RotateToGravity(Vector2 gravityDirection)
    {
        float targetRotationAngle = Vector2.SignedAngle(Vector2.down, gravityDirection);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetRotationAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    protected override void CheckGrounded(Vector2 gravityDirection)
    {
        Vector2 rayDirection = gravityDirection;
        isGrounded = Physics2D.Raycast(groundCheckLeft.position, rayDirection, groundCheckDistance, groundLayer) ||
                     Physics2D.Raycast(groundCheckMid.position, rayDirection, groundCheckDistance, groundLayer) ||
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
