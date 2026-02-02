using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private GridItem[,] allItems;
    private GridItem[,] allOverlays;

    private bool[,] activeCells;

    public GridItem[,] AllItems
    {
        get { return allItems; }
        set { allItems = value; }
    }

    public GridItem[,] AllOverlays
    {
        get { return allOverlays; }
        set { allOverlays = value; }
    }

    public GridData(int width, int height)
    {
        Width = width;
        Height = height;
        allItems = new GridItem[width, height];
        allOverlays = new GridItem[width, height];
    }

    public bool IsValidPos(int x, int y)
    {
       return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public bool IsValidPos(Vector2Int pos)
    {
       return IsValidPos(pos.x, pos.y);
    }

    public GridItem GetItem(int x, int y)
    {
        return IsValidPos(x, y) ? allItems[x, y] : null;
    }

    public void SetItem(int x, int y, GridItem item)
    {
        if (IsValidPos(x, y)) allItems[x, y] = item;
    }

    public GridItem GetOverlay(int x, int y)
    {
       return IsValidPos(x, y) ? allOverlays[x, y] : null;
    }

    public void SetOverlay(int x, int y, GridItem overlay) 
    { 
        if (IsValidPos(x, y)) allOverlays[x, y] = overlay; 
    }

    public Vector2Int GetPositionOf(GridItem item)
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (allItems[x, y] == item || allOverlays[x, y] == item) return new Vector2Int(x, y);
        return new Vector2Int(-1, -1);
    }

    public void ClearMatchAt(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return;
        GridItem gridItem = allItems[x, y];
        if (gridItem != null)
        {
            allItems[x, y] = null;
            gridItem.OnDestroyItem();
        }
    }

    public void ClearColor(E_CandyType candyType)
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (allItems[x, y]?.GetItemType() == candyType)
                    ClearMatchAt(x, y);
    }

    public void ClearOverlayAt(int x, int y)
    {
        if (!IsValidPos(new Vector2Int(x, y)) || allOverlays == null) return;

        GridItem overlay = allOverlays[x, y];

        if (overlay != null)
        {
            overlay.OnDestroyItem();

            allOverlays[x, y] = null;
        }
    }

    public bool HasEmptySpaces()
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (allItems[x, y] == null) return true;
        return false;
    }

    public bool CanMove(Vector2Int pos)
    {
        if (allOverlays != null && allOverlays[pos.x, pos.y] != null)
        {
            return false;
        }
        return allItems[pos.x, pos.y].IsMovable;
    }
}
