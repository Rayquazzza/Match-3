using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Modifier : ", menuName = "Match3/GridModifier/AddItem")]
public class AddItemModifier : GridModifier
{
    public GameObject ItemPrefab;

    public List<Vector2Int> positions;

    public override void ApplyToGrid(GridController grid)
    {
        if (grid == null || ItemPrefab == null) return;
        foreach(var pos in positions)
        {
            grid.Spawner.SpawnItem(pos.x, pos.y, ItemPrefab);
        }
    }

    public override void ApplyToOverlays(GridController grid)
    {
       
    }
}
