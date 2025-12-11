using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Swipe Settings")]
    [SerializeField] private float _swipeThreshold = 50f;
    [SerializeField] private float _tapMaxDuration = 0.3f;
    [SerializeField] private float _axisBias = 0.7f;

    public event Action OnTap;
    public event Action<Vector2> OnSwipe;
    public event Action<Vector2> OnRawSwipe;
    public event Action<Vector3> OnMoveDirection;

    private Vector2 _touchStartPosition;
    private float _touchStartTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        ProcessKeyboardInput();
#endif
        ProcessTouchInput();
    }

    private void ProcessKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnMoveDirection?.Invoke(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnMoveDirection?.Invoke(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnMoveDirection?.Invoke(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnMoveDirection?.Invoke(Vector2.down);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _touchStartPosition = Input.mousePosition;
            _touchStartTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            ProcessTouchEnd(Input.mousePosition);
        }
    }

    private void ProcessTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStartPosition = touch.position;
                    _touchStartTime = Time.time;
                    break;

                case TouchPhase.Ended:
                    ProcessTouchEnd(touch.position);
                    break;
            }
        }
    }

    private void ProcessTouchEnd(Vector2 touchEndPosition)
    {
        Vector2 swipeDelta = touchEndPosition - _touchStartPosition;
        float touchDuration = Time.time - _touchStartTime;

        if (touchDuration <= _tapMaxDuration && swipeDelta.magnitude < _swipeThreshold && !IsPointerOverUIElement())
        {
            OnTap?.Invoke();
        }
        else if (swipeDelta.magnitude >= _swipeThreshold)
        {
            Vector2 normalizedDelta = swipeDelta.normalized;
            Vector2 biasedDirection = VectorHelper.ApplyAxisBias(normalizedDelta, _axisBias);

            OnSwipe?.Invoke(biasedDirection);
            OnRawSwipe?.Invoke(swipeDelta);
        }
    }

    private bool IsPointerOverUIElement()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        return EventSystem.current.IsPointerOverGameObject();
    }
}