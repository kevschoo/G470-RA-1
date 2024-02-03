using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class KPhysics : MonoBehaviour
{
    
    public static KPhysics instance;
    public UnityEvent<GameObject, Vector2> onGravityChange;
    public UnityEvent<GameObject, float> onPause;
    public UnityEvent<GameObject> onUnpause;
    public UnityEvent<GameObject, float, float> onBulletTimeStart;
    public UnityEvent<GameObject> onBulletTimeEnd;
    public UnityEvent onTruePause;
    public UnityEvent onTrueUnpause;

    [field: SerializeField] public float OriginalTimeScale
    {
        get { return _originalTimeScale; }
        set 
        {
            if (value < 0)
            {
                Debug.LogWarning("TimeManager: Negative time scale is not allowed. Time scale is unchanged.");
                return; 
            }
            _originalTimeScale = value; 
        }
    }
    [SerializeField] private float _originalTimeScale = 1f;
    
    [SerializeField] private bool _isBulletTimeActive = false;

    [field: SerializeField] public float BulletTimeScale
    {
        get { return _bulletTimeScale; }
        set 
        {
            if (value <= 0 || value >= 1)
            {
                Debug.LogWarning("TimeManager: Bullet time scale must be greater than 0 and less than 1. Value is unchanged.");
                return;
            }
            _bulletTimeScale = value;
        }
    }
    [SerializeField] private float _bulletTimeScale = 0.1f;
    [SerializeField] private Coroutine bulletTimeCoroutine = null;
    [SerializeField] private static bool _isGamePaused = false;
    [SerializeField] private static bool _isGameTruePaused = false;

    [SerializeField] public Vector2 gravityVector {get; private set;} = Vector2.down;
    [field: SerializeField] public float gravityStrength {get; private set;} = 9.81f;
    [SerializeField] public static List<Vector2> gravityDirections = new List<Vector2> { Vector2.down, Vector2.right, Vector2.up, Vector2.left };
    [SerializeField] public int currentGravityIndex = 0;


    public void CycleGlobalGravity(GameObject initiator, int direction)
    {
        currentGravityIndex += direction;
        if (currentGravityIndex >= gravityDirections.Count) currentGravityIndex = 0;
        if (currentGravityIndex < 0) currentGravityIndex = gravityDirections.Count - 1;

        Vector2 newGravityDirection = gravityDirections[currentGravityIndex];
        FlipGravity(initiator, newGravityDirection);
    }

    public void FlipGravity(GameObject initiator, Vector2 newGravityDirection)
    {
        gravityVector = newGravityDirection.normalized; // Update gravity vector
        onGravityChange?.Invoke(initiator, gravityVector);
    }

    public static Vector2 GetGravityVector(GravityDirection direction)
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

    private void Awake()
    {
        Debug.Log("KPhysics Awake called.");
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
            onGravityChange = new UnityEvent<GameObject, Vector2>();

        if (onPause == null)
            onPause = new UnityEvent<GameObject, float>();

        if (onUnpause == null)
            onUnpause = new UnityEvent<GameObject>();

        if (onBulletTimeStart == null)
            onBulletTimeStart = new UnityEvent<GameObject, float,float>();

        if (onBulletTimeEnd == null)
            onBulletTimeEnd = new UnityEvent<GameObject>();

        if (onTruePause == null)
            onTruePause = new UnityEvent();

        if (onTrueUnpause == null)
            onTrueUnpause = new UnityEvent();
            
    }

    public void TruePauseGame()
    {
        if (!_isGameTruePaused)
        {
            _isGameTruePaused = true;
            Time.timeScale = 0;
            onTruePause?.Invoke();
        }
    }

    public void TrueUnpauseGame()
    {
        if (_isGameTruePaused)
        {
            _isGameTruePaused = false;
            Time.timeScale = _isBulletTimeActive ? _bulletTimeScale : _originalTimeScale;
            onTrueUnpause?.Invoke();
        }
    }
    public void PauseGame(GameObject initiator, float duration = 10f)
    {
        if (!_isGamePaused)
        {
            _isGamePaused = true;
            onPause?.Invoke(initiator, duration);
        }
    }

    public void UnpauseGame(GameObject initiator)
    {
        if (_isGamePaused)
        {
            _isGamePaused = false;
            onUnpause?.Invoke(initiator);
        }
    }

    public void StartBulletTime(GameObject initiator = null, float slowMotionFactor = .1f, float duration = 10f)
    {
        if (!_isGamePaused && Time.timeScale != 0 && !_isBulletTimeActive)
        {
            _isBulletTimeActive = true;
            _originalTimeScale = Time.timeScale;
            _bulletTimeScale = slowMotionFactor;
            Time.timeScale = slowMotionFactor;
            onBulletTimeStart?.Invoke(initiator, slowMotionFactor, duration);

            bulletTimeCoroutine = StartCoroutine(BulletTimeDuration(initiator, duration));
        }
    }

    private IEnumerator BulletTimeDuration(GameObject initiator, float duration)
    {
        float waitDuration = duration;
        while (waitDuration > 0)
        {
            if (!_isGamePaused || !_isGameTruePaused)
            {
                yield return null;
                waitDuration -= Time.unscaledDeltaTime;
            }
            else
            {
                yield return new WaitWhile(() => _isGamePaused);
            }
        }
        EndBulletTime(initiator);
    }

    public void EndBulletTime(GameObject initiator = null)
    {
        if (_isBulletTimeActive)
        {
            Time.timeScale = _originalTimeScale;
            _isBulletTimeActive = false;
            onBulletTimeEnd?.Invoke(initiator);

            if (bulletTimeCoroutine != null)
            {
                StopCoroutine(bulletTimeCoroutine);
                bulletTimeCoroutine = null; 
            }
        }
    }

    public bool IsGamePaused()
    {
        return _isGamePaused;
    }



}

public enum GravityDirection
{
    Down,
    Right,
    Up,
    Left,
}