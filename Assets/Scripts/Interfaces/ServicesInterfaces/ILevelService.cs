using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelService 
{
    public LevelData CurrentLevelData { get; }

    public event Action<LevelData> OnLoadLevelData;

    public void LoadLevelData(LevelData levelData);

    void LoadLevelData(LevelData levelData, E_GameState gameState);

    void SaveLevelProgress(int levelID, int starsEarned);

    void ResetProgress();
}
