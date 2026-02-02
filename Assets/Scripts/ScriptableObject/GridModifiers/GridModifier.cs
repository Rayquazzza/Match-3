using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridModifier : ScriptableObject
{
    public abstract void ApplyToGrid(GridController grid);

    public abstract void ApplyToOverlays(GridController grid);
}
