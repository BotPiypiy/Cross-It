using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyUIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RawImage _keyImage;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private string _lockTag;

    private Color _keyColor;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Vector2 _originalPosition;
    private Transform _originalParent;
    private PuzzleUIManager _puzzleManager;

    private bool _isDragging = false;
    private bool _canDrag = true;

    public void Initialize(Color color, PuzzleUIManager manager)
    {
        _keyColor = color;
        _keyImage.color = color;
        _puzzleManager = manager;

        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_canDrag) return;

        _isDragging = true;
        _originalPosition = _rectTransform.anchoredPosition;

        _canvasGroup.alpha = 0.7f;
        _canvasGroup.blocksRaycasts = false;

        transform.SetParent(_canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, _canvas.worldCamera, out Vector2 localPoint))
        {
            _rectTransform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool droppedOnLock = false;

        foreach (var result in results)
        {
            bool lockArea = result.gameObject.CompareTag(_lockTag);
            if (lockArea)
            {
                droppedOnLock = true;
                _puzzleManager.OnKeyDroppedOnLock(_keyColor);
                DisableKey();
                break;
            }
        }

        if (!droppedOnLock)
        {
            ReturnToOriginalPosition();
        }
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(_originalParent);
        _rectTransform.anchoredPosition = _originalPosition;
    }

    private void DisableKey()
    {
        _canDrag = false;
        _keyImage.raycastTarget = false;
        _canvasGroup.alpha = 0.3f;
        gameObject.SetActive(false);
    }
}
