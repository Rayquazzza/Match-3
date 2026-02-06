using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchChecker
{
    private int width;
    private int height;

    public MatchChecker(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public List<GridItem> GetAllMatches(GridItem[,] allItems)
    {
        HashSet<GridItem> matchesFound = new HashSet<GridItem>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GridItem current = allItems[x, y];
              
                if (current == null || !current.IsMatchable) continue;

                CandyItemData currentId = current.GetItemType();

                if (x < width - 2)
                {
                    if (allItems[x + 1, y]?.GetItemType() == currentId &&
                        allItems[x + 2, y]?.GetItemType() == currentId)
                    {
                        matchesFound.Add(allItems[x, y]);
                        matchesFound.Add(allItems[x + 1, y]);
                        matchesFound.Add(allItems[x + 2, y]);
                    }
                }

                if (y < height - 2)
                {
                    if (allItems[x, y + 1]?.GetItemType() == currentId &&
                        allItems[x, y + 2]?.GetItemType() == currentId)
                    {
                        matchesFound.Add(allItems[x, y]);
                        matchesFound.Add(allItems[x, y + 1]);
                        matchesFound.Add(allItems[x, y + 2]);
                    }
                }
            }
        }
        return new List<GridItem>(matchesFound);
    }

    public List<List<GridItem>> GetDetailedMatches(GridItem[,] allCandies)
    {
        List<List<GridItem>> horizontalMatches = new List<List<GridItem>>();
        List<List<GridItem>> verticalMatches = new List<List<GridItem>>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                GridItem c = allCandies[x, y];
                if (c == null) continue;

                List<GridItem> match = new List<GridItem> { c };

                for (int i = x + 1; i < width; i++)
                {
                    if (allCandies[i, y]?.GetItemType() == c.GetItemType()) match.Add(allCandies[i, y]);
                    else break;
                }
                if (match.Count >= 3)
                {
                    horizontalMatches.Add(match);
                    x += match.Count - 1;
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                GridItem c = allCandies[x, y];
                if (c == null) continue;

                List<GridItem> match = new List<GridItem> { c };

                for (int i = y + 1; i < height; i++)
                {
                    if (allCandies[x, i]?.GetItemType() == c.GetItemType()) match.Add(allCandies[x, i]);
                    else break;
                }
                if (match.Count >= 3)
                {
                    verticalMatches.Add(match);
                    y += match.Count - 1;
                }
            }
        }

        List<List<GridItem>> combinedMatches = new List<List<GridItem>>();


        foreach (var hMatch in horizontalMatches)
        {
            bool joined = false;

            foreach (var vMatch in verticalMatches)
            {
                if (hMatch.Any(item => vMatch.Contains(item)))
                {
                    combinedMatches.Add(hMatch.Union(vMatch).ToList());
                    verticalMatches.Remove(vMatch);
                    joined = true;
                    break;
                }
            }
            if (!joined) combinedMatches.Add(hMatch);
        }

        combinedMatches.AddRange(verticalMatches);

        return combinedMatches;
    }

    public bool IsMovePossible(GridItem[,] allCandies)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allCandies[x, y] == null || !allCandies[x, y].IsMovable) continue;

                if (WouldMatchOccur(x, y, x + 1, y, allCandies) || WouldMatchOccur(x, y, x, y + 1, allCandies))
                    return true;
            }
        }
        return false;
    }

    private bool WouldMatchOccur(int x1, int y1, int x2, int y2, GridItem[,] allCandies)
    {
        if (x2 < 0 || x2 >= width || y2 < 0 || y2 >= height) return false;

        GridItem item1 = allCandies[x1, y1];
        GridItem item2 = allCandies[x2, y2];

        if (item1 == null || item2 == null) return false;

        if (!item1.IsMovable || !item2.IsMovable) return false;

        CandyItemData candyType1 = item1.GetItemType();
        CandyItemData candyType2 = item2.GetItemType();

        return TestPos(x1, y1, candyType2, x2, y2, allCandies) ||
               TestPos(x2, y2, candyType1, x1, y1, allCandies);
    }

    private bool TestPos(int x, int y, CandyItemData candyType, int skipX, int skipY, GridItem[,] allCandies)
    {
        return (Count(x, y, 1, 0, candyType, skipX, skipY, allCandies) + Count(x, y, -1, 0, candyType, skipX, skipY, allCandies) >= 2) ||
               (Count(x, y, 0, 1, candyType, skipX, skipY, allCandies) + Count(x, y, 0, -1, candyType, skipX, skipY, allCandies) >= 2);
    }

    private int Count(int x, int y, int dx, int dy, CandyItemData candyType, int skipX, int skipY, GridItem[,] allCandies)
    {
        int count = 0;
        for (int i = 1; i < 3; i++)
        {
            int nx = x + dx * i, ny = y + dy * i;
            if (nx < 0 || nx >= width || ny < 0 || ny >= height || (nx == skipX && ny == skipY)) break;
            if (allCandies[nx, ny]?.GetItemType() == candyType) count++; else break;
        }
        return count;
    }
}