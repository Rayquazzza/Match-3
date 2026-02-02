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
                // 1. SI LA CASE EST UN TROU OU DÉJÀ PLEINE, ON PASSE À LA SUIVANTE
                if (!grid.IsValidPos(x, y) || grid.AllItems[x, y] != null) continue;

                List<E_CandyType> possibleCandies = new List<E_CandyType>(allTypes);

                // 2. VÉRIFICATION HORIZONTALE (avec sécurité null pour les trous)
                if (x >= 2)
                {
                    GridItem item1 = grid.AllItems[x - 1, y];
                    GridItem item2 = grid.AllItems[x - 2, y];
                    if (item1 != null && item2 != null)
                    {
                        E_CandyType type1 = item1.GetItemType();
                        if (type1 == item2.GetItemType()) possibleCandies.Remove(type1);
                    }
                }

                // 3. VÉRIFICATION VERTICALE (avec sécurité null pour les trous)
                if (y >= 2)
                {
                    GridItem item1 = grid.AllItems[x, y - 1];
                    GridItem item2 = grid.AllItems[x, y - 2];
                    if (item1 != null && item2 != null)
                    {
                        E_CandyType type1 = item1.GetItemType();
                        if (type1 == item2.GetItemType()) possibleCandies.Remove(type1);
                    }
                }

                E_CandyType chosenType = possibleCandies[Random.Range(0, possibleCandies.Count)];
                SpawnCandy(x, y, chosenType);
            }
        }

        // 4. VÉRIFICATION DE LA POSSIBILITÉ DE JOUER
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
        ResetGrid();
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
        // Sécurité doublée
        if (!grid.IsValidPos(x, y)) return;

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