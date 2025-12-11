using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CubeMovement))]
public class EnemyController : MonoBehaviour, IPoolable
{
    [HideInInspector] public bool _isCombat = false;

    private Transform _player;

    private ObjectPool<EnemyController> _parentPool;

    private CubeMovement _cubeMovement;

    private float _stepDelay = 0.3f;
    private Vector3 _moveDirection;

    private float _nextMoveTime;


    private void Awake()
    {
        _cubeMovement = GetComponent<CubeMovement>();
    }

    private void Start()
    {
        if(_isCombat)
        {
            _player = GameManager.Instance.PlayerController.transform;
        }
    }

    private void Update()
    {
        if (_cubeMovement == null || _cubeMovement.IsMoving)
        {
            return;
        }

        if (_isCombat)
        {
            CalculateDirection();
        }

        if (Time.time >= _nextMoveTime)
        {
            MoveStep();
        }
    }

    private void MoveStep()
    {
        if (_cubeMovement.TryMove(_moveDirection))
        {
            _nextMoveTime = Time.time + _stepDelay + _cubeMovement.MoveDuration;
        }
    }

    private void CalculateDirection()
    {
        if(_player.position.x < Mathf.Round(transform.position.x))
        {
            _moveDirection = Vector3.left;
        }
        else if(_player.position.x > Mathf.Round(transform.position.x))
        {
            _moveDirection = Vector3.right;
        }
        else if(_player.position.z < Mathf.Round(transform.position.z))
        {
            _moveDirection = Vector3.back;
        }
        else if(_player.position.z > Mathf.Round(transform.position.z))
        {
            _moveDirection = Vector3.forward;
        }
    }

    public void OnSpawnFromPool()
    {
        _nextMoveTime = Time.time + _stepDelay;
    }

    public void OnReturnToPool()
    {
        if (_cubeMovement != null)
        {
            _cubeMovement.ForceStop();
        }
    }

    public void SetPoolReference<T>(ObjectPool<T> pool) where T : Component, IPoolable
    {
        _parentPool = pool as ObjectPool<EnemyController>;
    }

    public void Initialize(float stepDelay, Vector3 moveDirection)
    {
        _stepDelay = stepDelay;
        _moveDirection = moveDirection;
    }

    public void Initialize(bool isCombat)
    {
        _isCombat = isCombat;
    }

    public void ReturnToPool()
    {
        if (_parentPool != null)
        {
            _parentPool.ReturnObject(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ReverseDirection()
    {
        _cubeMovement.ReverseCurrentAnimation();

        _moveDirection = -_moveDirection;
    }
}