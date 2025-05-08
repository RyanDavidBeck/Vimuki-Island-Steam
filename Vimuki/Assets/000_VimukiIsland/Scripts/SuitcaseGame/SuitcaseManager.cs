using System.Collections.Generic;
using UnityEngine;

public class SuitcaseManager : MonoBehaviour
{
    public GridCell[,] grid = new GridCell[8, 8];
    public float cellSize = 110f;

    [SerializeField] private RectTransform gridRoot;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private RectTransform itemParent;

    public RectTransform ItemParent => itemParent;

    private void Start()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var cellGo = Instantiate(cellPrefab, gridRoot);
                var cell = cellGo.GetComponent<GridCell>();
                cell.Init(new Vector2Int(x, y));

                var cellRect = cellGo.GetComponent<RectTransform>();
                cellRect.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);

                grid[x, y] = cell;
            }
        }
    }

    public void HighlightCellsUnderItem(SuitcaseItem item)
    {
        ClearHighlights();

        var size = item.GetSize();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRoot, Input.mousePosition, null, out var localPoint);
        var gridPos = GetCenteredGridPosition(localPoint, size);

        if (!IsInBounds(gridPos, size)) return;

        var canPlace = true;
        List<GridCell> cells = new();

        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                var pos = gridPos + new Vector2Int(x, y);
                var cell = grid[pos.x, pos.y];
                cells.Add(cell);

                if (cell.IsOccupied)
                    canPlace = false;
            }
        }

        foreach (var cell in cells)
        {
            cell.SetColor(cell.IsOccupied ? Color.red : Color.green);
        }
    }

    public bool TryPlaceItem(SuitcaseItem item)
    {
        var size = item.GetSize();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRoot, Input.mousePosition, null, out var localPoint);
        var gridPos = GetCenteredGridPosition(localPoint, size);

        if (!IsInBounds(gridPos, size)) return false;

        List<GridCell> targetCells = new();

        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                var cell = grid[gridPos.x + x, gridPos.y + y];
                if (cell.IsOccupied)
                    return false;

                targetCells.Add(cell);
            }
        }

        foreach (var cell in targetCells)
        {
            cell.SetOccupied(item);
        }

        // Snap Item
        var pos = new Vector2(gridPos.x + size.x / 2f, gridPos.y + size.y / 2f) * cellSize;
        var itemTransform = (RectTransform)item.transform;
        itemTransform.anchoredPosition = new Vector2(pos.x, -pos.y);

        return true;
    }

    public void ClearHighlights()
    {
        foreach (var cell in grid)
        {
            cell.SetColor(Color.clear);
        }
    }

    private Vector2Int GetCenteredGridPosition(Vector2 localPoint, Vector2Int size)
    {
        var centerOffset = new Vector2((size.x - 1) / 2f, (size.y - 1) / 2f);
        var x = Mathf.FloorToInt((localPoint.x / cellSize) - centerOffset.x);
        var y = Mathf.FloorToInt((-localPoint.y / cellSize) - centerOffset.y);
        return new Vector2Int(x, y);
    }
    
    private Vector2Int GetGridPosition(Vector2 localPoint)
    {
        var x = Mathf.FloorToInt(localPoint.x / cellSize);
        var y = Mathf.FloorToInt(-localPoint.y / cellSize);
        return new Vector2Int(x, y);
    }

    private bool IsInBounds(Vector2Int pos, Vector2Int size)
    {
        return pos.x >= 0 && pos.y >= 0 &&
               pos.x + size.x <= 8 && pos.y + size.y <= 8;
    }
}
