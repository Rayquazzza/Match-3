using UnityEngine;

[CreateAssetMenu(menuName = "Match3/Effects/Explosion")]
public class ExplosionEffect : SpecialEffect
{
    public override void Execute(int x, int y, GridData grid, GridController controller)
    {
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (grid.IsValidPos(i, j))
                {
                    grid.ClearMatchAt(i, j);
                }
            }
        }
    }

    public override void ExecuteCombo(GridItem source, GridItem target, GridData grid, GridController controller)
    {
        // Empty implementation
    }
}