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

    public MatchProcessor(GridController controller, GridData grid)
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


        // --- SUFFLE LOGIC ---
        int safetyBreak = 0;
        while (!controller.Match.IsMovePossible(grid.AllItems) && safetyBreak < 5)
        {
            yield return controller.StartCoroutine(controller.Spawner.ShuffleGrid());
            safetyBreak++;
        }

        if (!controller.Match.IsMovePossible(grid.AllItems))
        {
            Debug.Log("Plus aucun coup possible. Reset total.");
            controller.Spawner.ClearAndRestart();
        }
        // --------------------


        // End of processing
        GameServiceLocator.Get<IMoveService>().UseMove();
        controller.SetIsProcessing(false);
    }

    private void HandleMatchSpawn(List<GridItem> currentMatch)
    {
        if (currentMatch == null || currentMatch.Count == 0) return;

        MatchResult result = AnalyzeMatch(currentMatch);

        int totalMatchScore = (result.Items.Count * 50) * comboCount;
        GameServiceLocator.Get<IScoreService>().AddScore(totalMatchScore);

        Vector2Int firstItemGridPos = grid.GetPositionOf(result.Items[0]);
        Vector3 popupPos = controller.Visualizer.GetWorldPosition(firstItemGridPos.x, firstItemGridPos.y);

        GameServiceLocator.Get<IEffectService>().ShowScorePopup(popupPos, totalMatchScore);

        foreach (GridItem c in result.Items)
        {
            Vector2Int gridPos = grid.GetPositionOf(c);
            if (gridPos.x == -1) continue;

            NotifyNeighbors(gridPos);

            GridItem overlay = grid.GetOverlay(gridPos.x, gridPos.y);
            if (overlay != null)
            {
                overlay.BreakLayer();

                if (gridPos == result.SpawnPos) result.SpawnPos = new Vector2Int(-1, -1);
                continue;
            }

            grid.ClearMatchAt(gridPos.x, gridPos.y);
        }

        if (result.CanSpawnBonus)
        {
            controller.Spawner.SpawnItem(result.SpawnPos.x, result.SpawnPos.y, result.Pattern.bonusPrefab);
        }
    }

    private MatchResult AnalyzeMatch(List<GridItem> match)
    {
        int maxH = 0, maxV = 0;
        List<Vector2Int> posList = match.Select(item => grid.GetPositionOf(item)).ToList();

        foreach (var p in posList)
        {
            maxH = Mathf.Max(maxH, posList.Count(other => other.y == p.y));
            maxV = Mathf.Max(maxV, posList.Count(other => other.x == p.x));
        }

        int longestLine = Mathf.Max(maxH, maxV);

        MatchPattern bestPattern = controller.AvailablePatterns.OrderByDescending(p => p.priority).FirstOrDefault(p => longestLine >= p.minCount);

        Vector2Int spawnPos = (bestPattern != null) ? GetSpawnPositionForBonus(match) : new Vector2Int(-1, -1);

        return new MatchResult(match, bestPattern, spawnPos);
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