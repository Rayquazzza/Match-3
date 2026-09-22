using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSlot
{
    public bool isValid = true;
    public GridItemData baseItem;
    public OverlayItemData overlayItem;
}


[CreateAssetMenu(fileName = "Level_", menuName = "Match3/LevelData")]

public class LevelData : ScriptableObject
{
    public int width = 4;
    public int height = 8;

    public int levelID;



    public int maxMoves = 10;


    public LevelGoals goals;

    public List<CandyItemData> availableCandies;

    public List<GridItemData> specialItems;



    public LevelSlot[] grid;

    public void Initialize()
    {
        grid = new LevelSlot[width * height];

        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = new LevelSlot();
        }
    }

}