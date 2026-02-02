using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "IceOverlay", menuName = "Match3/GridModifier/IceOverlay")]
public class IceModifier : GridModifier
{
    public List<Vector2Int> positions;
    public GameObject icePrefab;

    public override void ApplyToGrid(GridController grid)
    {
        
    }

    public override void ApplyToOverlays(GridController grid)
    {
        foreach (var pos in positions)
        {
            grid.Spawner.SpawnOverlay(pos.x, pos.y, icePrefab);
        }
    }

    
}
