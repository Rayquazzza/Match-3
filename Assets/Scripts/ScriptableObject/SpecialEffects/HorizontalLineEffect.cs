using UnityEngine;

[CreateAssetMenu(menuName = "Match3/Effects/HorizontalLine")]
public class HorizontalLineEffect : SpecialEffect
{
    public override void Execute(int x, int y, GridData grid, GridController controller)
    {
        for (int i = 0; i < grid.Width; i++)
        {
            controller.Grid.ClearMatchAt(i, y); 
        }
    }

    public override void ExecuteCombo(GridItem source, GridItem target, GridData grid, GridController controller)
    {
        Vector2Int pos = grid.GetPositionOf(source);
        if (pos.x == -1) return;

        if (target is SpecialCandy other)
        {
            if (other.Effect is HorizontalLineEffect || other.Effect is VerticalLineEffect)
            {
                Execute(pos.x, pos.y, grid, controller);

                for (int j = 0; j < grid.Height; j++) grid.ClearMatchAt(pos.x, j);
            }

            else if (other.Effect is ExplosionEffect)
            {
                for (int i = pos.x - 1; i <= pos.x + 1; i++)
                    for (int j = 0; j < grid.Height; j++)
                        if (grid.IsValidPos(i, j)) grid.ClearMatchAt(i, j);

                for (int j = pos.y - 1; j <= pos.y + 1; j++)
                    for (int i = 0; i < grid.Width; i++)
                        if (grid.IsValidPos(i, j)) grid.ClearMatchAt(i, j);
            }
        }
    }
}