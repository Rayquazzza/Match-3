using UnityEngine;

[CreateAssetMenu(menuName = "Match3/Effects/VerticalLine")]
public class VerticalLineEffect : SpecialEffect
{
    public override void Execute(int x, int y, GridData grid, GridController controller)
    {
        for (int j = 0; j < grid.Height; j++)
        {
            grid.ClearMatchAt(x, j);
        }
    }

    public override void ExecuteCombo(GridItem source, GridItem target, GridData grid, GridController controller)
    {
        // Empty implementation
    }
}