using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

public class GridSwap 
{
    private GridController controller;
    private GridData grid;

    public GridSwap(GridController controller)
    {
        this.controller = controller;
        grid = controller.Grid;
        GameServiceLocator.Get<IMoveService>().OnSwapAttempt += HandleSwapAttempt;
    }

    private void HandleSwapAttempt(Candy candy, Vector2Int dir)
    {
        controller.StartCoroutine(TrySwap(candy, dir));
    }

    private IEnumerator TrySwap(GridItem item1, Vector2Int direction)
    {
        if (controller.IsProcessing) yield break;

        Vector2Int pos1 = controller.Grid.GetPositionOf(item1);
        Vector2Int pos2 = pos1 + direction;

        if (controller.Grid.GetOverlay(pos1.x, pos1.y) != null ||
        controller.Grid.GetOverlay(pos2.x, pos2.y) != null)
        {
            yield break;
        }

        if (pos2.x >= 0 && pos2.x < grid.Width && pos2.y >= 0 && pos2.y < grid.Height)
        {
            GridItem item2 = grid.AllItems[pos2.x, pos2.y];

            if (!item1.IsMovable || (item2 != null && !item2.IsMovable)) yield break;

            controller.SetLastSwapPos(pos2);
            controller.SetIsProcessing(true);

            if (item2 != null) controller.Visualizer.MoveItem(item1, pos2.x, pos2.y, controller.ShiftSpeed);
            if (item2 != null) controller.Visualizer.MoveItem(item2, pos1.x, pos1.y, controller.ShiftSpeed);

            grid.AllItems[pos1.x, pos1.y] = item2;
            grid.AllItems[pos2.x, pos2.y] = item1;

            yield return new WaitForSeconds(controller.ShiftSpeed);

            Candy candy1 = item1 as Candy;
            Candy candy2 = item2 as Candy;


            if (candy1 != null && candy2 != null)
            {
                if (candy1.TriggerSpecialEffect(candy2) || candy2.TriggerSpecialEffect(candy1))
                {
                    grid.AllItems[pos1.x, pos1.y] = null;
                    grid.AllItems[pos2.x, pos2.y] = null;

                    yield return controller.StartCoroutine(controller.Shifter.ShiftAndRefill());

                    yield return controller.StartCoroutine(controller.Processor.FindAndProcessMatchesRoutine());
                    yield break;
                }
            }

            if (controller.Match.GetAllMatches(grid.AllItems).Count > 0)
            {
                yield return controller.StartCoroutine(controller.Processor.FindAndProcessMatchesRoutine());
            }
            else
            {
                controller.Visualizer.MoveItem(item1, pos1.x, pos1.y, controller.ShiftSpeed);
                if (item2 != null) controller.Visualizer.MoveItem(item2, pos2.x, pos2.y, controller.ShiftSpeed);

                grid.AllItems[pos1.x, pos1.y] = item1;
                grid.AllItems[pos2.x, pos2.y] = item2;
                controller.SetIsProcessing(false);
            }
        }
    }

    ~GridSwap()
    {
        Dispose();
    }

    private void Dispose()
    {
        GameServiceLocator.Get<IMoveService>().OnSwapAttempt -= HandleSwapAttempt;
    }
}
