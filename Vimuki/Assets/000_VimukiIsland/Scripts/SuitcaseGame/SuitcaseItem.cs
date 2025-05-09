using UnityEngine;
using UnityEngine.EventSystems;

public class SuitcaseItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Vector2Int cellSize;

    private bool _isRotated;
    private bool _rightClickHandled;
    private Canvas _canvas;
    private RectTransform _originalParent;
    private Vector3 _originalPosition;

    private RectTransform _rectTransform;
    private SuitcaseManager _suitcaseManager;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _suitcaseManager = FindFirstObjectByType<SuitcaseManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalParent = (RectTransform)transform.parent;
        _originalPosition = _rectTransform.anchoredPosition;

        
        transform.SetParent(_canvas.transform);
        _rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        //_rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        // Rotation
        if (!_rightClickHandled && Input.GetMouseButtonDown(1))
        {
            ToggleRotation();
            _rightClickHandled = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            _rightClickHandled = false;
        }

        _suitcaseManager.HighlightCellsUnderItem(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_suitcaseManager.TryPlaceItem(this))
        {
            transform.SetParent(_suitcaseManager.ItemParent, true);
        }
        else
        {
            //Can't be placed
            _rectTransform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = _originalPosition;
        }

        _suitcaseManager.ClearHighlights();
    }

    public Vector2Int GetSize()
    {
        return _isRotated ? new Vector2Int(cellSize.y, cellSize.x) : cellSize;
    }

    private void ToggleRotation()
    {
        _isRotated = !_isRotated;
        _rectTransform.rotation = Quaternion.Euler(0, 0, _isRotated ? 90 : 0);
    }
}
