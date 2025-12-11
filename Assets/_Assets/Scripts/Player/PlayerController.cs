using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CubeMovement))]
[RequireComponent(typeof(PlayerCollisionHandler))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CubeMovement _cubeMovement;

    public float BombReloadingTime => _bombReloadingTime;
    [SerializeField] private float _bombReloadingTime;

    [SerializeField] private BombControllerPool _bombPool;

    private bool _canPlaceBomb;
    private Coroutine _bombReloadingCoroutine;

    private Vector3 _startPosition;

    private bool _allowToMove;

    private void Start()
    {
        if (_cubeMovement == null)
        {
            _cubeMovement = GetComponent<CubeMovement>();
        }

        if(_bombPool == null)
        {
            _bombPool = GetComponent<BombControllerPool>();
        }

        _allowToMove = true;

        SubscribeToEvents();

        SaveStartTransform();
    }

    private void SubscribeToEvents()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        InputManager.Instance.OnTap += OnTap;
        InputManager.Instance.OnSwipe += OnSwipe;
        InputManager.Instance.OnMoveDirection += OnMoveDirection;

        GameManager.Instance.OnClassicModeEnter += OnClassicModeEnter;
        GameManager.Instance.OnCombatModeEnter += OnCombatModeEnter;
        GameManager.Instance.OnCombatModeExit += OnCombatModeExit;
        GameManager.Instance.OnPuzzleEnter += OnPuzzleEnter;
        GameManager.Instance.OnPuzzleExit += OnPuzzleExit;
    }

    private void UnsubscribeFromEvents()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        InputManager.Instance.OnTap -= OnTap;
        InputManager.Instance.OnSwipe -= OnSwipe;
        InputManager.Instance.OnMoveDirection -= OnMoveDirection;

        GameManager.Instance.OnClassicModeEnter -= OnClassicModeEnter;
        GameManager.Instance.OnCombatModeEnter -= OnCombatModeEnter;
        GameManager.Instance.OnCombatModeExit -= OnCombatModeExit;
        GameManager.Instance.OnPuzzleEnter -=OnPuzzleEnter;
        GameManager.Instance.OnPuzzleExit -=OnPuzzleExit;
    }

    private void SaveStartTransform()
    {
        _startPosition = transform.position;
    }

    private void OnTap()
    {
        TryMove(Vector3.forward);
    }

    private void OnSwipe(Vector2 direction)
    {
        var moveDirection = VectorHelper.VectorXYToVectorXZ(direction);

        if (moveDirection != Vector3.zero)
        {
            TryMove(moveDirection);
        }
    }

    private void OnMoveDirection(Vector3 direction)
    {
        var moveDirection = VectorHelper.VectorXYToVectorXZ(direction);

        if (moveDirection != Vector3.zero)
        {
            TryMove(moveDirection);
        }
    }

    private void TryMove(Vector3 direction)
    {
        if (!_allowToMove
            || _cubeMovement == null
            || _cubeMovement.IsMoving
            || direction == Vector3.zero)
        {
            return;
        }

        _cubeMovement.TryMove(direction);
    }

    public void PlaceBomb()
    {
        if(_canPlaceBomb)
        {
            _bombPool.GetObject(transform.position, Quaternion.identity);
            _bombReloadingCoroutine = StartCoroutine(BombReloading());
        }
    }

    private IEnumerator BombReloading()
    {
        _canPlaceBomb = false;
        yield return new WaitForSeconds(_bombReloadingTime);
        _canPlaceBomb = true;
    }

    private void OnClassicModeEnter()
    {
        _cubeMovement.StopMovement();
        transform.position = _startPosition;
    }

    private void OnCombatModeEnter()
    {
        _canPlaceBomb = true;
    }

    private void OnCombatModeExit()
    {
        _canPlaceBomb = false;

        if(_bombReloadingCoroutine != null)
        {
            StopCoroutine(_bombReloadingCoroutine);
        }

        _bombPool.ReturnAllObjects();
    }

    private void OnPuzzleEnter()
    {
        _allowToMove = false;
    }

    private void OnPuzzleExit()
    {
        _allowToMove = true;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
