using System;
using System.Collections.Generic;
using UnityEngine;

public struct MatchData
{
    public Vector3 worldPosition;
    public int score;
    public int combo;
    public List<GridItem> matchedItems;
}