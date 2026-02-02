using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchProcessor
{
    private GridController controller;
    private GridData grid;
    private MatchChecker matchChecker;
    private int comboCount = 0;

    public MatchProcessor(GridController controller,GridData grid)
    {
        this.controller = controller;
        this.grid = grid;
        matchChecker = new MatchChecker(grid.Width, grid.Height);
    }

    public IEnumerator FindAndProcessMatchesRoutine()
    {
        comboCount = 0;
        bool matchesFoundThisCycle = true;

        while (matchesFoundThisCycle)
        {
            List<List<GridItem>> allMatches = matchChecker.GetDetailedMatches(grid.AllItems);

            if (allMatches.Count > 0)
            {
                matchesFoundThisCycle = true;
                comboCount++;

                foreach (List<GridItem> currentMatch in allMatches)
                {
                    HandleMatchSpawn(currentMatch);
                }

                yield return new WaitForSeconds(0.2f);

                yield return controller.StartCoroutine(controller.Shifter.ShiftAndRefill());
            }
            else
            {
                matchesFoundThisCycle = false;
            }
        }

        GameServiceLocator.Get<IMoveService>().UseMove();
        controller.SetIsProcessing(false);
    }

    private void HandleMatchSpawn(List<GridItem> currentMatch)
    {
        if (currentMatch == null || currentMatch.Count == 0) return;


        // --- We need to determine the best pattern for the match ---
        int maxH = 0;
        int maxV = 0;
        var posList = currentMatch.Select(item => grid.GetPositionOf(item)).ToList();

        foreach (var p in posList)
        {
            int countH = posList.Count(other => other.y == p.y);
            int countV = posList.Count(other => other.x == p.x);
            if (countH > maxH) maxH = countH;
            if (countV > maxV) maxV = countV;
        }

        MatchPattern bestPattern = null;
        int longestLine = Mathf.Max(maxH, maxV);

        foreach (var pattern in controller.AvailablePatterns.OrderByDescending(p => p.priority))
        {

            if (longestLine >= pattern.minCount)
            {
                bestPattern = pattern;
                break;
            }
        }


        // --- We calculate score and popup position ---
        Vector2Int gridCenterPos = posList[0];
        Vector3 popupWorldPos = controller.Visualizer.GetWorldPosition(gridCenterPos.x, gridCenterPos.y);

        int baseScorePerCandy = 50;
        int totalMatchScore = (currentMatch.Count * baseScorePerCandy) * comboCount;
        GameServiceLocator.Get<IScoreService>().AddScore(totalMatchScore);

        // Déterminer où le bonus doit apparaître
        Vector2Int spawnBonusPos = (bestPattern != null) ? GetSpawnPositionForBonus(currentMatch) : new Vector2Int(-1, -1);

        // --- ÉTAPE 4 : Nettoyage de la grille ---
        foreach (GridItem c in currentMatch)
        {
            Vector2Int gridPos = grid.GetPositionOf(c);
            if (gridPos.x != -1)
            {
                NotifyNeighbors(gridPos);

                GridItem overlay = grid.GetOverlay(gridPos.x, gridPos.y);
                if (overlay != null)
                {
                    overlay.BreakLayer();
                    // Si on casse un obstacle sur la case du bonus, on annule le spawn du bonus
                    if (gridPos == spawnBonusPos) spawnBonusPos = new Vector2Int(-1, -1);
                    continue;
                }

                grid.ClearMatchAt(gridPos.x, gridPos.y);
            }
        }

        // --- ÉTAPE 5 : Spawn du bonus ---
        if (bestPattern != null && spawnBonusPos.x != -1)
        {
            controller.Spawner.SpawnItem(spawnBonusPos.x, spawnBonusPos.y, bestPattern.bonusPrefab);
        }

        GameServiceLocator.Get<IEffectService>().ShowScorePopup(popupWorldPos, totalMatchScore);
    }

    private void NotifyNeighbors(Vector2Int pos)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            Vector2Int neighborPos = pos + dir;
            if (controller.Grid.IsValidPos(neighborPos))
            {
                GridItem neighbor = grid.AllItems[neighborPos.x, neighborPos.y];
                if (neighbor != null)
                {
                    neighbor.OnNearbyMatch();
                }
            }
        }
    }


    public Vector2Int GetSpawnPositionForBonus(List<GridItem> match)
    {
        foreach (GridItem item in match)
        {
            Vector2Int pos = controller.Grid.GetPositionOf(item);
            if (pos == controller.LastSwapPos) return pos;
        }
        return controller.Grid.GetPositionOf(match[0]);
    }
}