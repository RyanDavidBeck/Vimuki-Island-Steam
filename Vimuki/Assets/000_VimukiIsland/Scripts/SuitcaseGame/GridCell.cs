using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool IsOccupied => _occupyingItem != null;

    private SuitcaseItem _occupyingItem;
    private Image _backgroundImage;

    private void Awake()
    {
        _backgroundImage = GetComponent<Image>();
    }

    public void Init(Vector2Int position)
    {
        gridPosition = position;
        Clear();
    }

    public void SetOccupied(SuitcaseItem item)
    {
        _occupyingItem = item;
        SetColor(Color.clear);
    }

    public void Clear()
    {
        _occupyingItem = null;
        SetColor(Color.clear);
    }

    public void SetColor(Color color)
    {
        if (_backgroundImage != null)
            _backgroundImage.color = color;
    }
}