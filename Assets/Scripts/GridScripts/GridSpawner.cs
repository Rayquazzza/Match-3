using DG.Tweening;
using System.Collections;
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
        List<CandyItemData> allTypes = new List<CandyItemData>();

        foreach (var item in controller.CurrentLevel.availableCandies)
        {
            if (item != null)
            {
                allTypes.Add(item);
            }
        }

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                int dataIndex = y * grid.Width + x;
                var slotData = controller.CurrentLevel.grid[dataIndex];

                if (!slotData.isValid) continue;

                if (slotData.baseItem != null)
                {
                    SpawnItem(x, y, slotData.baseItem);
                }
                else
                {
                    List<CandyItemData> possibleCandies = GetValidRandomCandies(x, y, allTypes);
                    CandyItemData chosenType = possibleCandies[Random.Range(0, possibleCandies.Count)];

                    SpawnCandy(x, y, chosenType);
                }

                if (slotData.overlayItem != null)
                {
                    SpawnOverlay(x, y, slotData.overlayItem);
                }
            }
        }

        if (!controller.Match.IsMovePossible(grid.AllItems))
        {
            Debug.Log("Aucun coup possible, régénération...");
            ClearAndRestart();
        }
    }


    private List<CandyItemData> GetValidRandomCandies(int x, int y, List<CandyItemData> pool)
    {
        List<CandyItemData> possible = new List<CandyItemData>(pool);

        if (x >= 2)
        {
            var item1 = grid.AllItems[x - 1, y];
            var item2 = grid.AllItems[x - 2, y];
            if (item1 != null && item2 != null)
            {
                var type1 = item1.GetItemType(); 
                if (type1 == item2.GetItemType()) possible.Remove(type1 as CandyItemData);
            }
        }

        if (y >= 2)
        {
            var item1 = grid.AllItems[x, y - 1];
            var item2 = grid.AllItems[x, y - 2];
            if (item1 != null && item2 != null)
            {
                var type1 = item1.GetItemType();
                if (type1 == item2.GetItemType()) possible.Remove(type1 as CandyItemData);
            }
        }

        return possible;
    }

    public void ResetGrid()
    {
        Debug.Log("Resetting Grid...");
        DOTween.KillAll();
        if (grid.AllItems == null)
        {
            Debug.LogWarning("GridSpawner: AllItems is null, skipping ResetGrid.");
            return;
        }
            
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.AllItems[x, y] != null && grid.IsValidPos(x,y))
                {
                    grid.AllItems[x, y].OnDestroyItem();
                    grid.AllItems[x, y] = null;
                }
                else
                {
                    Debug.Log("No item to destroy");
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
        ResetGrid();
        GenerateGrid();
    }

    public void SpawnItem(int x, int y, GridItemData data)
    {
        if (grid.AllItems[x, y] != null || data == null) return;

        Vector3 pos = controller.Visualizer.GetWorldPosition(x, y);

        GameObject go = GameServiceLocator.Get<IPoolingService>().GetFromPool(data.prefab, pos, Quaternion.identity);
        go.transform.SetParent(controller.transform);

        GridItem item = go.GetComponent<GridItem>();
        if(item) item.Init(controller);

        if(item) item.SetType(data);

        grid.SetItem(x, y, item);
    }

    public void SpawnCandy(int x, int y, CandyItemData type, bool isRefill = false)
    {
        if (!grid.IsValidPos(x, y)) return;

        Vector3 targetPos = controller.Visualizer.GetWorldPosition(x, y);
        Vector3 spawnPos = targetPos;

        if (isRefill)
        {
            float offsetY = (grid.Height * controller.Spacing);
            spawnPos = new Vector3(targetPos.x, offsetY, 0);
        }

        GameObject go = GameServiceLocator.Get<IPoolingService>().GetFromPool(type.prefab, spawnPos, Quaternion.identity);
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

    public void SpawnOverlay(int x, int y, GridItemData data)
    {
        if (data == null) return;

        Vector3 pos = controller.Visualizer.GetWorldPosition(x, y);
        GameObject go = GameServiceLocator.Get<IPoolingService>().GetFromPool(data.prefab, pos, Quaternion.identity);
        go.transform.SetParent(controller.transform);

        GridItem item = go.GetComponent<GridItem>();
        item.Init(controller);
        item.SetType(data); 

        grid.SetOverlay(x, y, item);

        foreach (var layer in grid.AllOverlays)
        {
            if (layer != null) 
            {
                Debug.Log($"Overlay trouvé : {layer.name}");
            }
        }
    }

    public IEnumerator ShuffleGrid()
    {
        List<GridItem> itemsToShuffle = new List<GridItem>();
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                GridItem item = grid.AllItems[x, y];
                if (item != null && item.IsMovable && grid.GetOverlay(x, y) == null)
                {
                    itemsToShuffle.Add(item);
                    availablePositions.Add(new Vector2Int(x, y));
                    grid.AllItems[x, y] = null;
                }
            }
        }

        for (int i = 0; i < availablePositions.Count; i++)
        {
            Vector2Int temp = availablePositions[i];
            int randomIndex = Random.Range(i, availablePositions.Count);
            availablePositions[i] = availablePositions[randomIndex];
            availablePositions[randomIndex] = temp;
        }

        for (int i = 0; i < itemsToShuffle.Count; i++)
        {
            Vector2Int newPos = availablePositions[i];
            grid.AllItems[newPos.x, newPos.y] = itemsToShuffle[i];
            controller.Visualizer.MoveItem(itemsToShuffle[i], newPos.x, newPos.y, 0.5f);
        }

        yield return new WaitForSeconds(0.5f);
    }
}