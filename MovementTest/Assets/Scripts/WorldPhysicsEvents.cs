using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldPhysicsEvents : MonoBehaviour
{
    public static WorldPhysicsEvents instance;

    public UnityEvent<Vector2> onGravityChange;
    public static List<Vector2> gravityDirections = new List<Vector2> { Vector2.down, Vector2.right, Vector2.up, Vector2.left };
    public int currentGravityIndex = 0;
    public Vector2 gravityVector {get; private set;} = Vector2.down;
    public float gravityStrength {get; private set;} = 9.81f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (onGravityChange == null)
            onGravityChange = new UnityEvent<Vector2>();
    }


    public void CycleGlobalGravity(int direction)
    {
        currentGravityIndex += direction;
        if (currentGravityIndex >= gravityDirections.Count) currentGravityIndex = 0;
        if (currentGravityIndex < 0) currentGravityIndex = gravityDirections.Count - 1;

        Vector2 newGravityDirection = gravityDirections[currentGravityIndex];
        FlipGravity(newGravityDirection);
    }

    public void FlipGravity(Vector2 newGravityDirection)
    {
        gravityVector = newGravityDirection.normalized; // Update gravity vector
        onGravityChange.Invoke(gravityVector);
    }
    
    public Vector2 GetGravityVector(GravityDirection direction)
    {
        switch (direction)
        {
            case GravityDirection.Down: return Vector2.down;
            case GravityDirection.Right: return Vector2.right;
            case GravityDirection.Up: return Vector2.up;
            case GravityDirection.Left: return Vector2.left;
            default: return Vector2.down;
        }
    }
}
public enum GravityDirection
{
    Down,
    Right,
    Up,
    Left
}