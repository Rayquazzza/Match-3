using UnityEngine;

public abstract class SpecialEffect : ScriptableObject
{
    public abstract void Execute(int x, int y, GridData grid, GridController controller);

    public abstract void ExecuteCombo(GridItem source, GridItem target, GridData grid, GridController controller);
}