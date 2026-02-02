using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridShifter
{
    private GridController controller;
    private GridData grid;

    public GridShifter(GridController controller)
    {
        this.controller = controller;
        grid = controller.Grid;
    }

    public IEnumerator ShiftAndRefill()
    {
        yield return controller.StartCoroutine(ShiftTilesDownRoutine());
        yield return controller.StartCoroutine(RefillGridRoutine());
    }

    private IEnumerator ShiftTilesDownRoutine()
    {
        int width = grid.Width;
        int height = grid.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                GridItem itemToMove = grid.AllItems[x, y];

                if (itemToMove != null && itemToMove.IsMovable)
                {
                    int targetY = y;

                    while (targetY > 0 && CanFallInto(x, targetY - 1))
                    {
                        targetY--;
                    }

                    if (targetY != y)
                    {
                        grid.AllItems[x, targetY] = itemToMove;
                        grid.AllItems[x, y] = null;

                        controller.Visualizer.MoveItem(itemToMove, x, targetY,controller.ShiftSpeed, Ease.OutBounce);
                    }
                }
            }
        }
        yield return new WaitForSeconds(controller.ShiftSpeed);
    }

    private bool CanFallInto(int x, int y)
    {
        if (!grid.IsValidPos(x, y)) return false;

        if (grid.GetItem(x, y) != null) return false;
        if (grid.GetOverlay(x, y) != null) return false;

        return true;
    }

    private IEnumerator RefillGridRoutine() 
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.AllItems[x, y] == null)
                {
                    E_CandyType[] candyTypeList = System.Array.FindAll((E_CandyType[])System.Enum.GetValues(typeof(E_CandyType)), t => t != E_CandyType.None);

                    E_CandyType chosenType = candyTypeList[Random.Range(0, candyTypeList.Length)];


                    controller.Spawner.SpawnCandy(x, y, chosenType);
                }
            }
        }
        yield return new WaitForSeconds(0.3f);
    }
}