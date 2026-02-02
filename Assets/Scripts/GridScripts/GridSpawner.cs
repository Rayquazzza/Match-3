using DG.Tweening;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GridSpawner
{
    private GridController controller;
    private GridData grid;

    public GridSpawner(GridController controller, GridData grid)
    {
        this.controller = controller;
        this.grid = grid;
    }

    public void GenerateGrid()
    {
        E_CandyType[] allTypes = System.Array.FindAll((E_CandyType[])System.Enum.GetValues(typeof(E_CandyType)), t => t != E_CandyType.None);

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {

                if (grid.AllItems[x, y] != null) continue;

                List<E_CandyType> possibleCandies = new List<E_CandyType>(allTypes);

                if (x >= 2)
                {
                    E_CandyType left1 = grid.AllItems[x - 1, y].GetItemType();
                    E_CandyType left2 = grid.AllItems[x - 2, y].GetItemType();
                    if (left1 == left2) possibleCandies.Remove(left1);
                }

                if (y >= 2)
                {
                    E_CandyType down1 = grid.AllItems[x, y - 1].GetItemType();
                    E_CandyType down2 = grid.AllItems[x, y - 2].GetItemType();
                    if (down1 == down2) possibleCandies.Remove(down1);
                }

                E_CandyType chosenType = possibleCandies[Random.Range(0, possibleCandies.Count)];

                controller.Spawner.SpawnCandy(x, y, chosenType);
            }
        }

        if (!controller.Match.IsMovePossible(grid.AllItems))
        {
            Debug.Log("Aucun coup possible, régénération...");
            ClearAndRestart();
        }
    }

    public void ResetGrid()
    {
        DOTween.KillAll();
        if (grid.AllItems == null) return;

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.AllItems[x, y] != null)
                {
                    grid.AllItems[x, y].OnDestroyItem();
                    grid.AllItems[x, y] = null;
                }

                if (grid.AllOverlays != null && grid.AllOverlays[x, y] != null)
                {
                    grid.ClearOverlayAt(x, y);
                }
            }
        }

        controller.StopAllCoroutines();
        controller.SetIsProcessing(false);
    }

    public void ClearAndRestart()
    {
        DOTween.KillAll();
        foreach (Candy c in grid.AllItems) if (c != null) c.Destroy();
        GenerateGrid();
    }
    public void SpawnItem(int x, int y, GameObject prefab)
    {
        if (grid.AllItems[x, y] != null) return;

        Vector3 pos = controller.Visualizer.GetWorldPosition(x, y);
        GameObject go = GameServiceLocator.Get<IPoolingService>().GetFromPool(prefab, pos, Quaternion.identity);
        go.transform.SetParent(controller.transform);

        GridItem item = go.GetComponent<GridItem>();
        item.Init(controller);
        grid.SetItem(x, y, item);
    }

    public void SpawnCandy(int x, int y, E_CandyType type, bool isRefill = false)
    {
        Vector3 targetPos = controller.Visualizer.GetWorldPosition(x, y);
        Vector3 spawnPos = targetPos;

        if (isRefill)
        {
            float offsetY = (grid.Height * controller.Spacing);
            spawnPos = new Vector3(targetPos.x, offsetY, 0);
        }

        GameObject go = GameServiceLocator.Get<IPoolingService>().GetFromPool(controller.BaseCandyPrefab, spawnPos, Quaternion.identity);
        go.transform.SetParent(controller.transform);

        Candy candy = go.GetComponent<Candy>();
        candy.SetType(type);
        candy.Init(controller);

        grid.SetItem(x, y, candy);

        if (isRefill)
        {
            controller.Visualizer.MoveItem(candy, x, y);
        }
    }

    public void SpawnOverlay(int x, int y, GameObject prefab)
    {
        Vector3 pos = controller.Visualizer.GetWorldPosition(x, y);
        GameObject go = GameServiceLocator.Get<IPoolingService>().GetFromPool(prefab, pos, Quaternion.identity);
        go.transform.SetParent(controller.transform);

        GridItem item = go.GetComponent<GridItem>();
        item.Init(controller);

        grid.SetOverlay(x, y, item);
    }
}