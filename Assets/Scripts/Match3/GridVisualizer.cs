using UnityEngine;
using DG.Tweening;
using System;

public class GridVisualizer
{
    private int width, height;
    private float spacing;

    private GridController controller;

    public GridVisualizer(GridController controller, int width, int height, float spacing)
    {
        this.controller = controller;
        this.width = width;
        this.height = height;
        this.spacing = spacing;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        float offsetX = (width - 1) * spacing / 2f;
        float offsetY = (height - 1) * spacing / 2f;
        return new Vector3(x * spacing - offsetX, y * spacing - offsetY, 0);
    }
    public Vector3 GetWorldPosition(GridItem item)
    {
        Vector2Int pos = controller.Grid.GetPositionOf(item);

        return GetWorldPosition(pos.x, pos.y);
    }

    public void MoveItem(GridItem candy, int x, int y, float speed = 0.3f, Ease easeType = Ease.OutQuad)
    {
        Vector3 targetPosition = GetWorldPosition(x, y);
        float duration = (easeType == Ease.OutBounce) ? 0.5f : speed;

        candy.transform.DOKill();
        candy.transform.DOMove(targetPosition, duration).SetEase(easeType);
    }

    public void SpawnLightning(Vector3 position1, Vector3 position2, float travelTime)
    {
        
    }
}