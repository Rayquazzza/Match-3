using System.Collections.Generic;
using UnityEngine;

public class MatchResult
{
    public List<GridItem> Items;
    public MatchPattern Pattern;
    public Vector2Int SpawnPos;
    public bool CanSpawnBonus => Pattern != null && SpawnPos.x != -1;

    public MatchResult(List<GridItem> items, MatchPattern pattern, Vector2Int pos)
    {
        Items = items;
        Pattern = pattern;
        SpawnPos = pos;
    }
}